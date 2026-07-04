using TaskFlow.Domain.Enums;

namespace TaskFlow.Domain.Entities;

/// <summary>
/// Registro de um PDF importado para leitura visual (Modo B), sem integração com os dados principais.
/// </summary>
public class VisualPdfRecord
{
    /// <summary> Identificador único do registro. </summary>
    public int Id { get; set; }

    /// <summary> ID do usuário que importou o PDF. </summary>
    public int OwnerId { get; set; }

    /// <summary> Nome original do arquivo PDF importado. </summary>
    public string OriginalFileName { get; set; } = string.Empty;

    /// <summary> Caminho físico onde o PDF foi armazenado. </summary>
    public string StoredPath { get; set; } = string.Empty;

    /// <summary> Escopo dos dados contidos no PDF (TaskOnly, NoteOnly, FullSystem). </summary>
    public ImportScope SourceScope { get; set; }

    /// <summary> Indica se o PDF veio de um usuário externo. </summary>
    public bool IsExternal { get; set; }

    /// <summary> Descrição opcional fornecida pelo usuário. </summary>
    public string? UserDescription { get; set; }

    /// <summary> Data da importação. </summary>
    public DateTime ImportedAt { get; set; } = DateTime.UtcNow;

    /// <summary> Usuário que importou o PDF. </summary>
    public User Owner { get; set; } = null!;
}
