namespace TaskFlow.Application.DTOs;

/// <summary>
/// Manifesto de exportação serializado como JSON dentro do PDF de dados (Tipo 1).
/// </summary>
public class ExportManifest
{
    public int OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public DateTime ExportDate { get; set; }
    public int Scope { get; set; }
    public List<TaskDto> Tasks { get; set; } = new();
    public List<PendingLogDto> PendingLogs { get; set; } = new();
    public List<NoteDto> Notes { get; set; } = new();
    public List<AttachmentDto> Attachments { get; set; } = new();
}
