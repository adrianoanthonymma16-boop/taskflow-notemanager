using TaskFlow.Domain.Entities;

namespace TaskFlow.Domain.Interfaces;

/// <summary>
/// Contrato para operações de persistência da entidade TaskItem.
/// </summary>
public interface ITaskRepository
{
    /// <summary> Obtém uma tarefa pelo ID, incluindo logs de pendência. </summary>
    Task<TaskItem?> GetByIdAsync(int id);

    /// <summary> Lista todas as tarefas de um usuário com filtros opcionais. </summary>
    Task<IEnumerable<TaskItem>> GetByOwnerAsync(int ownerId, Domain.Enums.TaskStatus? status = null, DateTime? startDate = null, DateTime? endDate = null);

    /// <summary> Adiciona uma nova tarefa. </summary>
    Task AddAsync(TaskItem task);

    /// <summary> Atualiza uma tarefa existente. </summary>
    Task UpdateAsync(TaskItem task);

    /// <summary> Remove uma tarefa pelo ID. </summary>
    Task DeleteAsync(int id);

    /// <summary> Verifica se existe alguma pendência ativa vinculada à tarefa. </summary>
    Task<bool> HasActivePendingLogAsync(int taskId);
}
