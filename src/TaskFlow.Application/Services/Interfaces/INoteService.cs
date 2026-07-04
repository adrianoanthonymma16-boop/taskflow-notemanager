using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Services.Interfaces;

/// <summary>
/// Contrato para o serviço de gerenciamento de notas (NoteManager).
/// </summary>
public interface INoteService
{
    /// <summary> Obtém uma nota pelo ID, incluindo anexos. </summary>
    Task<NoteDto?> GetByIdAsync(int id);

    /// <summary> Lista notas do usuário com filtros opcionais. </summary>
    Task<List<NoteDto>> GetByOwnerAsync(int ownerId, int? status = null, DateTime? startDate = null, DateTime? endDate = null);

    /// <summary> Cria uma nova nota. </summary>
    Task<NoteDto> CreateAsync(int ownerId, CreateNoteDto dto);

    /// <summary> Atualiza uma nota existente. </summary>
    Task<NoteDto?> UpdateAsync(int noteId, UpdateNoteDto dto);

    /// <summary> Remove uma nota e seus anexos. </summary>
    Task<bool> DeleteAsync(int noteId);

    /// <summary> Adiciona um anexo a uma nota (arquivo físico). </summary>
    Task<AttachmentDto?> AddAttachmentAsync(int noteId, int ownerId, string fileName, string fileType, long fileSize, Stream fileStream);

    /// <summary> Remove um anexo. </summary>
    Task<bool> RemoveAttachmentAsync(int attachmentId);
}
