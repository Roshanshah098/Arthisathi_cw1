using System.Text.Json;
using arthiksathi.Models;

namespace arthiksathi.Services;

public class StorageService : IStorageService
{
    private readonly string _filePath;
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    public StorageService()
    {
        try
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                "arthiksathi"
            );
            Directory.CreateDirectory(appDataPath);
            _filePath = Path.Combine(appDataPath, "appstate.json");
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Failed to initialize storage service", ex);
        }
    }

    public async Task<AppState> LoadStateAsync()
    {
        // Try to acquire the semaphore with timeout
        if (!await _semaphore.WaitAsync(TimeSpan.FromSeconds(5)))
        {
            throw new TimeoutException("Operation timed out while waiting to access storage");
        }

        try
        {
            // If file doesn't exist, return new state
            if (!File.Exists(_filePath))
            {
                return new AppState();
            }

            // Read and deserialize the state
            string json = await File.ReadAllTextAsync(_filePath);
            if (string.IsNullOrEmpty(json))
            {
                return new AppState();
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var state = JsonSerializer.Deserialize<AppState>(json, options);
            return state ?? new AppState();
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Failed to deserialize application state", ex);
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException("Failed to read application state from storage", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Unexpected error while loading application state", ex);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task SaveStateAsync(AppState state)
    {
        if (state == null)
        {
            throw new ArgumentNullException(nameof(state));
        }

        if (!await _semaphore.WaitAsync(TimeSpan.FromSeconds(5)))
        {
            throw new TimeoutException("Operation timed out while waiting to access storage");
        }

        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };

            string json = JsonSerializer.Serialize(state, options);
            await File.WriteAllTextAsync(_filePath, json);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Failed to serialize application state", ex);
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException("Failed to write application state to storage", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Unexpected error while saving application state", ex);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<User?> AuthenticateUser(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Username and password are required");
        }

        try
        {
            var state = await LoadStateAsync();
            return state.Users.FirstOrDefault(u => 
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && 
                u.Password == password);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to authenticate user: {username}", ex);
        }
    }

    public async Task<bool> RegisterUser(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
        {
            throw new ArgumentException("Username and password are required");
        }

        try
        {
            var state = await LoadStateAsync();
            
            // Check if user already exists (case-insensitive comparison)
            if (state.Users.Any(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            state.Users.Add(user);
            await SaveStateAsync(state);
            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to register user: {user.Username}", ex);
        }
    }

    // Ensures resources are properly released
    public void Dispose()
    {
        _semaphore.Dispose();
    }
}