namespace TaskFlow.Domain.Enums;

/// <summary>
/// Estratégia de resolução de conflitos durante a importação de dados.
/// </summary>
public enum ConflictStrategy
{
    /// <summary> Limpa os dados existentes e recria a partir da importação. </summary>
    Replace = 0,

    /// <summary> Atualiza registros existentes e insere novos. </summary>
    Merge = 1,

    /// <summary> Insere apenas registros novos, ignora os já existentes. </summary>
    Append = 2
}
