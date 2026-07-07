namespace TaskFlow.Application.DTOs;

public class ExportManifest
{
    public int OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public DateTime ExportDate { get; set; }
    public int Scope { get; set; }
    public string ScopeName { get; set; } = string.Empty;
    public string MigrationId { get; set; } = string.Empty;
    public string ExportVersion { get; set; } = "1.0";
    public string FilterDescription { get; set; } = string.Empty;
    public List<TaskDto> Tasks { get; set; } = new();
    public List<PendingLogDto> PendingLogs { get; set; } = new();
    public List<NoteDto> Notes { get; set; } = new();
    public List<AttachmentDto> Attachments { get; set; } = new();
    public int TotalTasks { get; set; }
    public int TotalNotes { get; set; }
    public int TotalAttachments { get; set; }
    public int TotalPendingLogs { get; set; }
    public int ActivePendingLogs { get; set; }
}
