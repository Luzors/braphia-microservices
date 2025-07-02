using Braphia.Accounting.EventSourcing;

namespace Braphia.Accounting.Events
{
    public class InvoiceAmountAdjustedEvent : BaseEvent
    {
        public decimal AdjustmentAmount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public int InsurerId { get; set; }

        public InvoiceAmountAdjustedEvent()
        {
            EventType = "InvoiceAmountAdjusted";
        }

        public InvoiceAmountAdjustedEvent(int aggregateId, int version, decimal adjustmentAmount, string reason, string reference, int insurerId)
            : base(aggregateId, version)
        {
            AdjustmentAmount = adjustmentAmount;
            Reason = reason ?? string.Empty;
            Reference = reference ?? string.Empty;
            InsurerId = insurerId;
            EventType = "InvoiceAmountAdjusted";
        }
    }
}
