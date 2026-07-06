namespace TaskFlow.Application.DTOs;

/// <summary>
/// DTO para exibição de logs de pendência.
/// </summary>
public class PendingLogDto
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public int OwnerId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? CounterpartyName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string OwnerName { get; set; } = string.Empty;

    public string? MySignature { get; set; }

    public string? CounterpartySignature { get; set; }

    public bool IsActive => ResolvedAt is null;
}
