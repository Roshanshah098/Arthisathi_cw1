```

{
  "CurrentUser": {
    "Username": "currentlyLoggedInUser",
    "Password": "theirPassword",
    "PreferredCurrency": "USD"
  },
  "Users": [
    {
      "Username": "user1",
      "Password": "password1",
      "PreferredCurrency": "USD"
    },
    {
      "Username": "user2",
      "Password": "password2",
      "PreferredCurrency": "EUR"
    }
  ],
  "Transactions": [
    {
      "Id": "some-guid",
      "Type": 0,  // 0 = Credit, 1 = Debit, 2 = Debt
      "Amount": 100.00,
      "Date": "2024-01-16T00:00:00",
      "DueDate": null,
      "Notes": "Sample transaction",
      "Source": "Salary",
      "IsCleared": false,
      "Tags": ["Monthly", "Income"],
      "Username": "user1"
    }
  ],
  "DefaultTags": [
    "Yearly",
    "Monthly",
    "Food",
    "Drinks",
    "Clothes",
    "Gadgets",
    "Miscellaneous",
    "Fuel",
    "Rent",
    "EMI",
    "Party"
  ]
}

```