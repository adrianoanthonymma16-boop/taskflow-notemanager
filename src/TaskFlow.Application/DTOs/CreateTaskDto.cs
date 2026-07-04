using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Application.DTOs;

/// <summary>
/// DTO para criação de uma nova tarefa.
/// </summary>
public class CreateTaskDto
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
}
