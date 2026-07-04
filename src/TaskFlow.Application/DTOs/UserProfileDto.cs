namespace TaskFlow.Application.DTOs;

/// <summary>
/// DTO para exibição e edição do perfil do usuário.
/// </summary>
public class UserProfileDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhotoBase64 { get; set; }
    public string SecurityQuestion { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
