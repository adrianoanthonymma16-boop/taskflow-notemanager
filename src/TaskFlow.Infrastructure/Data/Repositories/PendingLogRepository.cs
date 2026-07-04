using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for PendingLog entity using Entity Framework Core.
/// </summary>
public class PendingLogRepository : IPendingLogRepository
{
    private readonly AppDbContext _context;

    public PendingLogRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<PendingLog?> GetByIdAsync(int id)
    {
        return await _context.PendingLogs
            .Include(pl => pl.Owner)
            .FirstOrDefaultAsync(pl => pl.Id == id);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PendingLog>> GetByTaskIdAsync(int taskId)
    {
        return await _context.PendingLogs
            .Include(pl => pl.Owner)
            .Where(pl => pl.TaskId == taskId)
            .OrderByDescending(pl => pl.CreatedAt)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task AddAsync(PendingLog pendingLog)
    {
        await _context.PendingLogs.AddAsync(pendingLog);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(PendingLog pendingLog)
    {
        _context.PendingLogs.Update(pendingLog);
        await _context.SaveChangesAsync();
    }
}
