using Braphia.NotificationDispatcher.Models.OutOfDb;

namespace Braphia.NotificationDispatcher.Events.ExternalEvents
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
