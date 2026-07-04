using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using TaskFlow.Application;
using TaskFlow.Infrastructure;
using TaskFlow.Infrastructure.Data;
using TaskFlow.Infrastructure.Services;

namespace TaskFlow.Presentation;

public static class Program
{
    private const string AppMutexName = "TaskFlow_NoteManager_SingleInstance";
    private static readonly string DbPath = Path.Combine(
        new FileStorageService().GetDataDirectory(), "taskflow.db");

    public static void Main(string[] args)
    {
        using var mutex = new Mutex(false, AppMutexName, out var createdNew);

        if (!createdNew)
        {
            MessageBoxShow("TaskFlow NoteManager",
                "A aplicação já está em execução. Verifique a bandeja do sistema.");
            return;
        }

        var builder = WebApplication.CreateBuilder(args);

        builder.WebHost.UseKestrel(options =>
        {
            options.ListenLocalhost(5000);
        });

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={DbPath}"));

        builder.Services.AddScoped<Components.Shared.UserStateService>();
        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructureServices();

        builder.Services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.BottomRight;
            config.SnackbarConfiguration.PreventDuplicates = false;
            config.SnackbarConfiguration.NewestOnTop = true;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 5000;
        });

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
        }

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<Components.App>()
            .AddInteractiveServerRenderMode();

        OpenBrowser("http://localhost:5000");

        app.Run();
    }

    private static void OpenBrowser(string url)
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            else if (OperatingSystem.IsLinux())
            {
                Process.Start("xdg-open", url);
            }
        }
        catch
        {
            // Browser launch failed silently – user can manually open
        }
    }

    [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

    private static void MessageBoxShow(string caption, string text)
    {
        if (OperatingSystem.IsWindows())
        {
            _ = MessageBox(IntPtr.Zero, text, caption, 0x00000030); // MB_ICONEXCLAMATION | MB_OK
        }
    }
}
