using arthiksathi.Models;

namespace arthiksathi.Services;

public interface IStorageService : IDisposable  
{
    Task<AppState> LoadStateAsync(); 
    Task SaveStateAsync(AppState state);
    Task<User?> AuthenticateUser(string username, string password); 
    Task<bool> RegisterUser(User user);  
} 