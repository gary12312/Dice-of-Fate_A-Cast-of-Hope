using DiceFate.EventBus;
using DiceFate.Dice;

namespace DiceFate.Events
{
    public struct OnMovmentValueEvent : IEvent
    {
        public int MoveValue { get; private set; }

        public OnMovmentValueEvent(int value)
        {
            MoveValue = value;
        }
    }
}