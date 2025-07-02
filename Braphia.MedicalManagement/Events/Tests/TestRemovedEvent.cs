using Braphia.MedicalManagement.Models;

namespace Braphia.MedicalManagement.Events.Tests
{
    public class TestRemovedEvent
    {
        public Models.Test Test { get; set; }

        public TestRemovedEvent() { }
        
        public TestRemovedEvent(Models.Test test)
        {
            Test = test;
        }
    }
}
