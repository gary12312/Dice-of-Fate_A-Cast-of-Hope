using DiceFate.EventBus;
using DiceFate.Units;

namespace DiceFate.Events
{
    public struct  UnitSelectedEvent : IEvent
    {
        public ISelectable Unit { get; private set; }

        public UnitSelectedEvent(ISelectable unit)
        {
            Unit = unit;
        }
    }
}