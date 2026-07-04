using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Application.DTOs;

/// <summary>
/// DTO para atualização de uma tarefa existente.
/// </summary>
public class UpdateTaskDto
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
}
