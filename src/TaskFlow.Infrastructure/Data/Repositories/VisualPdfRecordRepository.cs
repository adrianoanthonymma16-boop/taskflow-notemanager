using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for VisualPdfRecord entity using Entity Framework Core.
/// </summary>
public class VisualPdfRecordRepository : IVisualPdfRecordRepository
{
    private readonly AppDbContext _context;

    public VisualPdfRecordRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<VisualPdfRecord?> GetByIdAsync(int id)
    {
        return await _context.VisualPdfRecords.FindAsync(id);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<VisualPdfRecord>> GetByOwnerAsync(int ownerId)
    {
        return await _context.VisualPdfRecords
            .Where(v => v.OwnerId == ownerId)
            .OrderByDescending(v => v.ImportedAt)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task AddAsync(VisualPdfRecord record)
    {
        await _context.VisualPdfRecords.AddAsync(record);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id)
    {
        var record = await _context.VisualPdfRecords.FindAsync(id);
        if (record is not null)
        {
            _context.VisualPdfRecords.Remove(record);
            await _context.SaveChangesAsync();
        }
    }
}
