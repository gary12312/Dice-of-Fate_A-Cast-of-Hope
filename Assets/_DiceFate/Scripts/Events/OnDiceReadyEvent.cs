using DiceFate.EventBus;
using DiceFate.Dice;

namespace DiceFate.Events
{
    public struct OnDiceReadyEvent : IEvent
    {
        public DiceCube Dice { get; private set; }
        public OnDiceReadyEvent(DiceCube dice)
        {
            Dice = dice;
        }
    }
}

