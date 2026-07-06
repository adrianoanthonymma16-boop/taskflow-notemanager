using System.Text.Json;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Services.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Services.Implementations;

/// <summary>
/// Implementação do serviço de importação de dados a partir de PDF.
/// </summary>
public class ImportService : IImportService
{
    private readonly ITaskRepository _taskRepository;
    private readonly INoteRepository _noteRepository;
    private readonly IPendingLogRepository _pendingLogRepository;
    private readonly IAttachmentRepository _attachmentRepository;
    private readonly IVisualPdfRecordRepository _visualPdfRecordRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUserRepository _userRepository;
    private readonly IPdfJsonExtractor _pdfJsonExtractor;

    public ImportService(
        ITaskRepository taskRepository,
        INoteRepository noteRepository,
        IPendingLogRepository pendingLogRepository,
        IAttachmentRepository attachmentRepository,
        IVisualPdfRecordRepository visualPdfRecordRepository,
        IFileStorageService fileStorageService,
        IUserRepository userRepository,
        IPdfJsonExtractor pdfJsonExtractor)
    {
        _taskRepository = taskRepository;
        _noteRepository = noteRepository;
        _pendingLogRepository = pendingLogRepository;
        _attachmentRepository = attachmentRepository;
        _visualPdfRecordRepository = visualPdfRecordRepository;
        _fileStorageService = fileStorageService;
        _userRepository = userRepository;
        _pdfJsonExtractor = pdfJsonExtractor;
    }

