using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for Attachment entity using Entity Framework Core.
/// </summary>
public class AttachmentRepository : IAttachmentRepository
{
    private readonly AppDbContext _context;

    public AttachmentRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<Attachment?> GetByIdAsync(int id)
    {
        return await _context.Attachments.FindAsync(id);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Attachment>> GetByNoteIdAsync(int noteId)
    {
        return await _context.Attachments
            .Where(a => a.NoteId == noteId)
            .OrderByDescending(a => a.UploadedAt)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task AddAsync(Attachment attachment)
    {
        await _context.Attachments.AddAsync(attachment);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id)
    {
        var attachment = await _context.Attachments.FindAsync(id);
        if (attachment is not null)
        {
            _context.Attachments.Remove(attachment);
            await _context.SaveChangesAsync();
        }
    }
}
