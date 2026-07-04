using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Application.DTOs;

/// <summary>
/// DTO para o fluxo de recuperação de senha.
/// </summary>
public class RecoverPasswordDto
{
    [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "A resposta de segurança é obrigatória.")]
    public string SecurityAnswer { get; set; } = string.Empty;

    [Required(ErrorMessage = "A nova senha é obrigatória.")]
    [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "A confirmação da nova senha é obrigatória.")]
    [Compare("NewPassword", ErrorMessage = "As senhas não conferem.")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