    /// <inheritdoc/>
    public async Task<ImportResultDto> ImportDataPdfAsync(int loggedUserId, ImportRequestDto request)
    {
        var result = new ImportResultDto { Success = true };

        var jsonContent = _pdfJsonExtractor.ExtractJsonFromPdf(request.FileBytes);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        ExportManifest? manifest;
        try
        {
            manifest = JsonSerializer.Deserialize<ExportManifest>(jsonContent, options);
        }
        catch (JsonException)
        {
            result.Success = false;
            result.Message = "O arquivo PDF não contém dados JSON válidos para importação.";
            return result;
        }

        if (manifest is null)
        {
            result.Success = false;
            result.Message = "Manifesto de importação vazio ou inválido.";
            return result;
        }

        var loggedUser = await _userRepository.GetByIdAsync(loggedUserId);

        if (manifest.OwnerId != loggedUserId)
        {
            if (request.OwnershipAction == 0)
            {
                result.Success = false;
                result.Message = "OWNERSHIP_CONFLICT";
                return result;
            }
            else if (request.OwnershipAction == 1)
            {
                manifest.OwnerId = loggedUserId;
            }
        }

        switch (request.OwnershipAction)
        {
            case 1:
                await ImportWithStrategy(manifest, (ConflictStrategy)request.ConflictStrategy, request.FilterByDate, request.StartDate, request.EndDate, result);
                break;
            case 2:
                await _visualPdfRecordRepository.AddAsync(new VisualPdfRecord
                {
                    OwnerId = loggedUserId,
                    OriginalFileName = request.FileName,
                    StoredPath = await _fileStorageService.CopyToVisualFolderAsync(
                        await _fileStorageService.SaveFileAsync("VisualPdfs", request.FileName, new MemoryStream(request.FileBytes), loggedUserId),
                        request.FileName),
                    SourceScope = (ImportScope)manifest.Scope,
                    IsExternal = true,
                    ImportedAt = DateTime.UtcNow
                });
                result.Message = "PDF armazenado como leitura visual.";
                break;
            default:
                result.Success = false;
                result.Message = "Importação cancelada.";
                break;
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task VisualImportAsync(int loggedUserId, byte[] fileBytes, string fileName)
    {
        var storedPath = await _fileStorageService.SaveFileAsync("VisualPdfs", fileName, new MemoryStream(fileBytes), loggedUserId);

        await _visualPdfRecordRepository.AddAsync(new VisualPdfRecord
        {
            OwnerId = loggedUserId,
            OriginalFileName = fileName,
            StoredPath = storedPath,
            SourceScope = ImportScope.FullSystem,
            IsExternal = false,
            ImportedAt = DateTime.UtcNow
        });
    }

    private async Task ImportWithStrategy(ExportManifest manifest, ConflictStrategy strategy, bool filterByDate, DateTime? startDate, DateTime? endDate, ImportResultDto result)
    {
        var tasksToImport = FilterByDate(manifest.Tasks, filterByDate, startDate, endDate);
        var notesToImport = FilterByDate(manifest.Notes, filterByDate, startDate, endDate);

        switch (strategy)
        {
            case ConflictStrategy.Replace:
                await ImportReplace(manifest.OwnerId, tasksToImport, manifest.PendingLogs, notesToImport, manifest.Attachments);
                break;
            case ConflictStrategy.Merge:
                await ImportMerge(manifest.OwnerId, tasksToImport, manifest.PendingLogs, notesToImport, manifest.Attachments);
                break;
            case ConflictStrategy.Append:
                await ImportAppend(manifest.OwnerId, tasksToImport, manifest.PendingLogs, notesToImport, manifest.Attachments);
                break;
        }

        result.TasksImported = tasksToImport.Count;
        result.NotesImported = notesToImport.Count;
        result.Message = $"Importação concluída. {result.TasksImported} tarefas e {result.NotesImported} notas importadas.";
    }

    private async Task ImportReplace(int ownerId, List<TaskDto> tasks, List<PendingLogDto> pendingLogs, List<NoteDto> notes, List<AttachmentDto> attachments)
    {
        var existingTasks = await _taskRepository.GetByOwnerAsync(ownerId);
        foreach (var t in existingTasks)
            await _taskRepository.DeleteAsync(t.Id);

        var existingNotes = await _noteRepository.GetByOwnerAsync(ownerId);
        foreach (var n in existingNotes)
            await _noteRepository.DeleteAsync(n.Id);

        await InsertTasks(ownerId, tasks, pendingLogs);
        await InsertNotes(ownerId, notes, attachments);
    }

    private async Task ImportMerge(int ownerId, List<TaskDto> tasks, List<PendingLogDto> pendingLogs, List<NoteDto> notes, List<AttachmentDto> attachments)
    {
        var existingTaskIds = (await _taskRepository.GetByOwnerAsync(ownerId)).Select(t => t.Id).ToHashSet();
        var existingNoteIds = (await _noteRepository.GetByOwnerAsync(ownerId)).Select(n => n.Id).ToHashSet();

        foreach (var dto in tasks)
        {
            if (existingTaskIds.Contains(dto.Id))
            {
                var existing = await _taskRepository.GetByIdAsync(dto.Id);
                if (existing is not null)
                {
                    existing.Title = dto.Title;
                    existing.Description = dto.Description;
                    existing.DueDate = dto.DueDate;
                    existing.Status = (Domain.Enums.TaskStatus)dto.Status;
                    existing.UpdatedAt = DateTime.UtcNow;
                    await _taskRepository.UpdateAsync(existing);
                }
            }
            else
            {
                var task = new TaskItem
                {
                    Id = dto.Id,
                    OwnerId = ownerId,
                    Title = dto.Title,
                    Description = dto.Description,
                    DueDate = dto.DueDate,
                    Status = (Domain.Enums.TaskStatus)dto.Status,
                    CreatedAt = dto.CreatedAt
                };
                await _taskRepository.AddAsync(task);
            }
        }

        foreach (var dto in notes)
        {
            if (existingNoteIds.Contains(dto.Id))
            {
                var existing = await _noteRepository.GetByIdAsync(dto.Id);
                if (existing is not null)
                {
                    existing.Title = dto.Title;
                    existing.Content = dto.Content;
                    existing.Status = (Domain.Enums.NoteStatus)dto.Status;
                    existing.UpdatedAt = DateTime.UtcNow;
                    await _noteRepository.UpdateAsync(existing);
                }
            }
            else
            {
                var note = new Note
                {
                    Id = dto.Id,
                    OwnerId = ownerId,
                    Title = dto.Title,
                    Content = dto.Content,
                    Status = (Domain.Enums.NoteStatus)dto.Status,
                    CreatedAt = dto.CreatedAt
                };
                await _noteRepository.AddAsync(note);
            }
        }
    }

    private async Task ImportAppend(int ownerId, List<TaskDto> tasks, List<PendingLogDto> pendingLogs, List<NoteDto> notes, List<AttachmentDto> attachments)
    {
        var existingTaskIds = (await _taskRepository.GetByOwnerAsync(ownerId)).Select(t => t.Id).ToHashSet();
        var existingNoteIds = (await _noteRepository.GetByOwnerAsync(ownerId)).Select(n => n.Id).ToHashSet();

        var newTasks = tasks.Where(t => !existingTaskIds.Contains(t.Id)).ToList();
        var newNotes = notes.Where(n => !existingNoteIds.Contains(n.Id)).ToList();

        await InsertTasks(ownerId, newTasks, pendingLogs);
        await InsertNotes(ownerId, newNotes, attachments);
    }

    private async Task InsertTasks(int ownerId, List<TaskDto> tasks, List<PendingLogDto> pendingLogs)
    {
        foreach (var dto in tasks)
        {
            var task = new TaskItem
            {
                Id = dto.Id,
                OwnerId = ownerId,
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                Status = (Domain.Enums.TaskStatus)dto.Status,
                CreatedAt = dto.CreatedAt
            };
            await _taskRepository.AddAsync(task);

            var taskLogs = pendingLogs.Where(pl => pl.TaskId == dto.Id);
            foreach (var logDto in taskLogs)
            {
                await _pendingLogRepository.AddAsync(new PendingLog
                {
                    TaskId = logDto.TaskId,
                    OwnerId = ownerId,
                    Reason = logDto.Reason,
                    CounterpartyName = logDto.CounterpartyName,
                    CreatedAt = logDto.CreatedAt,
                    ResolvedAt = logDto.ResolvedAt
                });
            }
        }
    }

    private async Task InsertNotes(int ownerId, List<NoteDto> notes, List<AttachmentDto> attachments)
    {
        foreach (var dto in notes)
        {
            var note = new Note
            {
                Id = dto.Id,
                OwnerId = ownerId,
                Title = dto.Title,
                Content = dto.Content,
                Status = (Domain.Enums.NoteStatus)dto.Status,
                CreatedAt = dto.CreatedAt
            };
            await _noteRepository.AddAsync(note);

            var noteAttachments = attachments.Where(a => a.NoteId == dto.Id);
            foreach (var att in noteAttachments)
            {
                await _attachmentRepository.AddAsync(new Attachment
                {
                    NoteId = att.NoteId,
                    OwnerId = ownerId,
                    FileName = att.FileName,
                    FilePath = string.Empty,
                    FileType = att.FileType,
                    FileSize = att.FileSize,
                    UploadedAt = att.UploadedAt
                });
            }
        }
    }

    private static List<T> FilterByDate<T>(List<T> items, bool filterByDate, DateTime? startDate, DateTime? endDate) where T : class
    {
        if (!filterByDate || (!startDate.HasValue && !endDate.HasValue))
            return items;

        var createdAtProp = typeof(T).GetProperty("CreatedAt");
        if (createdAtProp is null) return items;

        return items.Where(item =>
        {
            var date = (DateTime?)createdAtProp.GetValue(item);
            if (date is null) return false;
            if (startDate.HasValue && date.Value < startDate.Value) return false;
            if (endDate.HasValue && date.Value > endDate.Value.AddDays(1)) return false;
            return true;
        }).ToList();
    }
}
