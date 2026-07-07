using System.Globalization;
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

    public async Task<List<MonthDto>> GetAvailableMonthsAsync(int ownerId, int scope)
    {
        var months = new HashSet<(int Year, int Month)>();
        var importScope = (ImportScope)scope;

        if (importScope == ImportScope.TaskOnly || importScope == ImportScope.FullSystem)
        {
            var tasks = await _taskRepository.GetByOwnerAsync(ownerId);
            foreach (var task in tasks)
                months.Add((task.CreatedAt.Year, task.CreatedAt.Month));
        }

        if (importScope == ImportScope.NoteOnly || importScope == ImportScope.FullSystem)
        {
            var notes = await _noteRepository.GetByOwnerAsync(ownerId);
            foreach (var note in notes)
                months.Add((note.CreatedAt.Year, note.CreatedAt.Month));
        }

        var cultura = new CultureInfo("pt-BR");
        return months
            .Select(m => new MonthDto
            {
                Year = m.Year,
                Month = m.Month,
                Name = $"{cultura.TextInfo.ToTitleCase(cultura.DateTimeFormat.GetMonthName(m.Month))} {m.Year}"
            })
            .OrderByDescending(m => m.Year)
            .ThenByDescending(m => m.Month)
            .ToList();
    }

    private static bool MatchesSelectedMonth(DateTime createdAt, List<MonthDto> selectedMonths)
    {
        return selectedMonths.Any(m => m.Year == createdAt.Year && m.Month == createdAt.Month);
    }

    private async Task<ExportManifest> BuildManifest(ExportRequestDto request)
    {
        var user = await _userRepository.GetByIdAsync(request.OwnerId);

        var manifest = new ExportManifest
        {
            OwnerId = request.OwnerId,
            OwnerName = user?.FullName ?? request.OwnerName,
            ExportDate = DateTime.UtcNow,
            Scope = request.Scope,
            MigrationId = $"TF-{DateTime.UtcNow:yyyyMMddHHmmss}",
            ExportVersion = "2.0"
        };

        var scope = (ImportScope)request.Scope;

        if (scope == ImportScope.TaskOnly || scope == ImportScope.FullSystem)
        {
            var tasks = await _taskRepository.GetByOwnerAsync(request.OwnerId);

            if (request.FilterByMonths && request.SelectedMonths.Count > 0)
                tasks = tasks.Where(t => MatchesSelectedMonth(t.CreatedAt, request.SelectedMonths)).ToList();

            var taskDtos = new List<TaskDto>();

            foreach (var task in tasks)
            {
                var logs = await _pendingLogRepository.GetByTaskIdAsync(task.Id);
                var logDtos = logs.Select(pl => new PendingLogDto
                {
                    Id = pl.Id,
                    TaskId = pl.TaskId,
                    OwnerId = pl.OwnerId,
                    Reason = pl.Reason,
                    CounterpartyName = pl.CounterpartyName,
                    MySignature = pl.MySignature,
                    CounterpartySignature = pl.CounterpartySignature,
                    ResolutionNote = pl.ResolutionNote,
                    CreatedAt = pl.CreatedAt,
                    ResolvedAt = pl.ResolvedAt,
                    OwnerName = pl.Owner?.FullName ?? string.Empty
                }).ToList();

                taskDtos.Add(new TaskDto
                {
                    Id = task.Id,
                    OwnerId = task.OwnerId,
                    Title = task.Title,
                    Description = task.Description,
                    DueDate = task.DueDate,
                    Status = (int)task.Status,
                    StatusName = task.Status.ToString(),
                    CreatedAt = task.CreatedAt,
                    UpdatedAt = task.UpdatedAt,
                    PendingLogs = logDtos
                });

                manifest.PendingLogs.AddRange(logDtos);
            }

            manifest.Tasks = taskDtos;
        }

        if (scope == ImportScope.NoteOnly || scope == ImportScope.FullSystem)
        {
            var notes = await _noteRepository.GetByOwnerAsync(request.OwnerId);

            if (request.FilterByMonths && request.SelectedMonths.Count > 0)
                notes = notes.Where(n => MatchesSelectedMonth(n.CreatedAt, request.SelectedMonths)).ToList();

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

        manifest.TotalTasks = manifest.Tasks.Count;
        manifest.TotalNotes = manifest.Notes.Count;
        manifest.TotalAttachments = manifest.Attachments.Count;
        manifest.TotalPendingLogs = manifest.PendingLogs.Count;
        manifest.ActivePendingLogs = manifest.PendingLogs.Count(pl => pl.IsActive);
        manifest.ScopeName = scope.ToString();

        if (request.FilterByMonths && request.SelectedMonths.Count > 0)
            manifest.FilterDescription = string.Join(", ", request.SelectedMonths.Select(m => m.Name));

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
