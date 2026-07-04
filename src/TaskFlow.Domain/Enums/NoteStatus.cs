namespace TaskFlow.Domain.Enums;

/// <summary>
/// Representa o status de publicação de uma nota.
/// </summary>
public enum NoteStatus
{
    /// <summary> Nota em rascunho, ainda não publicada. </summary>
    Draft = 0,

    /// <summary> Nota publicada e visível. </summary>
    Published = 1,

    /// <summary> Nota arquivada. </summary>
    Archived = 2
}
