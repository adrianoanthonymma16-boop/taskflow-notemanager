using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Services.Interfaces;

/// <summary>
/// Contrato para geração de PDF de visualização (Tipo 2) para leitura humana.
/// </summary>
public interface IPdfReportGenerator
{
    /// <summary> Gera um PDF com layout visual a partir do manifesto de exportação. </summary>
    byte[] Generate(ExportManifest manifest);
}
