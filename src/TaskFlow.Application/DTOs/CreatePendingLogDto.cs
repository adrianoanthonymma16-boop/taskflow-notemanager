using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Application.DTOs;

/// <summary>
/// DTO para criação de um log de pendência.
/// </summary>
public class CreatePendingLogDto
{
    [Required(ErrorMessage = "O ID da tarefa é obrigatório.")]
    public int TaskId { get; set; }

    [Required(ErrorMessage = "O motivo da pendência é obrigatório.")]
    public string Reason { get; set; } = string.Empty;

    public string? CounterpartyName { get; set; }

    public string? MySignature { get; set; }

    public string? CounterpartySignature { get; set; }
}
