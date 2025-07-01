using System.ComponentModel.DataAnnotations;

namespace Braphia.Accounting.EventSourcing.Projections
{
    public class InvoiceProjection
    {
        [Key]
        public Guid InvoiceAggregateId { get; set; }
        public int PatientId { get; set; }
        public int InsurerId { get; set; }
        public int LabTestId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountOutstanding { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public bool IsFullyPaid { get; set; }
        
        // Navigation properties for easier querying
        public string PatientName { get; set; } = string.Empty;
        public string InsurerName { get; set; } = string.Empty;
    }
}
