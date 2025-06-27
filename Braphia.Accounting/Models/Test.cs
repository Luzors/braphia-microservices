using System.ComponentModel.DataAnnotations;

namespace Braphia.Accounting.Models
{
    public class Test
    {
        public Test() { }

        public Test(int rootId, int patientId, string testType, string description, decimal cost, DateTime completedDate)
        {
            RootId = rootId;
            PatientId = patientId;
            TestType = testType;
            Description = description;
            Cost = cost;
            CompletedDate = completedDate;
        }

        [Key]
        public int Id { get; set; }
        public int RootId { get; set; }
        public int PatientId { get; set; }
        public string TestType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public DateTime CompletedDate { get; set; }
        
        // Optioneel?
        public Patient? Patient { get; set; }
    }
}
