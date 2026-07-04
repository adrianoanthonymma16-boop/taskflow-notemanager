namespace TaskFlow.Domain.Entities;

/// <summary>
/// Representa uma tarefa no sistema, com suporte a assinatura digital e controle de pendências.
/// </summary>
public class TaskItem
{
    /// <summary> Identificador único da tarefa. </summary>
    public int Id { get; set; }

    /// <summary> ID do usuário proprietário da tarefa. </summary>
    public int OwnerId { get; set; }

    /// <summary> Título da tarefa. </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary> Descrição detalhada da tarefa. </summary>
    public string? Description { get; set; }

    /// <summary> Data limite para conclusão. </summary>
    public DateTime? DueDate { get; set; }

    /// <summary> Status atual da tarefa (Open, Pending, Completed). </summary>
    public Domain.Enums.TaskStatus Status { get; set; } = Domain.Enums.TaskStatus.Open;

    /// <summary> Indica se a tarefa exige assinatura do emissor. </summary>
    public bool RequiresSignature { get; set; }

    /// <summary> Nome do emissor que assinou a tarefa. </summary>
    public string? GiverName { get; set; }

    /// <summary> Assinatura digital do emissor em Base64 (imagem PNG). </summary>
    public string? GiverSignature { get; set; }

    /// <summary> Data em que a assinatura do emissor foi registrada. </summary>
    public DateTime? SignedAt { get; set; }

    /// <summary> Data de criação da tarefa. </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary> Data da última atualização da tarefa. </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary> Usuário proprietário da tarefa. </summary>
    public User Owner { get; set; } = null!;

    /// <summary> Logs de pendência vinculados a esta tarefa. </summary>
    public ICollection<PendingLog> PendingLogs { get; set; } = new List<PendingLog>();
}
