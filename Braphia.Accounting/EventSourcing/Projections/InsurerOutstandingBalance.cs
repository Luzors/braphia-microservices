using System.ComponentModel.DataAnnotations;

namespace Braphia.Accounting.EventSourcing.Projections
{
    public class InsurerOutstandingBalance
    {
        [Key]
        public int InsurerId { get; set; }
        public string InsurerName { get; set; } = string.Empty;
        public decimal TotalOutstanding { get; set; }
        public int OutstandingInvoiceCount { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
