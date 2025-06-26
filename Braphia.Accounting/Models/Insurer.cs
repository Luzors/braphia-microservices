using System.ComponentModel.DataAnnotations;

namespace Braphia.Accounting.Models
{
    public class Insurer
    {
        public Insurer() { }

        public Insurer(string name, string contactEmail, string contactPhone)
        {
            Name = name;
            ContactEmail = contactEmail;
            ContactPhone = contactPhone;
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public List<Patient> Patients { get; set; } = [];
        public List<Invoice> Invoices { get; set; } = [];
    }
}
