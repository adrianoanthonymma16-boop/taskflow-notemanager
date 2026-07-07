using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Services.Interfaces;

/// <summary>
/// Contrato para o serviço de exportação de dados para PDF.
/// </summary>
public interface IExportService
{
    /// <summary> Gera um PDF de dados (Tipo 1) com JSON anexado. </summary>
    Task<byte[]> ExportDataPdfAsync(ExportRequestDto request);

    /// <summary> Gera um PDF de visualização (Tipo 2) para impressão/leitura humana. </summary>
    Task<byte[]> ExportVisualPdfAsync(ExportRequestDto request);

    /// <summary> Retorna os meses com dados disponíveis para exportação no escopo informado. </summary>
    Task<List<MonthDto>> GetAvailableMonthsAsync(int ownerId, int scope);
}
