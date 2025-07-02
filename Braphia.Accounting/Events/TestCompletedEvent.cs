using Braphia.Accounting.Models;

namespace Braphia.Accounting.Events
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
