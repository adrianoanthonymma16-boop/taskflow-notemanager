using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Application.Services.Interfaces;
using TaskFlow.Domain.Interfaces;
using TaskFlow.Infrastructure.Data;
using TaskFlow.Infrastructure.Data.Repositories;
using TaskFlow.Infrastructure.Pdf;
using TaskFlow.Infrastructure.Services;

namespace TaskFlow.Infrastructure;

/// <summary>
/// Dependency injection configuration for the Infrastructure layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all Infrastructure services, repositories, and PDF generators.
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IPendingLogRepository, PendingLogRepository>();
        services.AddScoped<INoteRepository, NoteRepository>();
        services.AddScoped<IAttachmentRepository, AttachmentRepository>();
        services.AddScoped<IVisualPdfRecordRepository, VisualPdfRecordRepository>();

        // Infrastructure services
        services.AddSingleton<IFileStorageService, FileStorageService>();

        // PDF generators
        services.AddScoped<IPdfDataTransferGenerator, QuestPdfDataTransferGenerator>();
        services.AddScoped<IPdfReportGenerator, QuestPdfReportGenerator>();
        services.AddScoped<IPdfJsonExtractor, PdfJsonExtractor>();

        return services;
    }
}
