namespace Braphia.UserManagement.Events
{
    /// <summary>
    /// This is an event that is published by the backgoroundservice that gets the external csv.
    /// These events are probably only consumed by the consumer within this usermanagement.
    /// </summary>
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
