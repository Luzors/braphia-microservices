namespace Braphia.UserManagement.Models
{
    public class MedicalRecord
    {
        public MedicalRecord() { }

        public MedicalRecord(string description, DateTime date)
        {
            Description = description;
            Date = date;
        }

        public int Id { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }
    }
}
