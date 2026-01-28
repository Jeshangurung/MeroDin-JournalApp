using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Journal.Data;
using QuestPDF.Infrastructure;
using Journal.Services;

namespace Journal;
using Data;

/// <summary>
/// This class serves as the entry point for the MAUI application, responsible for
/// bootstrapping the application, configuring dependency injection, and handling
/// initial database setup and schema consistency.
/// </summary>
public static class MauiProgram
{
    /// <summary>
    /// Creates and configures the standard MAUI application environment.
    /// Includes font registration, service registration, and database initialization.
    /// </summary>
    /// <returns>A fully configured MauiApp instance.</returns>
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
        // Register the database context using dependency injection.
        builder.Services.AddDbContext<AppDbContext>();
        
        // Register business logic services and application state managers.
        builder.Services.AddScoped<IJournalService, JournalService>();
            builder.Services.AddScoped<PinService>();
            builder.Services.AddScoped<AuthStateService>();
            builder.Services.AddScoped<ThemeService>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                // Ensure the base database file is established.
                db.Database.EnsureCreated();

                // The following blocks handle manual database schema updates for existing installations.
                // Using try-catch blocks to safely attempt column additions which may already exist.

                // Ensure the IsPublic flag is available for community sharing features.
                try
                {
                    db.Database.ExecuteSqlRaw("ALTER TABLE JournalEntries ADD COLUMN IsPublic INTEGER NOT NULL DEFAULT 0;");
                }
                catch
                {
                    // Column likely already exists
                }

                // Add PinHash to Users if missing
                try
                {
                    db.Database.ExecuteSqlRaw("ALTER TABLE Users ADD COLUMN PinHash TEXT;");
                }
                catch
                {
                    // Column already exists
                }

                // Add Email to Users if missing
                try
                {
                    db.Database.ExecuteSqlRaw("ALTER TABLE Users ADD COLUMN Email TEXT;");
                }
                catch
                {
                    // Column already exists
                }

                // Add CreatedAt to Users if missing
                try
                {
                    db.Database.ExecuteSqlRaw("ALTER TABLE Users ADD COLUMN CreatedAt TEXT NOT NULL DEFAULT '2024-01-01';");
                }
                catch
                {
                    // Column already exists
                }

                // Add UpdatedAt to Users if missing
                try
                {
                    db.Database.ExecuteSqlRaw("ALTER TABLE Users ADD COLUMN UpdatedAt TEXT NOT NULL DEFAULT '2024-01-01';");
                }
                catch
                {
                    // Column already exists
                }
            }

            return app;
        }
    }
