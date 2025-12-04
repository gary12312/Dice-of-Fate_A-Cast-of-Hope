using DiceFate.EventBus;
using DiceFate.Units;

namespace DiceFate.Events
{
    public struct EnemyDeselectedEvent : IEvent
    {
        public ISelectable Enemy { get; private set; }

        public EnemyDeselectedEvent(ISelectable enemy)
        {
            Enemy = enemy;
        }
    }
}

