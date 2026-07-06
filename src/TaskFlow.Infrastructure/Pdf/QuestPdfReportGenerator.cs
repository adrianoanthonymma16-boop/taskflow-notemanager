using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Services.Interfaces;

namespace TaskFlow.Infrastructure.Pdf;

public class QuestPdfReportGenerator : IPdfReportGenerator
{
    private static readonly Color AccentColor = Colors.Blue.Darken2;
    private static readonly Color BorderColor = Colors.Grey.Lighten2;
    private static readonly Color BgLight = Colors.Grey.Lighten4;
    private static readonly Color BgPending = Colors.Orange.Lighten4;
    private static readonly Color BgResolved = Colors.Green.Lighten4;

    public byte[] Generate(ExportManifest manifest)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(35);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Calibri"));

                page.Header().Column(header =>
                {
                    header.Item().Row(row =>
                    {
                        row.AutoItem().PaddingRight(10).Column(logo =>
                        {
                            logo.Item().Text("TF").FontSize(28).Bold().FontColor(AccentColor);
                        });
                        row.RelativeItem().Column(title =>
                        {
                            title.Item().Text("TaskFlow NoteManager").FontSize(22).Bold().FontColor(AccentColor).AlignCenter();
                            title.Item().Text("Relatório de Tarefas e Notas").FontSize(13).FontColor(Colors.Grey.Darken1).AlignCenter();
                        });
                    });

                    header.Item().PaddingTop(6).Row(info =>
                    {
                        info.RelativeItem().Text($"Proprietário: {manifest.OwnerName}").FontSize(10);
                        info.RelativeItem().Text($"Exportado em: {manifest.ExportDate:dd/MM/yyyy HH:mm}").FontSize(10).AlignRight();
                    });

                    header.Item().PaddingTop(4).LineHorizontal(2).LineColor(AccentColor);
                });

