namespace TaskFlow.Application.DTOs;

/// <summary>
/// DTO para requisição de exportação de PDF.
/// </summary>
public class ExportRequestDto
{
    public int Scope { get; set; }
    public bool IsVisualExport { get; set; }
    public int OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
}
