namespace Braphia.MedicalManagement.Events.Tests
{
    public class TestRequestedEvent
    {
        public Models.Test Test { get; set; }

        public TestRequestedEvent() { }

        public TestRequestedEvent(Models.Test test)
        {
            Test = test ?? throw new ArgumentNullException(nameof(test), "Test cannot be null.");
        }
    }
}