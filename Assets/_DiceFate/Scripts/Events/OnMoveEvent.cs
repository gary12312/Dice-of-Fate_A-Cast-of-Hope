using DiceFate.EventBus;
using DiceFate.Dice;

namespace DiceFate.Events
{
    public struct OnMoveEvent : IEvent
    {
        public int TestValue { get; private set; }

        public OnMoveEvent(int value)
        {
            TestValue = value;
        }
    }
}