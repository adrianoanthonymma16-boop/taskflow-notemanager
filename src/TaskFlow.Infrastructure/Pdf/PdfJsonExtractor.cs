using TaskFlow.Application.Services.Interfaces;

namespace TaskFlow.Infrastructure.Pdf;

/// <summary>
/// Extracts JSON content embedded as attachment in data-transfer PDFs (Type 1).
/// </summary>
public class PdfJsonExtractor : IPdfJsonExtractor
{
    /// <inheritdoc/>
    public string ExtractJsonFromPdf(byte[] pdfBytes)
    {
        // Simple approach: parse the PDF and look for an embedded file stream containing JSON.
        // This scans the raw PDF bytes for a JSON object embedded in a stream.
        var pdfText = System.Text.Encoding.UTF8.GetString(pdfBytes);

        // Look for JSON content after the /EmbeddedFile marker or within stream/endstream
        var jsonStart = pdfText.LastIndexOf('{');
        var jsonEnd = pdfText.LastIndexOf('}');

        if (jsonStart == -1 || jsonEnd == -1 || jsonEnd < jsonStart)
            throw new InvalidOperationException("No JSON content found in PDF.");

        var json = pdfText.Substring(jsonStart, jsonEnd - jsonStart + 1);

        // Decompress if needed (ASCII85Decode or FlateDecode are common filters)
        // For now, return as-is assuming uncompressed JSON

        if (string.IsNullOrWhiteSpace(json))
            throw new InvalidOperationException("Empty JSON content extracted from PDF.");

        return json;
    }
}
