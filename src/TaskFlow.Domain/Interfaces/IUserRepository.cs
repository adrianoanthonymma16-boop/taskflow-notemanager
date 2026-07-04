using TaskFlow.Domain.Entities;

namespace TaskFlow.Domain.Interfaces;

/// <summary>
/// Contrato para operações de persistência da entidade User.
/// </summary>
public interface IUserRepository
{
    /// <summary> Obtém um usuário pelo ID. </summary>
    Task<User?> GetByIdAsync(int id);

    /// <summary> Obtém um usuário pelo nome de usuário (login). </summary>
    Task<User?> GetByUsernameAsync(string username);

    /// <summary> Adiciona um novo usuário ao banco. </summary>
    Task AddAsync(User user);

    /// <summary> Atualiza os dados de um usuário existente. </summary>
    Task UpdateAsync(User user);

    /// <summary> Verifica se um nome de usuário já está em uso. </summary>
    Task<bool> UsernameExistsAsync(string username);
}
