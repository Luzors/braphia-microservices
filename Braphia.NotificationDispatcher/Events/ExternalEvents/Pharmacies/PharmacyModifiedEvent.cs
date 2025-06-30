namespace Braphia.NotificationDispatcher.Events.ExternalEvents.Pharmacies
{
    public class PharmacyModifiedEvent
    {
        public int PharmacyId { get; set; }
        public Models.Pharmacy Pharmacy { get; set; }
        public PharmacyModifiedEvent() { }
        public PharmacyModifiedEvent(int pharmacyId, Models.Pharmacy pharmacy)
        {
            PharmacyId = pharmacyId;
            Pharmacy = pharmacy ?? throw new ArgumentNullException(nameof(pharmacy), "Pharmacy cannot be null.");
        }
    }
}
