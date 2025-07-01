using System.ComponentModel.DataAnnotations;

namespace Braphia.Accounting.EventSourcing.Projections
{
    public class PatientInsurerEventHistory
    {
        [Key]
        public long Id { get; set; }
        
        public int PatientId { get; set; }
        public int InsurerId { get; set; }
        public Guid InvoiceAggregateId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string EventDescription { get; set; } = string.Empty;
        public decimal? Amount { get; set; }
        public DateTime EventOccurredOn { get; set; }
        
        // Aggregated totals per patient-insurer combination
        public decimal TotalInvoiced { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalOutstanding { get; set; }
        public int TotalInvoiceCount { get; set; }
        public int PaidInvoiceCount { get; set; }
        public DateTime LastActivityDate { get; set; }
        
        // Navigation properties for easier querying
        public string PatientName { get; set; } = string.Empty;
        public string InsurerName { get; set; } = string.Empty;
    }
} 