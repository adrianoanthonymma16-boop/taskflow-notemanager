using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Services.Interfaces;

/// <summary>
/// Contrato para o serviço de autenticação e gerenciamento de identidade.
/// </summary>
public interface IAuthService
{
    /// <summary> Autentica um usuário com credenciais e retorna os dados do perfil. </summary>
    Task<UserProfileDto?> LoginAsync(LoginDto loginDto);

    /// <summary> Cria uma nova conta de usuário. </summary>
    Task<UserProfileDto> RegisterAsync(RegisterDto registerDto);

    /// <summary> Obtém a pergunta de segurança de um usuário para recuperação de senha. </summary>
    Task<string?> GetSecurityQuestionAsync(string username);

    /// <summary> Recupera a senha do usuário validando a resposta de segurança. </summary>
    Task<bool> RecoverPasswordAsync(RecoverPasswordDto recoverDto);

    /// <summary> Altera a senha do usuário logado. </summary>
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changeDto);

    /// <summary> Obtém o perfil do usuário pelo ID. </summary>
    Task<UserProfileDto?> GetProfileAsync(int userId);

    /// <summary> Atualiza o perfil do usuário. </summary>
    Task<UserProfileDto?> UpdateProfileAsync(int userId, UpdateProfileDto updateDto);
}
