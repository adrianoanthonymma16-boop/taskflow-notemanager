using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for TaskItem entity using Entity Framework Core.
/// </summary>
public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<TaskItem?> GetByIdAsync(int id)
    {
        return await _context.Tasks
            .Include(t => t.PendingLogs)
                .ThenInclude(pl => pl.Owner)
            .Include(t => t.Owner)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TaskItem>> GetByOwnerAsync(int ownerId, Domain.Enums.TaskStatus? status = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Tasks
            .Include(t => t.PendingLogs)
                .ThenInclude(pl => pl.Owner)
            .Include(t => t.Owner)
            .Where(t => t.OwnerId == ownerId);

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (startDate.HasValue)
            query = query.Where(t => t.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.CreatedAt <= endDate.Value.AddDays(1));

        return await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
    }

    /// <inheritdoc/>
    public async Task AddAsync(TaskItem task)
    {
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(TaskItem task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task is not null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }

    /// <inheritdoc/>
    public async Task<bool> HasActivePendingLogAsync(int taskId)
    {
        return await _context.PendingLogs
            .AnyAsync(pl => pl.TaskId == taskId && pl.ResolvedAt == null);
    }
}
