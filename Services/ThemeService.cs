using Microsoft.Maui.Storage;

namespace Journal.Services
{
    public class ThemeService
    {
        private const string ThemeKey = "current_theme";
        
        public event Action? OnThemeChanged;

        public string CurrentTheme { get; private set; } = "light";

        public ThemeService()
        {
            CurrentTheme = Preferences.Default.Get(ThemeKey, "light");
            if (CurrentTheme != "light" && CurrentTheme != "dark")
            {
                CurrentTheme = "light";
            }
        }

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
