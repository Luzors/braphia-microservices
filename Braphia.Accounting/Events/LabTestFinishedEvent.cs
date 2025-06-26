namespace Braphia.Accounting.Events
{
    public class LabTestFinishedEvent
    {
        public int LabTestId { get; set; }
        public int PatientId { get; set; }
        public string TestType { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public DateTime CompletedDate { get; set; }
        public string Description { get; set; } = string.Empty;

        public LabTestFinishedEvent() { }

        public LabTestFinishedEvent(int labTestId, int patientId, string testType, decimal cost, DateTime completedDate, string description = "")
        {
            LabTestId = labTestId;
            PatientId = patientId;
            TestType = testType;
            Cost = cost;
            CompletedDate = completedDate;
            Description = description;
        }
    }
}
