using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Application.DTOs;

/// <summary>
/// DTO para alteração de senha do usuário logado.
/// </summary>
public class ChangePasswordDto
{
    [Required(ErrorMessage = "A senha atual é obrigatória.")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "A nova senha é obrigatória.")]
    [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "A confirmação da nova senha é obrigatória.")]
    [Compare("NewPassword", ErrorMessage = "As senhas não conferem.")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
