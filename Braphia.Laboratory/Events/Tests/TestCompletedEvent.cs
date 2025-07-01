using Braphia.Laboratory.Models;

namespace Braphia.Laboratory.Events.Tests
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
