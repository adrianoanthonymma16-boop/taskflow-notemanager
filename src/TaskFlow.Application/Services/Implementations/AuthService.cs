using TaskFlow.Application.DTOs;
using TaskFlow.Application.Services.Interfaces;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Services.Implementations;

/// <summary>
/// Implementação do serviço de autenticação e gerenciamento de identidade.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <inheritdoc/>
    public async Task<UserProfileDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
        if (user is null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            return null;

        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        return MapToProfile(user);
    }

    /// <inheritdoc/>
    public async Task<UserProfileDto> RegisterAsync(RegisterDto registerDto)
    {
        if (await _userRepository.UsernameExistsAsync(registerDto.Username))
            throw new InvalidOperationException("Este nome de usuário já está em uso.");

        var user = new User
        {
            Username = registerDto.Username.Trim(),
            FullName = registerDto.FullName.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            SecurityQuestion = registerDto.SecurityQuestion.Trim(),
            SecurityAnswerHash = BCrypt.Net.BCrypt.HashPassword(registerDto.SecurityAnswer.Trim().ToLowerInvariant()),
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        return MapToProfile(user);
    }

    /// <inheritdoc/>
    public async Task<string?> GetSecurityQuestionAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        return user?.SecurityQuestion;
    }

    /// <inheritdoc/>
    public async Task<bool> RecoverPasswordAsync(RecoverPasswordDto recoverDto)
    {
        var user = await _userRepository.GetByUsernameAsync(recoverDto.Username);
        if (user is null)
            return false;

        if (!BCrypt.Net.BCrypt.Verify(recoverDto.SecurityAnswer.Trim().ToLowerInvariant(), user.SecurityAnswerHash))
            return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(recoverDto.NewPassword);
        await _userRepository.UpdateAsync(user);
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changeDto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return false;

        if (!BCrypt.Net.BCrypt.Verify(changeDto.CurrentPassword, user.PasswordHash))
            return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changeDto.NewPassword);
        await _userRepository.UpdateAsync(user);
        return true;
    }

    /// <inheritdoc/>
    public async Task<UserProfileDto?> GetProfileAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user is null ? null : MapToProfile(user);
    }

    /// <inheritdoc/>
    public async Task<UserProfileDto?> UpdateProfileAsync(int userId, UpdateProfileDto updateDto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return null;

        user.FullName = updateDto.FullName.Trim();
        user.PhotoBase64 = updateDto.PhotoBase64;
        await _userRepository.UpdateAsync(user);

        return MapToProfile(user);
    }

    private static UserProfileDto MapToProfile(User user)
    {
        return new UserProfileDto
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            PhotoBase64 = user.PhotoBase64,
            SecurityQuestion = user.SecurityQuestion,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        };
    }
}
