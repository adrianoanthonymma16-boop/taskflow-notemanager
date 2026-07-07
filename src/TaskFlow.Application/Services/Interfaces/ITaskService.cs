using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Services.Interfaces;

/// <summary>
/// Contrato para o serviço de gerenciamento de tarefas (TaskFlow).
/// </summary>
public interface ITaskService
{
    /// <summary> Obtém uma tarefa pelo ID. </summary>
    Task<TaskDto?> GetByIdAsync(int id);

    /// <summary> Lista tarefas do usuário com filtros opcionais. </summary>
    Task<List<TaskDto>> GetByOwnerAsync(int ownerId, int? status = null, DateTime? startDate = null, DateTime? endDate = null);

    /// <summary> Cria uma nova tarefa. </summary>
    Task<TaskDto> CreateAsync(int ownerId, CreateTaskDto dto);

    /// <summary> Atualiza uma tarefa existente. </summary>
    Task<TaskDto?> UpdateAsync(int taskId, UpdateTaskDto dto);

    /// <summary> Remove uma tarefa. </summary>
    Task<bool> DeleteAsync(int taskId);

    /// <summary> Marca uma tarefa como pendente, criando um PendingLog. </summary>
    Task<bool> MarkAsPendingAsync(int ownerId, CreatePendingLogDto dto);

    /// <summary> Marca uma tarefa como concluída (se não houver pendências ativas). </summary>
    Task<bool> MarkAsCompletedAsync(int taskId);

    /// <summary> Reabre uma tarefa concluída. </summary>
    Task<bool> ReopenAsync(int taskId);

    /// <summary> Remove a pendência ativa de uma tarefa, reabrindo-a. </summary>
    Task<bool> ResolvePendingAsync(int taskId, string? resolutionNote = null);

    /// <summary> Conclui a tarefa mesmo com pendências ativas, resolvendo-as automaticamente. </summary>
    Task<bool> ForceMarkAsCompletedAsync(int taskId);

    /// <summary> Resolve uma pendência específica pelo ID. Se não houver mais pendências ativas, reabre a tarefa. </summary>
    Task<bool> ResolvePendingLogAsync(int pendingLogId, string? resolutionNote = null);
}
