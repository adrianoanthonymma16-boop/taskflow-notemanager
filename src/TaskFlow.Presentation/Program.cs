using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using TaskFlow.Application;
using TaskFlow.Infrastructure;
using TaskFlow.Infrastructure.Data;
using TaskFlow.Infrastructure.Services;

namespace TaskFlow.Presentation;

public static class Program
{
    private static readonly string LockFilePath = Path.Combine(Path.GetTempPath(), "taskflow.pid");
    private static readonly string DbPath = Path.Combine(
        new FileStorageService().GetDataDirectory(), "taskflow.db");
    private static Mutex? _mutex;

    public static void Main(string[] args)
    {
        if (!TryAcquireLock())
        {
            ShowAlreadyRunning();
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
            .AddInteractiveServerComponents(circuit =>
            {
                circuit.DetailedErrors = builder.Environment.IsDevelopment();
                circuit.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(5);
                circuit.JSInteropDefaultCallTimeout = TimeSpan.FromSeconds(120);
            });

        builder.Services.Configure<HubOptions>(options =>
        {
            options.MaximumReceiveMessageSize = 64 * 1024 * 1024;
            options.KeepAliveInterval = TimeSpan.FromSeconds(15);
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
            options.HandshakeTimeout = TimeSpan.FromSeconds(30);
        });

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

        app.Lifetime.ApplicationStopped.Register(ReleaseLock);

        OpenBrowser("http://localhost:5000");

        app.Run();
    }

    private static bool TryAcquireLock()
    {
        try
        {
            _mutex = new Mutex(true, "TaskFlow_NoteManager_SingleInstance", out var createdNew);
            if (!createdNew)
            {
                _mutex.Close();
                _mutex = null;
                return false;
            }
        }
        catch
        {
            var pid = TryReadLockFile();
            if (pid is not null && ProcessExists(pid.Value))
                return false;
        }

        try
        {
            File.WriteAllText(LockFilePath, Environment.ProcessId.ToString());
        }
        catch { }

        return true;
    }

    private static int? TryReadLockFile()
    {
        try
        {
            if (File.Exists(LockFilePath))
            {
                var txt = File.ReadAllText(LockFilePath);
                if (int.TryParse(txt.Trim(), out var pid))
                    return pid;
            }
        }
        catch { }
        return null;
    }

    private static bool ProcessExists(int pid)
    {
        try
        {
            var p = Process.GetProcessById(pid);
            return !p.HasExited;
        }
        catch
        {
            return false;
        }
    }

    private static void ShowAlreadyRunning()
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                _ = MessageBox(IntPtr.Zero,
                    "A aplicação já está em execução.\nVerifique a bandeja do sistema.",
                    "TaskFlow NoteManager", 0x00000030);
            }
            else
            {
                Console.Error.WriteLine("⚠ TaskFlow NoteManager já está em execução.");
            }
        }
        catch { }
    }

    private static void ReleaseLock()
    {
        try
        {
            _mutex?.Close();
            _mutex?.Dispose();
        }
        catch { }

        try
        {
            if (File.Exists(LockFilePath))
                File.Delete(LockFilePath);
        }
        catch { }
    }

    private static void OpenBrowser(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch
        {
        }
    }

    [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);
}
