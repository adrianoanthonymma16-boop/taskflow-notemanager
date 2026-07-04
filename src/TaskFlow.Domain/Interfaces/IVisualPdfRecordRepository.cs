using TaskFlow.Domain.Entities;

namespace TaskFlow.Domain.Interfaces;

/// <summary>
/// Contrato para operações de persistência da entidade VisualPdfRecord.
/// </summary>
public interface IVisualPdfRecordRepository
{
    /// <summary> Obtém um registro de PDF visual pelo ID. </summary>
    Task<VisualPdfRecord?> GetByIdAsync(int id);

    /// <summary> Lista todos os registros de PDF visual de um usuário. </summary>
    Task<IEnumerable<VisualPdfRecord>> GetByOwnerAsync(int ownerId);

    /// <summary> Adiciona um novo registro de PDF visual. </summary>
    Task AddAsync(VisualPdfRecord record);

    /// <summary> Remove um registro de PDF visual pelo ID. </summary>
    Task DeleteAsync(int id);
}
