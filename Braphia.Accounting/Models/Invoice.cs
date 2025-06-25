namespace Braphia.Accounting.Models
{
    public class Invoice
    {
        public Invoice() { }

        public Invoice(DateTime date, decimal amount, string description)
        { 
            Date = date;
            Amount = amount;
            Description = description;
        }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}
