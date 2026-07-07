using TaskFlow.Application.DTOs;
using TaskFlow.Application.Services.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Services.Implementations;

/// <summary>
/// Implementação do serviço de gerenciamento de tarefas (TaskFlow).
/// </summary>
public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IPendingLogRepository _pendingLogRepository;

    public TaskService(ITaskRepository taskRepository, IPendingLogRepository pendingLogRepository)
    {
        _taskRepository = taskRepository;
        _pendingLogRepository = pendingLogRepository;
    }

    /// <inheritdoc/>
    public async Task<TaskDto?> GetByIdAsync(int id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        return task is null ? null : MapToDto(task);
    }

    /// <inheritdoc/>
    public async Task<List<TaskDto>> GetByOwnerAsync(int ownerId, int? status = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        Domain.Enums.TaskStatus? statusEnum = status.HasValue ? (Domain.Enums.TaskStatus)status.Value : null;
        var tasks = await _taskRepository.GetByOwnerAsync(ownerId, statusEnum, startDate, endDate);
        return tasks.Select(MapToDto).ToList();
    }

    /// <inheritdoc/>
    public async Task<TaskDto> CreateAsync(int ownerId, CreateTaskDto dto)
    {
        var task = new TaskItem
        {
            OwnerId = ownerId,
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            DueDate = dto.DueDate,
            Status = Domain.Enums.TaskStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        await _taskRepository.AddAsync(task);
        return MapToDto(task);
    }

    /// <inheritdoc/>
    public async Task<TaskDto?> UpdateAsync(int taskId, UpdateTaskDto dto)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task is null) return null;

        task.Title = dto.Title.Trim();
        task.Description = dto.Description?.Trim();
        task.DueDate = dto.DueDate;
        task.UpdatedAt = DateTime.UtcNow;

        await _taskRepository.UpdateAsync(task);
        return MapToDto(task);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int taskId)
    {
        await _taskRepository.DeleteAsync(taskId);
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> MarkAsPendingAsync(int ownerId, CreatePendingLogDto dto)
    {
        var task = await _taskRepository.GetByIdAsync(dto.TaskId);
        if (task is null) return false;

        var pendingLog = new PendingLog
        {
            TaskId = dto.TaskId,
            OwnerId = ownerId,
            Reason = dto.Reason.Trim(),
            CounterpartyName = dto.CounterpartyName?.Trim(),
            MySignature = dto.MySignature,
            CounterpartySignature = dto.CounterpartySignature,
            CreatedAt = DateTime.UtcNow
        };

        await _pendingLogRepository.AddAsync(pendingLog);

        task.Status = Domain.Enums.TaskStatus.Pending;
        task.UpdatedAt = DateTime.UtcNow;
        await _taskRepository.UpdateAsync(task);

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> MarkAsCompletedAsync(int taskId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task is null) return false;

        var hasActivePending = await _taskRepository.HasActivePendingLogAsync(taskId);
        if (hasActivePending)
            throw new InvalidOperationException("Não é possível concluir uma tarefa com pendências ativas.");

        task.Status = Domain.Enums.TaskStatus.Completed;
        task.UpdatedAt = DateTime.UtcNow;
        await _taskRepository.UpdateAsync(task);

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> ReopenAsync(int taskId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task is null) return false;

        task.Status = Domain.Enums.TaskStatus.Open;
        task.UpdatedAt = DateTime.UtcNow;
        await _taskRepository.UpdateAsync(task);

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> ResolvePendingAsync(int taskId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task is null) return false;

        var activeLog = task.PendingLogs.FirstOrDefault(pl => pl.ResolvedAt == null);
        if (activeLog is null) return false;

        activeLog.ResolvedAt = DateTime.UtcNow;
        await _pendingLogRepository.UpdateAsync(activeLog);

        task.Status = Domain.Enums.TaskStatus.Open;
        task.UpdatedAt = DateTime.UtcNow;
        await _taskRepository.UpdateAsync(task);

        return true;
    }

    public async Task<bool> ForceMarkAsCompletedAsync(int taskId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task is null) return false;

        foreach (var log in task.PendingLogs.Where(pl => pl.ResolvedAt == null))
        {
            log.ResolvedAt = DateTime.UtcNow;
            await _pendingLogRepository.UpdateAsync(log);
        }

        task.Status = Domain.Enums.TaskStatus.Completed;
        task.UpdatedAt = DateTime.UtcNow;
        await _taskRepository.UpdateAsync(task);

        return true;
    }

    private static TaskDto MapToDto(TaskItem task)
    {
        return new TaskDto
        {
            Id = task.Id,
            OwnerId = task.OwnerId,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            Status = (int)task.Status,
            StatusName = task.Status.ToString(),
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt,
            PendingLogs = task.PendingLogs.Select(pl => new PendingLogDto
            {
                Id = pl.Id,
                TaskId = pl.TaskId,
                OwnerId = pl.OwnerId,
                Reason = pl.Reason,
                CounterpartyName = pl.CounterpartyName,
                MySignature = pl.MySignature,
                CounterpartySignature = pl.CounterpartySignature,
                CreatedAt = pl.CreatedAt,
                ResolvedAt = pl.ResolvedAt,
                OwnerName = pl.Owner?.FullName ?? string.Empty
            }).ToList()
        };
    }
}
