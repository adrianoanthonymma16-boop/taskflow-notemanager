using TaskFlow.Domain.Entities;

namespace TaskFlow.Domain.Interfaces;

/// <summary>
/// Contrato para operações de persistência da entidade Attachment.
/// </summary>
public interface IAttachmentRepository
{
    /// <summary> Obtém um anexo pelo ID. </summary>
    Task<Attachment?> GetByIdAsync(int id);

    /// <summary> Lista todos os anexos de uma nota. </summary>
    Task<IEnumerable<Attachment>> GetByNoteIdAsync(int noteId);

    /// <summary> Adiciona um novo anexo. </summary>
    Task AddAsync(Attachment attachment);

    /// <summary> Remove um anexo pelo ID. </summary>
    Task DeleteAsync(int id);
}
