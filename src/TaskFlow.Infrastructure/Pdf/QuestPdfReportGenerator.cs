using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Services.Interfaces;

namespace TaskFlow.Infrastructure.Pdf;

/// <summary>
/// Generates visual/human-readable PDF reports (Type 2) from export manifest data.
/// Uses QuestPDF for clean, printable layout with tables and formatted content.
/// </summary>
public class QuestPdfReportGenerator : IPdfReportGenerator
{
    /// <inheritdoc/>
    public byte[] Generate(ExportManifest manifest)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Column(header =>
                    {
                        header.Item()
                            .Text("TaskFlow - Visual Report")
                            .FontSize(22)
                            .Bold()
                            .AlignCenter();

                        header.Item()
                            .Text($"Owner: {manifest.OwnerName} | Date: {manifest.ExportDate:yyyy-MM-dd HH:mm}")
                            .FontSize(10)
                            .AlignCenter();

                        header.Item().PaddingTop(5).LineHorizontal(1);
                    });

                page.Content()
                    .Column(content =>
                    {
                        // Tasks section
                        if (manifest.Tasks.Count > 0)
                        {
                            content.Item().PaddingTop(10)
                                .Text($"Tasks ({manifest.Tasks.Count})")
                                .FontSize(16).Bold();

                            content.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Text("Title").Bold();
                                    header.Cell().Text("Status").Bold();
                                    header.Cell().Text("Due Date").Bold();
                                });

                                foreach (var task in manifest.Tasks)
                                {
                                    table.Cell().Text(task.Title);
                                    table.Cell().Text(task.StatusName);
                                    table.Cell().Text(task.DueDate?.ToString("yyyy-MM-dd") ?? "-");
                                }
                            });
                        }

                        // Notes section
                        if (manifest.Notes.Count > 0)
                        {
                            content.Item().PaddingTop(20)
                                .Text($"Notes ({manifest.Notes.Count})")
                                .FontSize(16).Bold();

                            foreach (var note in manifest.Notes)
                            {
                                content.Item().PaddingTop(10)
                                    .Text(note.Title)
                                    .FontSize(13).Bold();

                                if (!string.IsNullOrWhiteSpace(note.Content))
                                {
                                    content.Item().PaddingTop(5)
                                        .Text(note.Content)
                                        .FontSize(10);
                                }

                                var noteAttachments = manifest.Attachments
                                    .Where(a => a.NoteId == note.Id).ToList();

                                if (noteAttachments.Count > 0)
                                {
                                    content.Item().PaddingTop(5)
                                        .Text($"Attachments: {noteAttachments.Count}")
                                        .FontSize(9).Italic();

                                    foreach (var att in noteAttachments)
                                    {
                                        content.Item()
                                            .Text($"  - {att.FileName} ({att.FileSizeFormatted})")
                                            .FontSize(9);
                                    }
                                }
                            }
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Generated by TaskFlow NoteManager — Page ");
                        x.CurrentPageNumber();
                    });
            });
        }).GeneratePdf();
    }
}
