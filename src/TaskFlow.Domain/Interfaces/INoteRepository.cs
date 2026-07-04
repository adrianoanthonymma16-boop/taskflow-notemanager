using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Domain.Interfaces;

/// <summary>
/// Contrato para operações de persistência da entidade Note.
/// </summary>
public interface INoteRepository
{
    /// <summary> Obtém uma nota pelo ID, incluindo anexos. </summary>
    Task<Note?> GetByIdAsync(int id);

    /// <summary> Lista todas as notas de um usuário com filtros opcionais. </summary>
    Task<IEnumerable<Note>> GetByOwnerAsync(int ownerId, NoteStatus? status = null, DateTime? startDate = null, DateTime? endDate = null);

    /// <summary> Adiciona uma nova nota. </summary>
    Task AddAsync(Note note);

    /// <summary> Atualiza uma nota existente. </summary>
    Task UpdateAsync(Note note);

    /// <summary> Remove uma nota pelo ID. </summary>
    Task DeleteAsync(int id);

    /// <summary> Obtém o maior número de nota do usuário. </summary>
    Task<int> GetMaxNoteNumberAsync(int ownerId);
}
