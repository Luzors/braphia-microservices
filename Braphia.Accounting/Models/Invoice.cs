using System.ComponentModel.DataAnnotations;

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

        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public int PatientId { get; set; }
        public int InsurerId { get; set; }
    }
}
