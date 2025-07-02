using Braphia.Laboratory.Models;

namespace Braphia.Laboratory.Events.Tests
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
