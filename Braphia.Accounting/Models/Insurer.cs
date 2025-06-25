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

        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        
        // TODO invoices? hier?
        public List<Invoice> Invoices { get; set; } = [];
    }
}
