using Braphia.MedicalManagement.Models;

namespace Braphia.MedicalManagement.Events
{
    public class TestRequestedEvent
    {
        public Test Test { get; set; }

        public TestRequestedEvent() { }

        public TestRequestedEvent(Test test)
        {
            Test = test ?? throw new ArgumentNullException(nameof(test), "Test cannot be null.");
        }
    }
}