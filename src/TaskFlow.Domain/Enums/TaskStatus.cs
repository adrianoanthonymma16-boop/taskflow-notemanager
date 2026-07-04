namespace TaskFlow.Domain.Enums;

/// <summary>
/// Representa o status de uma tarefa no fluxo de trabalho.
/// </summary>
public enum TaskStatus
{
    /// <summary> Tarefa aberta e em andamento. </summary>
    Open = 0,

    /// <summary> Tarefa bloqueada por pendência. </summary>
    Pending = 1,

    /// <summary> Tarefa concluída. </summary>
    Completed = 2
}
