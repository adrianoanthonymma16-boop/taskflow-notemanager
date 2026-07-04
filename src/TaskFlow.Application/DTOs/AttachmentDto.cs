namespace TaskFlow.Application.DTOs;

/// <summary>
/// DTO para exibição de anexos.
/// </summary>
public class AttachmentDto
{
    public int Id { get; set; }
    public int NoteId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long? FileSize { get; set; }
    public string FileSizeFormatted { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}
