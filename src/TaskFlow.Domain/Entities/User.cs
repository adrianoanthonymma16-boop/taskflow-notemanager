namespace TaskFlow.Domain.Entities;

/// <summary>
/// Representa um usuário do sistema com credenciais de autenticação e dados de perfil.
/// </summary>
public class User
{
    /// <summary> Identificador único do usuário. </summary>
    public int Id { get; set; }

    /// <summary> Nome de usuário (login), único no sistema. </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary> Nome completo do usuário. </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary> Hash da senha gerado pelo BCrypt. </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary> Foto do perfil em Base64 (opcional). </summary>
    public string? PhotoBase64 { get; set; }

    /// <summary> Pergunta de segurança para recuperação de senha. </summary>
    public string SecurityQuestion { get; set; } = string.Empty;

    /// <summary> Hash da resposta de segurança gerado pelo BCrypt. </summary>
    public string SecurityAnswerHash { get; set; } = string.Empty;

    /// <summary> Data de criação da conta. </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary> Data do último login. </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary> Tarefas pertencentes a este usuário. </summary>
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();

    /// <summary> Logs de pendência deste usuário. </summary>
    public ICollection<PendingLog> PendingLogs { get; set; } = new List<PendingLog>();

    /// <summary> Notas pertencentes a este usuário. </summary>
    public ICollection<Note> Notes { get; set; } = new List<Note>();

    /// <summary> Registros visuais de PDF importados por este usuário. </summary>
    public ICollection<VisualPdfRecord> VisualPdfRecords { get; set; } = new List<VisualPdfRecord>();
}
