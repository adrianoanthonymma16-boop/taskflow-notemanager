namespace TaskFlow.Application.DTOs;

/// <summary>
/// DTO para criação, edição e exibição de notas.
/// </summary>
public class NoteDto
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<AttachmentDto> Attachments { get; set; } = new();
}
