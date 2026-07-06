using TaskFlow.Application.DTOs;
using TaskFlow.Application.Services.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Services.Implementations;

/// <summary>
/// Implementação do serviço de gerenciamento de notas (NoteManager).
/// </summary>
public class NoteService : INoteService
{
    private readonly INoteRepository _noteRepository;
    private readonly IAttachmentRepository _attachmentRepository;
    private readonly IFileStorageService _fileStorageService;

    public NoteService(
        INoteRepository noteRepository,
        IAttachmentRepository attachmentRepository,
        IFileStorageService fileStorageService)
    {
        _noteRepository = noteRepository;
        _attachmentRepository = attachmentRepository;
        _fileStorageService = fileStorageService;
    }

    /// <inheritdoc/>
    public async Task<NoteDto?> GetByIdAsync(int id)
    {
        var note = await _noteRepository.GetByIdAsync(id);
        return note is null ? null : MapToDto(note);
    }

    /// <inheritdoc/>
    public async Task<List<NoteDto>> GetByOwnerAsync(int ownerId, int? status = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        Domain.Enums.NoteStatus? statusEnum = status.HasValue ? (Domain.Enums.NoteStatus)status.Value : null;
        var notes = await _noteRepository.GetByOwnerAsync(ownerId, statusEnum, startDate, endDate);
        return notes.Select(MapToDto).ToList();
    }

    /// <inheritdoc/>
    public async Task<NoteDto> CreateAsync(int ownerId, CreateNoteDto dto)
    {
        var nextNumber = await _noteRepository.GetMaxNoteNumberAsync(ownerId) + 1;

        var note = new Note
        {
            OwnerId = ownerId,
            NoteNumber = nextNumber,
            Title = dto.Title.Trim(),
            Content = dto.Content?.Trim() ?? string.Empty,
            Status = (Domain.Enums.NoteStatus)dto.Status,
            CreatedAt = DateTime.UtcNow
        };

        await _noteRepository.AddAsync(note);
        return MapToDto(note);
    }

    /// <inheritdoc/>
    public async Task<NoteDto?> UpdateAsync(int noteId, UpdateNoteDto dto)
    {
        var note = await _noteRepository.GetByIdAsync(noteId);
        if (note is null) return null;

        note.Title = dto.Title.Trim();
        note.Content = dto.Content?.Trim() ?? string.Empty;
        note.Status = (Domain.Enums.NoteStatus)dto.Status;
        note.UpdatedAt = DateTime.UtcNow;

        await _noteRepository.UpdateAsync(note);
        return MapToDto(note);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int noteId)
    {
        await _noteRepository.DeleteAsync(noteId);
        return true;
    }

    /// <inheritdoc/>
    public async Task<AttachmentDto?> AddAttachmentAsync(int noteId, int ownerId, string fileName, string fileType, long fileSize, Stream fileStream)
    {
        var filePath = await _fileStorageService.SaveFileAsync("Uploads", $"{Guid.NewGuid()}_{fileName}", fileStream, ownerId);

        var attachment = new Attachment
        {
            NoteId = noteId,
            OwnerId = ownerId,
            FileName = fileName,
            FilePath = filePath,
            FileType = fileType,
            FileSize = fileSize,
            UploadedAt = DateTime.UtcNow
        };

        await _attachmentRepository.AddAsync(attachment);

        return new AttachmentDto
        {
            Id = attachment.Id,
            NoteId = attachment.NoteId,
            FileName = attachment.FileName,
            FileType = attachment.FileType,
            FileSize = attachment.FileSize,
            FileSizeFormatted = FormatFileSize(attachment.FileSize),
            UploadedAt = attachment.UploadedAt
        };
    }

    /// <inheritdoc/>
    public async Task<bool> RemoveAttachmentAsync(int attachmentId)
    {
        var attachment = await _attachmentRepository.GetByIdAsync(attachmentId);
        if (attachment is null) return false;

        await _fileStorageService.DeleteFileAsync(attachment.FilePath);
        await _attachmentRepository.DeleteAsync(attachmentId);
        return true;
    }

    private static NoteDto MapToDto(Note note)
    {
        return new NoteDto
        {
            Id = note.Id,
            NoteNumber = note.NoteNumber,
            OwnerId = note.OwnerId,
            Title = note.Title,
            Content = note.Content,
            Status = (int)note.Status,
            StatusName = note.Status.ToString(),
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.UpdatedAt,
            Attachments = note.Attachments.Select(a => new AttachmentDto
            {
                Id = a.Id,
                NoteId = a.NoteId,
                FileName = a.FileName,
                FileType = a.FileType,
                FileSize = a.FileSize,
                FileSizeFormatted = FormatFileSize(a.FileSize),
                UploadedAt = a.UploadedAt
            }).ToList()
        };
    }

    private static string FormatFileSize(long? bytes)
    {
        if (!bytes.HasValue) return "0 B";
        if (bytes.Value < 1024) return $"{bytes.Value} B";
        if (bytes.Value < 1024 * 1024) return $"{bytes.Value / 1024.0:F1} KB";
        return $"{bytes.Value / (1024.0 * 1024):F1} MB";
    }
}
