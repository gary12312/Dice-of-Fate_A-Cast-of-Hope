using DiceFate.EventBus;
using DiceFate.Dice;

namespace DiceFate.Events
{
    public struct OnDropEvent : IEvent
    {
        public int TestValue { get; private set; }

        public OnDropEvent(int value)
        {
            TestValue = value;
        }
    }
}