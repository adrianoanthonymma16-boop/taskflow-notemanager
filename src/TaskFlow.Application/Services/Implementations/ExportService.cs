using System.Text.Json;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Services.Interfaces;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Services.Implementations;

/// <summary>
/// Implementação do serviço de exportação de dados para PDF.
/// Delega a geração concreta dos PDFs para os serviços da camada de Infraestrutura.
/// </summary>
public class ExportService : IExportService
{
    private readonly ITaskRepository _taskRepository;
    private readonly INoteRepository _noteRepository;
    private readonly IPendingLogRepository _pendingLogRepository;
    private readonly IAttachmentRepository _attachmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPdfDataTransferGenerator _dataTransferGenerator;
    private readonly IPdfReportGenerator _reportGenerator;

    public ExportService(
        ITaskRepository taskRepository,
        INoteRepository noteRepository,
        IPendingLogRepository pendingLogRepository,
        IAttachmentRepository attachmentRepository,
        IUserRepository userRepository,
        IPdfDataTransferGenerator dataTransferGenerator,
        IPdfReportGenerator reportGenerator)
    {
        _taskRepository = taskRepository;
        _noteRepository = noteRepository;
        _pendingLogRepository = pendingLogRepository;
        _attachmentRepository = attachmentRepository;
        _userRepository = userRepository;
        _dataTransferGenerator = dataTransferGenerator;
        _reportGenerator = reportGenerator;
    }

    /// <inheritdoc/>
    public async Task<byte[]> ExportDataPdfAsync(ExportRequestDto request)
    {
        var manifest = await BuildManifest(request);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var jsonContent = JsonSerializer.Serialize(manifest, options);

        return _dataTransferGenerator.Generate(jsonContent, request.OwnerName, manifest.ExportDate);
    }

    /// <inheritdoc/>
    public async Task<byte[]> ExportVisualPdfAsync(ExportRequestDto request)
    {
        var manifest = await BuildManifest(request);

        return _reportGenerator.Generate(manifest);
    }

    private async Task<ExportManifest> BuildManifest(ExportRequestDto request)
    {
        var user = await _userRepository.GetByIdAsync(request.OwnerId);

        var manifest = new ExportManifest
        {
            OwnerId = request.OwnerId,
            OwnerName = user?.FullName ?? request.OwnerName,
            ExportDate = DateTime.UtcNow,
            Scope = request.Scope
        };

        var scope = (ImportScope)request.Scope;

        if (scope == ImportScope.TaskOnly || scope == ImportScope.FullSystem)
        {
            var tasks = await _taskRepository.GetByOwnerAsync(request.OwnerId);
            manifest.Tasks = tasks.Select(t => new TaskDto
            {
                Id = t.Id,
                OwnerId = t.OwnerId,
                Title = t.Title,
                Description = t.Description,
                DueDate = t.DueDate,
                Status = (int)t.Status,
                StatusName = t.Status.ToString(),
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            }).ToList();

            foreach (var task in tasks)
            {
                var logs = await _pendingLogRepository.GetByTaskIdAsync(task.Id);
                manifest.PendingLogs.AddRange(logs.Select(pl => new PendingLogDto
                {
                    Id = pl.Id,
                    TaskId = pl.TaskId,
                    OwnerId = pl.OwnerId,
                    Reason = pl.Reason,
                    CounterpartyName = pl.CounterpartyName,
                    CreatedAt = pl.CreatedAt,
                    ResolvedAt = pl.ResolvedAt,
                    OwnerName = pl.Owner?.FullName ?? string.Empty
                }));
            }
        }

        if (scope == ImportScope.NoteOnly || scope == ImportScope.FullSystem)
        {
            var notes = await _noteRepository.GetByOwnerAsync(request.OwnerId);
            manifest.Notes = notes.Select(n => new NoteDto
            {
                Id = n.Id,
                NoteNumber = n.NoteNumber,
                OwnerId = n.OwnerId,
                Title = n.Title,
                Content = n.Content,
                Status = (int)n.Status,
                StatusName = n.Status.ToString(),
                CreatedAt = n.CreatedAt,
                UpdatedAt = n.UpdatedAt
            }).ToList();

            foreach (var note in notes)
            {
                var attachments = await _attachmentRepository.GetByNoteIdAsync(note.Id);
                manifest.Attachments.AddRange(attachments.Select(a => new AttachmentDto
                {
                    Id = a.Id,
                    NoteId = a.NoteId,
                    FileName = a.FileName,
                    FileType = a.FileType,
                    FileSize = a.FileSize,
                    FileSizeFormatted = FormatFileSize(a.FileSize),
                    UploadedAt = a.UploadedAt
                }));
            }
        }

        return manifest;
    }

    private static string FormatFileSize(long? bytes)
    {
        if (!bytes.HasValue) return "0 B";
        if (bytes.Value < 1024) return $"{bytes.Value} B";
        if (bytes.Value < 1024 * 1024) return $"{bytes.Value / 1024.0:F1} KB";
        return $"{bytes.Value / (1024.0 * 1024):F1} MB";
    }
}
