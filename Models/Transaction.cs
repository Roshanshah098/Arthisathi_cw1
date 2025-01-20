namespace arthiksathi.Models;

public class Transaction
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public DateTime? DueDate { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public bool IsCleared { get; set; }
    public List<string> Tags { get; set; } = new();
    public string Username { get; set; } = string.Empty;
}