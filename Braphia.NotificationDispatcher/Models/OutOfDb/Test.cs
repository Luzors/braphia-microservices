using System.ComponentModel.DataAnnotations;

namespace Braphia.NotificationDispatcher.Models.OutOfDb
{
    public class Test
    {
        public Test() { }

        public Test(int patientId, TestType testType, string description, DateTime completedDate)
        {
            PatientId = patientId;
            TestType = testType;
            Description = description;
            CompletedDate = completedDate;
        }

        [Key]
        public int Id { get; set; }
        public int PatientId { get; set; }
        public TestType TestType { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public DateTime CompletedDate { get; set; }
    }
}
