using DiceFate.EventBus;
using DiceFate.Dice;

namespace DiceFate.Events
{
    public class OnGridEvent : IEvent
    {
        public int MovingNumber { get; private set; }

        public OnGridEvent(int number)
        {
            MovingNumber = number;
        }

    }
}

