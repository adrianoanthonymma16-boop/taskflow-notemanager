using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Services.Interfaces;

namespace TaskFlow.Infrastructure.Pdf;

public class QuestPdfReportGenerator : IPdfReportGenerator
{
    private static readonly Color Accent = Colors.Blue.Darken2;
    private static readonly Color Border = Colors.Grey.Lighten2;
    private static readonly Color BgCard = Colors.Grey.Lighten4;
    private static readonly Color BgPending = Colors.Orange.Lighten4;
    private static readonly Color BgResolved = Colors.Green.Lighten4;
    private static readonly Color TextMuted = Colors.Grey.Darken1;
    private static readonly Color TextWhite = Colors.White;

    public byte[] Generate(ExportManifest m)
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
                    header.Item().Row(r =>
                    {
                        r.AutoItem().PaddingRight(10).Column(logo =>
                        {
                            logo.Item().Text("TF").FontSize(28).Bold().FontColor(Accent);
                        });
                        r.RelativeItem().Column(title =>
                        {
                            title.Item().Text("TaskFlow NoteManager").FontSize(22).Bold().FontColor(Accent).AlignCenter();
                            title.Item().Text("Relatório de Tarefas e Notas").FontSize(13).FontColor(TextMuted).AlignCenter();
                        });
                    });

                    header.Item().PaddingTop(4).Column(info =>
                    {
                        info.Item().Row(r =>
                        {
                            r.RelativeItem().Text($"Proprietário: {m.OwnerName}").FontSize(10);
                            r.RelativeItem().Text($"Escopo: {m.ScopeName}").FontSize(10).AlignRight();
                        });
                        info.Item().Row(r =>
                        {
                            r.RelativeItem().Text($"Exportado: {m.ExportDate:dd/MM/yyyy HH:mm} UTC").FontSize(10);
                            r.RelativeItem().Text($"Migração: {m.MigrationId}").FontSize(9).FontColor(TextMuted).AlignRight();
                        });
                        if (!string.IsNullOrWhiteSpace(m.FilterDescription))
                            info.Item().Text($"Período: {m.FilterDescription}").FontSize(10).FontColor(Colors.Orange.Darken2);
                        info.Item().Text($"Versão do formato: {m.ExportVersion}").FontSize(8).FontColor(Colors.Grey.Lighten1);
                    });

