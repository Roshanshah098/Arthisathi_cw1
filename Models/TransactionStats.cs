using arthiksathi.Models;

public class TransactionStats
{
    public decimal TotalInflows { get; set; }
    public decimal TotalOutflows { get; set; }
    public decimal TotalDebts { get; set; }
    public decimal ClearedDebts { get; set; }
    public decimal RemainingDebts { get; set; }
    public List<Transaction> TopTransactions { get; set; } = new();
    public List<Transaction> PendingDebts { get; set; } = new();
}