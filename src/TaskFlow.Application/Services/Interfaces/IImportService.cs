using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Services.Interfaces;

/// <summary>
/// Contrato para o serviço de importação de dados a partir de PDF.
/// </summary>
public interface IImportService
{
    /// <summary> Importa dados de um PDF (Tipo 1) realizando a fusão com os dados existentes. </summary>
    Task<ImportResultDto> ImportDataPdfAsync(int loggedUserId, ImportRequestDto request);

    /// <summary> Importa um PDF como registro visual (Modo B) sem alterar os dados. </summary>
    Task VisualImportAsync(int loggedUserId, byte[] fileBytes, string fileName);
}
