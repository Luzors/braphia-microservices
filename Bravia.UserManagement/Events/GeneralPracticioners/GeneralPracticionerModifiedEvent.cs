using Braphia.UserManagement.Models;

namespace Braphia.UserManagement.Events.GeneralPracticioners
{
    public class GeneralPracticionerModifiedEvent
    {
        public int GeneralPracticionerId { get; set; }
        public GeneralPracticioner GeneralPracticioner { get; set; }
        public GeneralPracticionerModifiedEvent() { }

        public GeneralPracticionerModifiedEvent(int generalPracticionerId, GeneralPracticioner generalPracticioner)
        {
            GeneralPracticionerId = generalPracticionerId;
            GeneralPracticioner = generalPracticioner ?? throw new ArgumentNullException(nameof(generalPracticioner), "General Practicioner cannot be null.");
        }
    }
}
