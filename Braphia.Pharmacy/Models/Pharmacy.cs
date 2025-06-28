using System.ComponentModel.DataAnnotations;

namespace Braphia.Pharmacy.Models
{
    public class Pharmacy
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public Pharmacy() { }

        public Pharmacy(string name, string address, string phoneNumber, string email)
        {
            Name = name;
            Address = address;
            PhoneNumber = phoneNumber;
            Email = email;
        }
    }
}
