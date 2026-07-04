namespace TaskFlow.Domain.Entities;

/// <summary>
/// Registra uma pendência (bloqueio) associada a uma tarefa, com motivo e assinaturas.
/// </summary>
public class PendingLog
{
    /// <summary> Identificador único do log de pendência. </summary>
    public int Id { get; set; }

    /// <summary> ID da tarefa à qual esta pendência está vinculada. </summary>
    public int TaskId { get; set; }

    /// <summary> ID do usuário que registrou a pendência. </summary>
    public int OwnerId { get; set; }

    /// <summary> Motivo/justificativa da pendência. </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary> Assinatura digital de quem marcou a pendência (Base64 PNG). </summary>
    public string MySignature { get; set; } = string.Empty;

    /// <summary> Nome da contraparte (opcional). </summary>
    public string? CounterpartyName { get; set; }

    /// <summary> Assinatura da contraparte em Base64 (opcional). </summary>
    public string? CounterpartySignature { get; set; }

    /// <summary> Data de criação do registro de pendência. </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary> Data em que a pendência foi resolvida. </summary>
    public DateTime? ResolvedAt { get; set; }

    /// <summary> Tarefa vinculada a esta pendência. </summary>
    public TaskItem Task { get; set; } = null!;

    /// <summary> Usuário que registrou a pendência. </summary>
    public User Owner { get; set; } = null!;
}
