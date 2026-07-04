using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Services.Interfaces;

/// <summary>
/// Contrato para geração de PDF de transferência de dados (Tipo 1) com JSON anexado.
/// </summary>
public interface IPdfDataTransferGenerator
{
    /// <summary> Gera um PDF contendo o JSON de dados como anexo. </summary>
    byte[] Generate(string jsonContent, string ownerName, DateTime exportDate);
}
