using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Data;

/// <summary>
/// Database context for TaskFlow application using SQLite.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<PendingLog> PendingLogs => Set<PendingLog>();
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<VisualPdfRecord> VisualPdfRecords => Set<VisualPdfRecord>();

    /// <summary>
    /// Configures entity relationships, indexes, and constraints using Fluent API.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
            entity.Property(u => u.FullName).IsRequired().HasMaxLength(200);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.SecurityQuestion).IsRequired().HasMaxLength(500);
            entity.Property(u => u.SecurityAnswerHash).IsRequired();
            entity.HasMany(u => u.Tasks)
                  .WithOne(t => t.Owner)
                  .HasForeignKey(t => t.OwnerId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(u => u.PendingLogs)
                  .WithOne(pl => pl.Owner)
                  .HasForeignKey(pl => pl.OwnerId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(u => u.Notes)
                  .WithOne(n => n.Owner)
                  .HasForeignKey(n => n.OwnerId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(u => u.VisualPdfRecords)
                  .WithOne(v => v.Owner)
                  .HasForeignKey(v => v.OwnerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.Property(t => t.Title).IsRequired().HasMaxLength(300);
            entity.Property(t => t.Status).HasConversion<int>();
            entity.HasMany(t => t.PendingLogs)
                  .WithOne(pl => pl.Task)
                  .HasForeignKey(pl => pl.TaskId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PendingLog>(entity =>
        {
            entity.Property(pl => pl.Reason).IsRequired().HasMaxLength(1000);
            entity.Property(pl => pl.MySignature).IsRequired();
        });

        modelBuilder.Entity<Note>(entity =>
        {
            entity.Property(n => n.Title).IsRequired().HasMaxLength(300);
            entity.Property(n => n.Status).HasConversion<int>();
            entity.HasMany(n => n.Attachments)
                  .WithOne(a => a.Note)
                  .HasForeignKey(a => a.NoteId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.Property(a => a.FileName).IsRequired().HasMaxLength(300);
            entity.Property(a => a.FilePath).IsRequired().HasMaxLength(1000);
            entity.HasOne(a => a.Owner)
                  .WithMany()
                  .HasForeignKey(a => a.OwnerId)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<VisualPdfRecord>(entity =>
        {
            entity.Property(v => v.OriginalFileName).IsRequired().HasMaxLength(300);
            entity.Property(v => v.StoredPath).IsRequired().HasMaxLength(1000);
            entity.Property(v => v.SourceScope).HasConversion<int>();
        });
    }
}
