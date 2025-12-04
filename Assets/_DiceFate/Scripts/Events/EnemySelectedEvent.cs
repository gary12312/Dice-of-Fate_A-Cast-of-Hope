using DiceFate.EventBus;
using DiceFate.Units;

namespace DiceFate.Events
{

    public struct EnemySelectedEvent : IEvent
    {
        public ISelectable Enemy { get; private set; }

        public EnemySelectedEvent(ISelectable enemy)
        {
            this.Enemy = enemy;
        }
    }
}
