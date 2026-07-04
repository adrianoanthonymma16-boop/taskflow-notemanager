namespace TaskFlow.Application.Services.Interfaces;

/// <summary>
/// Contrato para o serviço de armazenamento de arquivos no sistema de arquivos local.
/// </summary>
public interface IFileStorageService
{
    /// <summary> Salva um arquivo no sistema de arquivos e retorna o caminho completo. </summary>
    Task<string> SaveFileAsync(string subfolder, string fileName, Stream fileStream);

    /// <summary> Remove um arquivo do sistema de arquivos. </summary>
    Task DeleteFileAsync(string filePath);

    /// <summary> Obtém o diretório base de dados da aplicação. </summary>
    string GetDataDirectory();

    /// <summary> Copia um arquivo PDF para a pasta de visuais (Modo B). </summary>
    Task<string> CopyToVisualFolderAsync(string sourcePath, string fileName);

    /// <summary> Obtém o caminho da pasta de uploads. </summary>
    string GetUploadsDirectory();

    /// <summary> Obtém o caminho da pasta de PDFs visuais. </summary>
    string GetVisualPdfsDirectory();
}
