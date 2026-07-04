using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Application.DTOs;

/// <summary>
/// DTO para a requisição de login do usuário.
/// </summary>
public class LoginDto
{
    [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    public string Password { get; set; } = string.Empty;
}
