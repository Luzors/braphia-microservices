namespace Braphia.Pharmacy.Events
{
    public class PharmacyRegisteredEvent
    {
        public Models.Pharmacy Pharmacy { get; set; }
        public PharmacyRegisteredEvent() { }
        public PharmacyRegisteredEvent(Models.Pharmacy pharmacy)
        {
            Pharmacy = pharmacy ?? throw new ArgumentNullException(nameof(pharmacy), "Pharmacy cannot be null.");
        }
    }
}
