using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Application.Services.Implementations;
using TaskFlow.Application.Services.Interfaces;

namespace TaskFlow.Application;

/// <summary>
/// Configuração da injeção de dependência para a camada de Aplicação.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra todos os serviços da camada de Aplicação no contêiner de DI.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<INoteService, NoteService>();
        services.AddScoped<IExportService, ExportService>();
        services.AddScoped<IImportService, ImportService>();

        return services;
    }
}
