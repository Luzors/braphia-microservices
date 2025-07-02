using System.ComponentModel.DataAnnotations;
using Braphia.Laboratory.Enums;

namespace Braphia.Laboratory.Models
{
    public class Test
    {
        public Test() { }

        public Test(int patientId, TestType testType, string description, decimal cost, DateTime completedDate)
        {
            PatientId = patientId;
            TestType = testType;
            Description = description;
            Cost = cost;
            CompletedDate = completedDate;
        }


        [Key]
        public int Id { get; set; }
        public int PatientId { get; set; }
        public TestType TestType { get; set; } 
        public string Description { get; set; } = string.Empty;
        public string? Result { get; set; } = null;
        public decimal Cost { get; set; }
        public DateTime? CompletedDate { get; set; } = null;
        
        // Optioneel?
        //public Patient? Patient { get; set; }

    }
}
