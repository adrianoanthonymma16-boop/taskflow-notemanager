using TaskFlow.Application.DTOs;

namespace TaskFlow.Presentation.Components.Shared;

/// <summary>
/// Serviço de estado do usuário logado (escopo de sessão Blazor).
/// </summary>
public class UserStateService
{
    private UserProfileDto? _currentUser;

    /// <summary> Usuário atualmente logado, ou null se não autenticado. </summary>
    public UserProfileDto? CurrentUser
    {
        get => _currentUser;
        private set
        {
            _currentUser = value;
            OnStateChanged?.Invoke();
        }
    }

    /// <summary> Indica se há um usuário autenticado. </summary>
    public bool IsAuthenticated => CurrentUser is not null;

    /// <summary> Evento disparado quando o estado do usuário muda. </summary>
    public event Action? OnStateChanged;

    /// <summary> Define o usuário logado. </summary>
    public void SetUser(UserProfileDto? user)
    {
        CurrentUser = user;
    }

    /// <summary> Realiza o logout do usuário atual. </summary>
    public void Logout()
    {
        CurrentUser = null;
    }
}
