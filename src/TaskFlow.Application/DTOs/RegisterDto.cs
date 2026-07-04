using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Application.DTOs;

/// <summary>
/// DTO para a requisição de criação de conta.
/// </summary>
public class RegisterDto
{
    [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
    [MinLength(3, ErrorMessage = "O nome de usuário deve ter no mínimo 3 caracteres.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nome completo é obrigatório.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "A confirmação de senha é obrigatória.")]
    [Compare("Password", ErrorMessage = "As senhas não conferem.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "A pergunta de segurança é obrigatória.")]
    public string SecurityQuestion { get; set; } = string.Empty;

    [Required(ErrorMessage = "A resposta de segurança é obrigatória.")]
    public string SecurityAnswer { get; set; } = string.Empty;
}
