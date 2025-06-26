namespace Braphia.Laboratory.Models
{
    public class CentralLaboratory
    {
        public Guid Id { get; set; }
        public string LaboratoryName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public CentralLaboratory() { }

        public CentralLaboratory(string laboratoryName, string address, string phoneNumber, string email)
        {
            Id = Guid.NewGuid();
            LaboratoryName = laboratoryName;
            Address = address;
            PhoneNumber = phoneNumber;
            Email = email;
        }
    }
}
