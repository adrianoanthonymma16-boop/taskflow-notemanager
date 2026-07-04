namespace TaskFlow.Application.DTOs;

/// <summary>
/// DTO com o resultado de uma operação de importação de dados.
/// </summary>
public class ImportResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int TasksImported { get; set; }
    public int NotesImported { get; set; }
    public int AttachmentsImported { get; set; }
}
