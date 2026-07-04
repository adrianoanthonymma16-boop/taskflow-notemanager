namespace TaskFlow.Application.DTOs;

/// <summary>
/// DTO para criação, edição e exibição de tarefas.
/// </summary>
public class TaskDto
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<PendingLogDto> PendingLogs { get; set; } = new();
}
