using Braphia.MedicalManagement.Models;

namespace Braphia.MedicalManagement.Events.Tests
{
    public class TestCompletedEvent
    {
        public Models.Test Test { get; set; }

        public TestCompletedEvent() { }
        
        public TestCompletedEvent(Models.Test test)
        {
            Test = test;
        }
    }
}
