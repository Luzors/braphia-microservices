namespace Laboratory.Models
{
    public class Test
    {
        public Guid Id { get; set; }
        public string TestName { get; set; }
        public string Description { get; set; }
        public Enums.TestType TestType { get; set; }
        public Enums.TestStatus TestStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime FinishedAt { get; set; }
        public Guid CentralLaboratoryId { get; set; }
        public CentralLaboratory CentralLaboratory { get; set; }
        public Guid AppointmentId { get; set; }
        public Appointment Appointment { get; set; }

    }
}
