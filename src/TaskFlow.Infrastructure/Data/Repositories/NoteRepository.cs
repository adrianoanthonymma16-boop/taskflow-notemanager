using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for Note entity using Entity Framework Core.
/// </summary>
public class NoteRepository : INoteRepository
{
    private readonly AppDbContext _context;

    public NoteRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<Note?> GetByIdAsync(int id)
    {
        return await _context.Notes
            .Include(n => n.Attachments)
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Note>> GetByOwnerAsync(int ownerId, Domain.Enums.NoteStatus? status = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Notes
            .Include(n => n.Attachments)
            .Where(n => n.OwnerId == ownerId);

        if (status.HasValue)
            query = query.Where(n => n.Status == status.Value);

        if (startDate.HasValue)
            query = query.Where(n => n.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(n => n.CreatedAt <= endDate.Value.AddDays(1));

        return await query.OrderByDescending(n => n.CreatedAt).ToListAsync();
    }

    /// <inheritdoc/>
    public async Task AddAsync(Note note)
    {
        await _context.Notes.AddAsync(note);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(Note note)
    {
        _context.Notes.Update(note);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id)
    {
        var note = await _context.Notes.FindAsync(id);
        if (note is not null)
        {
            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
        }
    }

    /// <inheritdoc/>
    public async Task<int> GetMaxNoteNumberAsync(int ownerId)
    {
        return await _context.Notes
            .Where(n => n.OwnerId == ownerId)
            .MaxAsync(n => (int?)n.NoteNumber) ?? 0;
    }
}
