using Braphia.Laboratory.Models;

namespace Braphia.Laboratory.Events
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
