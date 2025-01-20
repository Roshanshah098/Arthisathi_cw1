using arthiksathi.Models;

namespace arthiksathi.Services;

public interface ITransactionService
{
    Task<List<Transaction>> GetTransactionsAsync(
        string username,
        DateTime? startDate = null,
        DateTime? endDate = null,
        TransactionType? type = null,
        string? searchTerm = null,
        List<string>? tags = null);
    Task<bool> AddTransactionAsync(string username, Transaction transaction);
    Task<bool> HasSufficientBalanceAsync(string username, decimal amount);
    Task<TransactionStats> GetStatsAsync(string username, DateTime? startDate = null, DateTime? endDate = null);
} 