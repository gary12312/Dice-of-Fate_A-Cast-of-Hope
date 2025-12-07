using DiceFate.EventBus;
using DiceFate.Dice;

namespace DiceFate.Events
{
    public struct OnTestingEvent : IEvent
    {
        public int TestValue { get; private set; }

        public OnTestingEvent(int value)
        {
            TestValue = value;
        }
    }
}