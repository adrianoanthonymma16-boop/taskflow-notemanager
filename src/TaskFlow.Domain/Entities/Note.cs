using TaskFlow.Domain.Enums;

namespace TaskFlow.Domain.Entities;

/// <summary>
/// Representa uma nota interna do usuário, com conteúdo textual e anexos.
/// </summary>
public class Note
{
    /// <summary> Identificador único da nota. </summary>
    public int Id { get; set; }

    /// <summary> Número sequencial da nota por usuário. </summary>
    public int NoteNumber { get; set; }

    /// <summary> ID do usuário proprietário da nota. </summary>
    public int OwnerId { get; set; }

    /// <summary> Título da nota. </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary> Conteúdo textual da nota. </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary> Status da nota (Draft, Published, Archived). </summary>
    public NoteStatus Status { get; set; } = NoteStatus.Draft;

    /// <summary> Data de criação da nota. </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary> Data da última atualização da nota. </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary> Usuário proprietário da nota. </summary>
    public User Owner { get; set; } = null!;

    /// <summary> Anexos vinculados a esta nota. </summary>
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}
