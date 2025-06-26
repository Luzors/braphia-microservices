namespace Braphia.UserManagement.Events
{
    public class ExternalUserFetchedEvent
    {
        public string ExternalUserData { get; set; }
        public ExternalUserFetchedEvent() { }
        public ExternalUserFetchedEvent(string externalUserData)
        {
            ExternalUserData = externalUserData ?? throw new ArgumentNullException(nameof(externalUserData), "External user data cannot be null.");
        }

    }
}
