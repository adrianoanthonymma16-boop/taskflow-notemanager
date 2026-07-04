using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Application.DTOs;

/// <summary>
/// DTO para atualização dos dados de perfil do usuário.
/// </summary>
public class UpdateProfileDto
{
    [Required(ErrorMessage = "O nome completo é obrigatório.")]
    public string FullName { get; set; } = string.Empty;
    public string? PhotoBase64 { get; set; }
}
