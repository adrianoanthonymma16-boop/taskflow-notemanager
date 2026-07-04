namespace TaskFlow.Domain.Enums;

/// <summary>
/// Define o escopo de dados abrangido por uma operação de exportação ou importação.
/// </summary>
public enum ImportScope
{
    /// <summary> Apenas tarefas e logs de pendência. </summary>
    TaskOnly = 1,

    /// <summary> Apenas notas e anexos. </summary>
    NoteOnly = 2,

    /// <summary> Sistema completo (tarefas, notas, anexos). </summary>
    FullSystem = 3
}
