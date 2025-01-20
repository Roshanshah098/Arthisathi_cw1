using arthiksathi.Models;

namespace arthiksathi.Services;

public class TransactionService : ITransactionService
{
    private readonly IStorageService _storageService;

    public TransactionService(IStorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task<List<Transaction>> GetTransactionsAsync( 
        string username,
        DateTime? startDate = null,
        DateTime? endDate = null,
        TransactionType? type = null,
        string? searchTerm = null,
        List<string>? tags = null)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username is required", nameof(username));
        }

        try
        {
            var state = await _storageService.LoadStateAsync();
            var query = state.Transactions.Where(t => t.Username == username);

            if (startDate.HasValue)
                query = query.Where(t => t.Date >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(t => t.Date <= endDate.Value);

            if (type.HasValue)
                query = query.Where(t => t.Type == type.Value);

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(t => 
                    t.Source.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || 
                    t.Notes.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

            if (tags != null && tags.Any())
                query = query.Where(t => t.Tags.Any(tag => tags.Contains(tag)));

            return query.OrderByDescending(t => t.Date).ToList();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to retrieve transactions", ex);
        }
    }

    public async Task<bool> AddTransactionAsync(string username, Transaction transaction)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username is required", nameof(username));
        }

        if (transaction == null)
        {
            throw new ArgumentNullException(nameof(transaction));
        }

        try
        {
            if (transaction.Type == TransactionType.Debit)
            {
                if (!await HasSufficientBalanceAsync(username, transaction.Amount))
                    return false;
            }

            var state = await _storageService.LoadStateAsync();
            transaction.Username = username;
            state.Transactions.Add(transaction);
            await _storageService.SaveStateAsync(state);
            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to add transaction", ex);
        }
    }

    public async Task<bool> HasSufficientBalanceAsync(string username, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username is required", nameof(username));
        }

        if (amount < 0)
        {
            throw new ArgumentException("Amount must be positive", nameof(amount));
        }

        try
        {
            var stats = await GetStatsAsync(username);
            return (stats.TotalInflows - stats.TotalOutflows) >= amount;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to check balance", ex);
        }
    }

    public async Task<TransactionStats> GetStatsAsync(string username, DateTime? startDate = null, DateTime? endDate = null)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username is required", nameof(username));
        }

        try
        {
            var transactions = await GetTransactionsAsync(username, startDate, endDate);

            return new TransactionStats
            {
                TotalInflows = transactions.Where(t => t.Type == TransactionType.Credit).Sum(t => t.Amount),
                TotalOutflows = transactions.Where(t => t.Type == TransactionType.Debit).Sum(t => t.Amount),
                TotalDebts = transactions.Where(t => t.Type == TransactionType.Debt).Sum(t => t.Amount),
                ClearedDebts = transactions.Where(t => t.Type == TransactionType.Debt && t.IsCleared).Sum(t => t.Amount),
                RemainingDebts = transactions.Where(t => t.Type == TransactionType.Debt && !t.IsCleared).Sum(t => t.Amount),
                TopTransactions = transactions.OrderByDescending(t => t.Amount).Take(5).ToList(),
                PendingDebts = transactions.Where(t => t.Type == TransactionType.Debt && !t.IsCleared).ToList()
            };
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to retrieve transaction statistics", ex);
        }
    }
}