namespace Braphia.NotificationDispatcher.Events.ExternalEvents.Pharmacies
{
    public class PharmacyRemovedEvent
    {
        public Models.Pharmacy Pharmacy { get; set; }

        public PharmacyRemovedEvent() { }

        public PharmacyRemovedEvent(Models.Pharmacy pharmacy)
        {
            Pharmacy = pharmacy ?? throw new ArgumentNullException(nameof(pharmacy), "Pharmacy cannot be null.");
        }
    }
}
