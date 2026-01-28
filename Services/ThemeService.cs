using Microsoft.Maui.Storage;

namespace Journal.Services
{
    /// <summary>
    /// Manages application-wide theme preferences and persistence.
    /// Supports light and dark modes with automatic validation and subscriber notifications.
    /// </summary>
    public class ThemeService
    {
        private const string ThemeKey = "current_theme";
        
        public event Action? OnThemeChanged;

        public string CurrentTheme { get; private set; } = "light";

        public ThemeService()
        {
            // Restores the user's last selected theme from persistent storage.
            CurrentTheme = Preferences.Default.Get(ThemeKey, "light");
            
            // Validates the retrieved value and defaults to light mode if corrupted.
            if (CurrentTheme != "light" && CurrentTheme != "dark")
            {
                CurrentTheme = "light";
            }
        }

        /// <summary>
        /// Updates the active theme and persists the change across application sessions.
        /// Triggers the OnThemeChanged event to notify subscribed UI components.
        /// </summary>
        public void SetTheme(string theme)
        {
            if (CurrentTheme != theme)
            {
                CurrentTheme = theme;
                Preferences.Default.Set(ThemeKey, theme);
                OnThemeChanged?.Invoke();
            }
        }
    }
}
