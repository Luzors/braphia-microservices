using Braphia.UserManagement.Models.ExternalOnly;

namespace Braphia.UserManagement.Events.ConsumedEvents.Tests
{
    public class TestCompletedEvent
    {
        public Test Test { get; set; }

        public TestCompletedEvent() { }

        public TestCompletedEvent(Test test)
        {
            Test = test;
        }
    }
}
