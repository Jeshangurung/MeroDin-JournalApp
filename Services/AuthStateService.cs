using Journal.Entities;
using Journal.Data;
using Microsoft.EntityFrameworkCore;

public class AuthStateService
{
    private readonly AppDbContext _db;
    private const string AuthUserIdKey = "auth_user_id";

    public User? CurrentUser { get; private set; }
    
    // Step 1: Logged in (Password passed or remembered)
    public bool IsAuthenticated => CurrentUser != null;
    
    // Step 2: Unlocked (PIN passed)
    public bool IsUnlocked { get; private set; } = false;

    public event Action? OnChange;

    public AuthStateService(AppDbContext db)
    {
        _db = db;
        InitializeAsync().Wait(); // Synchronous wait for startup (simple for Scoped service)
    }

    private async Task InitializeAsync()
    {
        if (Preferences.Default.ContainsKey(AuthUserIdKey))
        {
            var userIdStr = Preferences.Default.Get(AuthUserIdKey, "");
            if (int.TryParse(userIdStr, out int userId))
            {
                CurrentUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            }
        }
    }

    public void Authenticate(User user)
    {
        CurrentUser = user;
        IsUnlocked = false; 
        
        // Remember the user ID for next time
        Preferences.Default.Set(AuthUserIdKey, user.Id.ToString());
        
        NotifyStateChanged();
    }

    public void Unlock()
    {
        IsUnlocked = true;
        NotifyStateChanged();
    }

    public void Lock()
    {
        IsUnlocked = false;
        NotifyStateChanged();
    }

    public void Logout()
    {
        CurrentUser = null;
        IsUnlocked = false;
        
        // Forget the user
        Preferences.Default.Remove(AuthUserIdKey);
        
        NotifyStateChanged();
    }

    public void Login(User user) => Authenticate(user);

    private void NotifyStateChanged() => OnChange?.Invoke();
}
