using DiceFate.EventBus;
using DiceFate.Units;

namespace DiceFate.Events
{
    public struct UnitDeselectedEvent : IEvent
    {
        public ISelectable Unit { get; private set; }

        public UnitDeselectedEvent(ISelectable unit)
        {
            Unit = unit;
        }
    }
}
