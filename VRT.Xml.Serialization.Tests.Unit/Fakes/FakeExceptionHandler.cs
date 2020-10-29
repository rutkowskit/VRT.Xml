using System;
using VRT.Xml.Serialization.Handlers;

namespace VRT.Xml.Serialization.Fakes
{
    internal class FakeExceptionHandler : IExceptionHandler
    {
        private readonly bool _simulateHandled;

        public FakeExceptionHandler(bool simulateHandled)
        {
            _simulateHandled = simulateHandled;
        }
        public int ExceptionHandledCount { get; private set; }
        public bool Handle(Exception ex)
        {
            ExceptionHandledCount++;
            return _simulateHandled;
        }
    }
}
