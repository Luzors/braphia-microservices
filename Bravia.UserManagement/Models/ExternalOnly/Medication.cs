using System.ComponentModel.DataAnnotations;

namespace Braphia.UserManagement.Models.ExternalOnly
{
    public class Medication
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public Medication(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        public Medication() { }

    }
}
