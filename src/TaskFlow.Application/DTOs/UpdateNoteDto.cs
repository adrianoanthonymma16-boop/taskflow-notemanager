using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Application.DTOs;

/// <summary>
/// DTO para atualização de uma nota existente.
/// </summary>
public class UpdateNoteDto
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Status { get; set; }
}
