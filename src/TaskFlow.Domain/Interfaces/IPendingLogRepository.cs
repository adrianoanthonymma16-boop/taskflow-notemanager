using TaskFlow.Domain.Entities;

namespace TaskFlow.Domain.Interfaces;

/// <summary>
/// Contrato para operações de persistência da entidade PendingLog.
/// </summary>
public interface IPendingLogRepository
{
    /// <summary> Obtém um log de pendência pelo ID. </summary>
    Task<PendingLog?> GetByIdAsync(int id);

    /// <summary> Lista todos os logs de pendência de uma tarefa. </summary>
    Task<IEnumerable<PendingLog>> GetByTaskIdAsync(int taskId);

    /// <summary> Adiciona um novo log de pendência. </summary>
    Task AddAsync(PendingLog pendingLog);

    /// <summary> Atualiza um log de pendência existente (ex: marcar como resolvido). </summary>
    Task UpdateAsync(PendingLog pendingLog);
}
