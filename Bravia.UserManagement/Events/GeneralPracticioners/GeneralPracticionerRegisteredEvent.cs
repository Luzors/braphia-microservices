using Braphia.UserManagement.Models;

namespace Braphia.UserManagement.Events.GeneralPracticioners
{
    public class GeneralPracticionerRegisteredEvent
    {
        public GeneralPracticioner GeneralPracticioner { get; set; }
        public GeneralPracticionerRegisteredEvent() { }

        public GeneralPracticionerRegisteredEvent(GeneralPracticioner generalPracticioner)
        {
            GeneralPracticioner = generalPracticioner ?? throw new ArgumentNullException(nameof(generalPracticioner), "GeneralPracticioner cannot be null.");
        }
    }
}