                    header.Item().PaddingTop(4).LineHorizontal(2).LineColor(Accent);
                });

                page.Content().Column(content =>
                {
                    RenderSummaryBar(content, m);

                    if (m.Tasks.Count > 0)
                        RenderTasksSection(content, m);

                    if (m.Notes.Count > 0)
                        RenderNotesSection(content, m);

                    if (m.PendingLogs.Count > 0)
                        RenderPendingSection(content, m);

                    RenderFooterInfo(content, m);
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("TaskFlow NoteManager — Página ").FontSize(8).FontColor(TextMuted);
                    x.CurrentPageNumber().FontSize(8).FontColor(TextMuted);
                });
            });
        }).GeneratePdf();
    }

    private void RenderSummaryBar(ColumnDescriptor content, ExportManifest m)
    {
        content.Item().PaddingTop(10).PaddingBottom(8).Background(Accent).Padding(10).Row(summary =>
        {
            summary.RelativeItem().Column(c =>
            {
                c.Item().Text("Tarefas").FontSize(8).FontColor(TextWhite);
                c.Item().Text($"{m.TotalTasks}").FontSize(14).Bold().FontColor(TextWhite);
            });
            summary.RelativeItem().Column(c =>
            {
                c.Item().Text("Notas").FontSize(8).FontColor(TextWhite);
                c.Item().Text($"{m.TotalNotes}").FontSize(14).Bold().FontColor(TextWhite);
            });
            summary.RelativeItem().Column(c =>
            {
                c.Item().Text("Pendências").FontSize(8).FontColor(TextWhite);
                c.Item().Text($"{m.TotalPendingLogs} ({m.ActivePendingLogs} ativas)").FontSize(14).Bold().FontColor(TextWhite);
            });
            summary.RelativeItem().Column(c =>
            {
                c.Item().Text("Anexos").FontSize(8).FontColor(TextWhite);
                c.Item().Text($"{m.TotalAttachments}").FontSize(14).Bold().FontColor(TextWhite);
            });
        });
    }

    private void RenderTasksSection(ColumnDescriptor content, ExportManifest m)
    {
        content.Item().PaddingTop(12);
        content.Item().Row(r => { r.AutoItem().PaddingRight(6).Text("📋").FontSize(16); r.RelativeItem().Text($"Tarefas ({m.Tasks.Count})").FontSize(16).Bold().FontColor(Accent); });
        content.Item().PaddingBottom(4).LineHorizontal(1).LineColor(Border);

        foreach (var task in m.Tasks)
        {
            content.Item().PaddingTop(8);

            content.Item().Border(1).BorderColor(Border).Background(BgCard).Padding(10).Column(card =>
            {
                card.Item().Row(r =>
                {
                    r.RelativeItem().Text(task.Title).FontSize(13).Bold();
                    r.AutoItem().Background(StatusColor(task.Status)).PaddingHorizontal(8).PaddingVertical(2)
                        .Text(task.StatusName).FontSize(9).Bold().FontColor(TextWhite);
                });

                card.Item().PaddingVertical(3).LineHorizontal(1).LineColor(Border);

                card.Item().Row(details =>
                {
                    details.RelativeItem().Column(c =>
                    {
                        c.Item().Text($"Criada: {task.CreatedAt:dd/MM/yyyy HH:mm}").FontSize(9).FontColor(TextMuted);
                        if (task.UpdatedAt.HasValue)
                            c.Item().Text($"Atualizada: {task.UpdatedAt:dd/MM/yyyy HH:mm}").FontSize(9).FontColor(TextMuted);
                    });
                    details.RelativeItem().Column(c =>
                    {
                        c.Item().Text($"Prazo: {task.DueDate?.ToString("dd/MM/yyyy") ?? "Sem prazo"}").FontSize(9).FontColor(TextMuted).AlignRight();
                        if (task.DueDate.HasValue)
                        {
                            var days = (task.DueDate.Value.Date - DateTime.Today).Days;
                            c.Item().Text(days < 0 ? $"Atrasada há {-days}d" : days == 0 ? "Vence hoje" : $"Faltam {days}d").FontSize(8).FontColor(days < 0 ? Colors.Red.Darken2 : Colors.Orange.Darken2).AlignRight();
                        }
                    });
                });

                if (!string.IsNullOrWhiteSpace(task.Description))
                {
                    card.Item().PaddingTop(4).Background(Colors.White).Padding(6).Border(1).BorderColor(Border)
                        .Text(task.Description).FontSize(9);
                }

                if (task.PendingLogs.Count > 0)
                {
                    card.Item().PaddingTop(5);
                    card.Item().Text("Pendências:").FontSize(10).Bold().FontColor(Colors.Orange.Darken3);

                    foreach (var log in task.PendingLogs.OrderByDescending(l => l.CreatedAt))
                    {
                        card.Item().PaddingTop(3).Background(log.IsActive ? BgPending : BgResolved).Padding(8).Border(1).BorderColor(log.IsActive ? Colors.Orange.Lighten1 : Colors.Green.Lighten1).Column(pend =>
                        {
                            pend.Item().Row(r =>
                            {
                                r.AutoItem().Text(log.IsActive ? "⚠ ATIVA" : "✅ RESOLVIDA").FontSize(8).Bold().FontColor(log.IsActive ? Colors.Orange.Darken3 : Colors.Green.Darken3);
                                r.RelativeItem().Text($"Criada: {log.CreatedAt:dd/MM/yyyy HH:mm}").FontSize(8).FontColor(TextMuted).AlignRight();
                            });

                            pend.Item().Text($"Motivo: {log.Reason}").FontSize(9);
                            pend.Item().Text($"Registrado por: {log.OwnerName}").FontSize(8).FontColor(TextMuted);

                            if (!string.IsNullOrWhiteSpace(log.CounterpartyName))
                                pend.Item().Text($"Contraparte: {log.CounterpartyName}").FontSize(8).FontColor(TextMuted);

                            if (log.ResolvedAt.HasValue)
                                pend.Item().Text($"Resolvida: {log.ResolvedAt:dd/MM/yyyy HH:mm}").FontSize(8).FontColor(Colors.Green.Darken3);

                            if (!string.IsNullOrWhiteSpace(log.ResolutionNote))
                                pend.Item().Text($"Providências: {log.ResolutionNote}").FontSize(8).FontColor(Colors.Blue.Darken1).Italic();

                            if (!string.IsNullOrWhiteSpace(log.MySignature) || !string.IsNullOrWhiteSpace(log.CounterpartySignature))
                            {
                                pend.Item().Text("Assinaturas registradas").FontSize(8).Italic().FontColor(TextMuted);
                            }
                        });
                    }
                }
            });
        }
    }

    private void RenderNotesSection(ColumnDescriptor content, ExportManifest m)
    {
        content.Item().PaddingTop(16);
        content.Item().Row(r => { r.AutoItem().PaddingRight(6).Text("📄").FontSize(16); r.RelativeItem().Text($"Notas ({m.Notes.Count})").FontSize(16).Bold().FontColor(Accent); });
        content.Item().PaddingBottom(4).LineHorizontal(1).LineColor(Border);

        foreach (var note in m.Notes)
        {
            content.Item().PaddingTop(8);

            content.Item().Border(1).BorderColor(Border).Background(BgCard).Padding(10).Column(card =>
            {
                card.Item().Row(r =>
                {
                    r.AutoItem().PaddingRight(6).Text($"#{note.NoteNumber}").FontSize(10).Bold().FontColor(TextMuted);
                    r.RelativeItem().Text(note.Title).FontSize(13).Bold();
                    r.AutoItem().Background(NoteStatusColor(note.Status)).PaddingHorizontal(8).PaddingVertical(2)
                        .Text(note.StatusName).FontSize(9).Bold().FontColor(TextWhite);
                });

                card.Item().PaddingVertical(3).LineHorizontal(1).LineColor(Border);

                card.Item().Row(details =>
                {
                    details.RelativeItem().Text($"Criada: {note.CreatedAt:dd/MM/yyyy HH:mm}").FontSize(9).FontColor(TextMuted);
                    if (note.UpdatedAt.HasValue)
                        details.RelativeItem().Text($"Atualizada: {note.UpdatedAt:dd/MM/yyyy HH:mm}").FontSize(9).FontColor(TextMuted).AlignRight();
                });

                if (!string.IsNullOrWhiteSpace(note.Content))
                {
                    card.Item().PaddingTop(4).Background(Colors.White).Padding(6).Border(1).BorderColor(Border)
                        .Text(note.Content).FontSize(9);
                }

                var noteAttachments = m.Attachments.Where(a => a.NoteId == note.Id).ToList();
                if (noteAttachments.Count > 0)
                {
                    card.Item().PaddingTop(4);
                    card.Item().Text($"Anexos ({noteAttachments.Count}):").FontSize(9).Bold().FontColor(TextMuted);
                    foreach (var att in noteAttachments)
                        card.Item().PaddingLeft(8).Text($"• {att.FileName} ({att.FileSizeFormatted})").FontSize(8).FontColor(TextMuted);
                }
            });
        }
    }

    private void RenderPendingSection(ColumnDescriptor content, ExportManifest m)
    {
        content.Item().PaddingTop(16);
        content.Item().Row(r => { r.AutoItem().PaddingRight(6).Text("⚠️").FontSize(16); r.RelativeItem().Text($"Pendências ({m.TotalPendingLogs})").FontSize(16).Bold().FontColor(Colors.Orange.Darken2); });
        content.Item().PaddingBottom(4).LineHorizontal(1).LineColor(Border);

        foreach (var log in m.PendingLogs.OrderByDescending(l => l.CreatedAt))
        {
            content.Item().PaddingTop(4).Background(log.IsActive ? BgPending : BgResolved).Padding(8).Border(1).BorderColor(log.IsActive ? Colors.Orange.Lighten1 : Colors.Green.Lighten1).Column(pend =>
            {
                pend.Item().Row(r =>
                {
                    r.AutoItem().Text(log.IsActive ? "⚠ ATIVA" : "✅ RESOLVIDA").FontSize(9).Bold().FontColor(log.IsActive ? Colors.Orange.Darken3 : Colors.Green.Darken3);
                    r.RelativeItem().Text(log.CreatedAt.ToString("dd/MM/yyyy HH:mm")).FontSize(9).FontColor(TextMuted).AlignRight();
                });
                pend.Item().Text($"Tarefa ID {log.TaskId}").FontSize(8).FontColor(TextMuted);
                pend.Item().Text($"Motivo: {log.Reason}").FontSize(10);
                pend.Item().Text($"Por: {log.OwnerName}").FontSize(9).FontColor(TextMuted);
                if (!string.IsNullOrWhiteSpace(log.CounterpartyName))
                    pend.Item().Text($"Contraparte: {log.CounterpartyName}").FontSize(9).FontColor(TextMuted);
                if (log.ResolvedAt.HasValue)
                    pend.Item().Text($"Resolvida: {log.ResolvedAt:dd/MM/yyyy HH:mm}").FontSize(9).FontColor(Colors.Green.Darken3);
                if (!string.IsNullOrWhiteSpace(log.ResolutionNote))
                    pend.Item().Text($"Providências: {log.ResolutionNote}").FontSize(9).FontColor(Colors.Blue.Darken1).Italic();
            });
        }
    }

    private void RenderFooterInfo(ColumnDescriptor content, ExportManifest m)
    {
        content.Item().PaddingTop(20).LineHorizontal(1).LineColor(Border);
        content.Item().PaddingTop(6).Row(r =>
        {
            r.RelativeItem().Text($"Migração: {m.MigrationId}").FontSize(8).FontColor(Colors.Grey.Lighten1);
            r.RelativeItem().Text($"Formato: {m.ExportVersion}").FontSize(8).FontColor(Colors.Grey.Lighten1).AlignRight();
        });
        content.Item().Text("Este PDF pode ser importado por outro usuário do TaskFlow NoteManager.").FontSize(8).FontColor(TextMuted);
        content.Item().Text("Os dados contidos são reutilizáveis e completos para migração entre instâncias.").FontSize(8).FontColor(TextMuted);
    }

    private static string StatusColor(int s) => s switch { 0 => Colors.Blue.Darken2, 1 => Colors.Orange.Darken2, 2 => Colors.Green.Darken2, _ => Colors.Grey.Darken1 };
    private static string NoteStatusColor(int s) => s switch { 0 => Colors.Grey.Darken1, 1 => Colors.Green.Darken2, 2 => Colors.Purple.Darken2, _ => Colors.Grey.Darken1 };
}
