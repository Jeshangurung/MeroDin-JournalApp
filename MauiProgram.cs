using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Journal.Data;
using QuestPDF.Infrastructure;
using Journal.Services;

namespace Journal;
using Data;

    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
        QuestPDF.Settings.License = LicenseType.Community;

        var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();

#endif
            // ✅ Register your AppDbContext for DI
            builder.Services.AddDbContext<AppDbContext>();
            builder.Services.AddScoped<IJournalService, JournalService>();
            builder.Services.AddScoped<PinService>();
            builder.Services.AddScoped<AuthStateService>();
            builder.Services.AddScoped<ThemeService>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                // Ensure the database is created if it doesn't exist
                db.Database.EnsureCreated();

                // Manual Migration: Add IsPublic column if it doesn't exist
                try
                {
                    db.Database.ExecuteSqlRaw("ALTER TABLE JournalEntries ADD COLUMN IsPublic INTEGER NOT NULL DEFAULT 0;");
                }
                catch
                {
                    // Column likely already exists
                }
            }

            return app;
        }
    }
