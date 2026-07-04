namespace TaskFlow.Application.DTOs;

/// <summary>
/// DTO para exibição de registros de PDF visual.
/// </summary>
public class VisualPdfRecordDto
{
    public int Id { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public int SourceScope { get; set; }
    public string SourceScopeName { get; set; } = string.Empty;
    public bool IsExternal { get; set; }
    public string? UserDescription { get; set; }
    public DateTime ImportedAt { get; set; }
}
