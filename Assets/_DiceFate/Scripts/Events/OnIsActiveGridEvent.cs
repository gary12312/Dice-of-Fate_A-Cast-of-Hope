using DiceFate.EventBus;
using DiceFate.Dice;

namespace DiceFate.Events
{
    public class OnIsActiveGridEvent : IEvent
    {
        public bool isActiveGrid { get; private set; }

        public OnIsActiveGridEvent(bool isActive)
        {
            isActiveGrid = isActive;
        }

    }
}