                page.Content().Column(content =>
                {
                    if (manifest.Tasks.Count > 0)
                        RenderTasksSection(content, manifest);

                    if (manifest.Notes.Count > 0)
                        RenderNotesSection(content, manifest);

                    if (manifest.PendingLogs.Count > 0)
                        RenderGlobalPendingSection(content, manifest);
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Gerado por TaskFlow NoteManager — Página ").FontSize(8).FontColor(Colors.Grey.Darken1);
                    x.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Darken1);
                });
            });
        }).GeneratePdf();
    }

    private void RenderTasksSection(ColumnDescriptor content, ExportManifest manifest)
    {
        content.Item().PaddingTop(16);
        content.Item().Row(row =>
        {
            row.AutoItem().PaddingRight(6).Text("📋").FontSize(16);
            row.RelativeItem().Text($"Tarefas ({manifest.Tasks.Count})").FontSize(16).Bold().FontColor(AccentColor);
        });
        content.Item().PaddingBottom(4).LineHorizontal(1).LineColor(BorderColor);

        foreach (var task in manifest.Tasks)
        {
            content.Item().PaddingTop(10);

            content.Item().Border(1).BorderColor(BorderColor).Background(BgLight).Padding(12).Column(taskBlock =>
            {
                taskBlock.Item().Row(row =>
                {
                    row.RelativeItem().Text(task.Title).FontSize(13).Bold();
                    row.AutoItem().Background(StatusColor(task.Status)).PaddingHorizontal(8).PaddingVertical(2)
                        .Text(task.StatusName).FontSize(9).Bold().FontColor(Colors.White);
                });

                taskBlock.Item().PaddingVertical(4).LineHorizontal(1).LineColor(BorderColor);

                taskBlock.Item().Row(details =>
                {
                    details.RelativeItem().Text($"Criada em: {task.CreatedAt:dd/MM/yyyy HH:mm}").FontSize(9).FontColor(Colors.Grey.Darken1);
                    details.RelativeItem().Text($"Prazo: {task.DueDate?.ToString("dd/MM/yyyy") ?? "Sem prazo"}").FontSize(9).FontColor(Colors.Grey.Darken1).AlignRight();
                });

                if (!string.IsNullOrWhiteSpace(task.Description))
                {
                    taskBlock.Item().PaddingTop(4).Background(Colors.White).Padding(6).Border(1).BorderColor(BorderColor)
                        .Text(task.Description).FontSize(9);
                }

                if (task.PendingLogs.Count > 0)
                {
                    taskBlock.Item().PaddingTop(6);
                    taskBlock.Item().Text("Pendências:").FontSize(10).Bold().FontColor(Colors.Orange.Darken2);

                    foreach (var log in task.PendingLogs.OrderByDescending(l => l.CreatedAt))
                    {
                        taskBlock.Item().PaddingTop(4).Background(log.IsActive ? BgPending : BgResolved).Padding(8).Border(1).BorderColor(log.IsActive ? Colors.Orange.Lighten1 : Colors.Green.Lighten1).Column(pend =>
                        {
                            pend.Item().Row(r =>
                            {
                                r.AutoItem().Text(log.IsActive ? "⚠ ATIVA" : "✅ RESOLVIDA").FontSize(8).Bold().FontColor(log.IsActive ? Colors.Orange.Darken3 : Colors.Green.Darken3);
                                r.RelativeItem().Text(log.CreatedAt.ToString("dd/MM/yyyy HH:mm")).FontSize(8).FontColor(Colors.Grey.Darken1).AlignRight();
                            });

                            pend.Item().PaddingTop(2).Text($"Motivo: {log.Reason}").FontSize(9);
                            pend.Item().Text($"Registrado por: {log.OwnerName}").FontSize(8).FontColor(Colors.Grey.Darken1);

                            if (!string.IsNullOrWhiteSpace(log.CounterpartyName))
                                pend.Item().Text($"Contraparte: {log.CounterpartyName}").FontSize(8).FontColor(Colors.Grey.Darken1);

                            if (log.ResolvedAt.HasValue)
                                pend.Item().Text($"Resolvida em: {log.ResolvedAt:dd/MM/yyyy HH:mm}").FontSize(8).FontColor(Colors.Grey.Darken1);
                        });
                    }
                }
            });
        }
    }

    private void RenderNotesSection(ColumnDescriptor content, ExportManifest manifest)
    {
        content.Item().PaddingTop(20);
        content.Item().Row(row =>
        {
            row.AutoItem().PaddingRight(6).Text("📄").FontSize(16);
            row.RelativeItem().Text($"Notas ({manifest.Notes.Count})").FontSize(16).Bold().FontColor(AccentColor);
        });
        content.Item().PaddingBottom(4).LineHorizontal(1).LineColor(BorderColor);

        foreach (var note in manifest.Notes)
        {
            content.Item().PaddingTop(10);

            content.Item().Border(1).BorderColor(BorderColor).Background(BgLight).Padding(12).Column(noteBlock =>
            {
                noteBlock.Item().Row(row =>
                {
                    row.AutoItem().PaddingRight(6).Text($"#{note.NoteNumber}").FontSize(9).Bold().FontColor(Colors.Grey.Darken1);
                    row.RelativeItem().Text(note.Title).FontSize(13).Bold();
                    row.AutoItem().Background(NoteStatusColor(note.Status)).PaddingHorizontal(8).PaddingVertical(2)
                        .Text(note.StatusName).FontSize(9).Bold().FontColor(Colors.White);
                });

                noteBlock.Item().PaddingVertical(4).LineHorizontal(1).LineColor(BorderColor);

                noteBlock.Item().Text($"Criada em: {note.CreatedAt:dd/MM/yyyy HH:mm}").FontSize(9).FontColor(Colors.Grey.Darken1);
                if (note.UpdatedAt.HasValue)
                    noteBlock.Item().Text($"Atualizada em: {note.UpdatedAt:dd/MM/yyyy HH:mm}").FontSize(9).FontColor(Colors.Grey.Darken1);

                if (!string.IsNullOrWhiteSpace(note.Content))
                {
                    noteBlock.Item().PaddingTop(4).Background(Colors.White).Padding(6).Border(1).BorderColor(BorderColor)
                        .Text(note.Content).FontSize(9);
                }

                var noteAttachments = manifest.Attachments.Where(a => a.NoteId == note.Id).ToList();
                if (noteAttachments.Count > 0)
                {
                    noteBlock.Item().PaddingTop(4);
                    noteBlock.Item().Text($"Anexos ({noteAttachments.Count}):").FontSize(9).Bold().FontColor(Colors.Grey.Darken1);

                    foreach (var att in noteAttachments)
                    {
                        noteBlock.Item().PaddingLeft(8).Text($"• {att.FileName} ({att.FileSizeFormatted})").FontSize(8).FontColor(Colors.Grey.Darken1);
                    }
                }

                var notePendingLogs = manifest.PendingLogs
                    .Where(pl => pl.TaskId == 0 || manifest.Tasks.Any(t => t.Id == pl.TaskId && t.Title.Contains(note.Title)))
                    .ToList();

                if (notePendingLogs.Count > 0)
                {
                    noteBlock.Item().PaddingTop(4);
                    noteBlock.Item().Text("Pendências relacionadas:").FontSize(9).Bold().FontColor(Colors.Orange.Darken2);

                    foreach (var log in notePendingLogs)
                    {
                        noteBlock.Item().PaddingLeft(8).Background(log.IsActive ? BgPending : BgResolved).Padding(4).Border(1).BorderColor(BorderColor).Column(p =>
                        {
                            p.Item().Text($"{(log.IsActive ? "⚠" : "✅")} {log.Reason}").FontSize(8);
                            p.Item().Text($"Registrado por: {log.OwnerName} em {log.CreatedAt:dd/MM/yyyy HH:mm}").FontSize(7).FontColor(Colors.Grey.Darken1);
                        });
                    }
                }
            });
        }
    }

    private void RenderGlobalPendingSection(ColumnDescriptor content, ExportManifest manifest)
    {
        content.Item().PaddingTop(20);
        content.Item().Row(row =>
        {
            row.AutoItem().PaddingRight(6).Text("⚠️").FontSize(16);
            row.RelativeItem().Text($"Pendências do Sistema ({manifest.PendingLogs.Count})").FontSize(16).Bold().FontColor(Colors.Orange.Darken2);
        });
        content.Item().PaddingBottom(4).LineHorizontal(1).LineColor(BorderColor);

        foreach (var log in manifest.PendingLogs.OrderByDescending(l => l.CreatedAt))
        {
            content.Item().PaddingTop(6).Background(log.IsActive ? BgPending : BgResolved).Padding(10).Border(1).BorderColor(log.IsActive ? Colors.Orange.Lighten1 : Colors.Green.Lighten1).Column(pend =>
            {
                pend.Item().Row(r =>
                {
                    r.AutoItem().Text(log.IsActive ? "⚠ ATIVA" : "✅ RESOLVIDA").FontSize(9).Bold().FontColor(log.IsActive ? Colors.Orange.Darken3 : Colors.Green.Darken3);
                    r.RelativeItem().Text(log.CreatedAt.ToString("dd/MM/yyyy HH:mm")).FontSize(9).FontColor(Colors.Grey.Darken1).AlignRight();
                });

                pend.Item().PaddingTop(2).Text($"Motivo: {log.Reason}").FontSize(10);
                pend.Item().Text($"Registrado por: {log.OwnerName}").FontSize(9).FontColor(Colors.Grey.Darken1);
                pend.Item().Text($"Tarefa vinculada: ID {log.TaskId}").FontSize(8).FontColor(Colors.Grey.Darken1);

                if (!string.IsNullOrWhiteSpace(log.CounterpartyName))
                    pend.Item().Text($"Contraparte: {log.CounterpartyName}").FontSize(9).FontColor(Colors.Grey.Darken1);

                if (log.ResolvedAt.HasValue)
                    pend.Item().Text($"Resolvida em: {log.ResolvedAt:dd/MM/yyyy HH:mm}").FontSize(9).FontColor(Colors.Grey.Darken1);
            });
        }
    }

    private static string StatusColor(int status) => status switch
    {
        0 => Colors.Blue.Darken2,
        1 => Colors.Orange.Darken2,
        2 => Colors.Green.Darken2,
        _ => Colors.Grey.Darken1
    };

    private static string NoteStatusColor(int status) => status switch
    {
        0 => Colors.Grey.Darken1,
        1 => Colors.Green.Darken2,
        2 => Colors.Purple.Darken2,
        _ => Colors.Grey.Darken1
    };
}
