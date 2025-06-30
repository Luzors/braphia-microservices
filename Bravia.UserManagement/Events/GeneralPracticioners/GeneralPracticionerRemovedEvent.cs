using Braphia.UserManagement.Models;

namespace Braphia.UserManagement.Events.GeneralPracticioners
{
    public class GeneralPracticionerRemovedEvent
    {
        public GeneralPracticioner GeneralPracticioner { get; set; }
        public GeneralPracticionerRemovedEvent() { }

        public GeneralPracticionerRemovedEvent(GeneralPracticioner generalPracticioner)
        {
            GeneralPracticioner = generalPracticioner ?? throw new ArgumentNullException(nameof(generalPracticioner), "GeneralPracticioner cannot be null.");
        }
    }
}
