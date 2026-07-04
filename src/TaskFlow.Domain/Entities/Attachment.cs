namespace TaskFlow.Domain.Entities;

/// <summary>
/// Representa um arquivo anexado a uma nota (PDF, DOC, DOCX).
/// </summary>
public class Attachment
{
    /// <summary> Identificador único do anexo. </summary>
    public int Id { get; set; }

    /// <summary> ID da nota à qual este anexo está vinculado. </summary>
    public int NoteId { get; set; }

    /// <summary> ID do usuário proprietário do anexo. </summary>
    public int OwnerId { get; set; }

    /// <summary> Nome original do arquivo. </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary> Caminho físico do arquivo no sistema de arquivos. </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary> Tipo MIME do arquivo (ex: application/pdf). </summary>
    public string FileType { get; set; } = string.Empty;

    /// <summary> Tamanho do arquivo em bytes. </summary>
    public long? FileSize { get; set; }

    /// <summary> Data de upload do anexo. </summary>
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    /// <summary> Nota vinculada a este anexo. </summary>
    public Note Note { get; set; } = null!;

    /// <summary> Usuário proprietário do anexo. </summary>
    public User Owner { get; set; } = null!;
}
