using Braphia.Laboratory.Models;

namespace Braphia.Laboratory.Events.Tests
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
