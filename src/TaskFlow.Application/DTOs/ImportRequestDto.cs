namespace TaskFlow.Application.DTOs;

/// <summary>
/// DTO para requisição de importação de dados a partir de PDF.
/// </summary>
public class ImportRequestDto
{
    public byte[] FileBytes { get; set; } = Array.Empty<byte>();
    public string FileName { get; set; } = string.Empty;
    public int ConflictStrategy { get; set; }
    public bool FilterByDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int OwnershipAction { get; set; }
}
