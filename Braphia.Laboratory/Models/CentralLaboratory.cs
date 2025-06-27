using System.ComponentModel.DataAnnotations;

namespace Braphia.Laboratory.Models
{
    public class CentralLaboratory
    {
        public CentralLaboratory() { }

        public CentralLaboratory(string laboratoryName, string address, string phoneNumber, string email)
        {
            LaboratoryName = laboratoryName;
            Address = address;
            PhoneNumber = phoneNumber;
            Email = email;
        }

        [Key]
        public int Id { get; set; }
        public string LaboratoryName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
