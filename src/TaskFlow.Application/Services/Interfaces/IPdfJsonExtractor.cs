namespace TaskFlow.Application.Services.Interfaces;

/// <summary>
/// Contrato para extração de JSON anexado em PDFs de transferência de dados.
/// </summary>
public interface IPdfJsonExtractor
{
    /// <summary> Extrai o conteúdo JSON de um PDF de dados a partir dos bytes do arquivo. </summary>
    string ExtractJsonFromPdf(byte[] pdfBytes);
}
