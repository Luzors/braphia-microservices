using Braphia.MedicalManagement.Models;

namespace Braphia.MedicalManagement.Events.Tests
{
    public class TestChangedEvent
    {
        public Models.Test Test { get; set; }

        public TestChangedEvent() { }
        
        public TestChangedEvent(Models.Test test)
        {
            Test = test;
        }
    }
}
