using DiceFate.EventBus;
using DiceFate.Dice;

namespace DiceFate.Events
{
    public struct OnResetDiceResultsEvent : IEvent
    {
        public int TestValue { get; private set; }

        public OnResetDiceResultsEvent(int value)
        {
            TestValue = value;
        }
    }
}