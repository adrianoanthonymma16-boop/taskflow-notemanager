using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Application.DTOs;

/// <summary>
/// DTO para criação de uma nova nota.
/// </summary>
public class CreateNoteDto
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Status { get; set; }
}
