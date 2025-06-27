using System.ComponentModel.DataAnnotations;

namespace Braphia.Pharmacy.Models
{
    public class Medication
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Dosage { get; set; }
        public string Manufacturer { get; set; }
        public DateTime ExpiryDate { get; set; }

        public decimal Price { get; set; }

        public Medication(string name, string description, string dosage, string manufacturer, DateTime expiryDate, decimal price)
        {
            Name = name;
            Description = description;
            Dosage = dosage;
            Manufacturer = manufacturer;
            ExpiryDate = expiryDate;
            Price = price;
        }

        public Medication() { }

    }
}
