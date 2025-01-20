namespace arthiksathi.Models;

public class AppState
{
    public User? CurrentUser { get; set; }
    public List<User> Users { get; set; } = new();
    public List<Transaction> Transactions { get; set; } = new();
    public List<string> DefaultTags { get; set; } = new()
    {
        "Yearly", "Monthly", "Food", "Drinks", "Clothes", 
        "Gadgets", "Miscellaneous", "Fuel", "Rent", "EMI", "Party"
    };
}