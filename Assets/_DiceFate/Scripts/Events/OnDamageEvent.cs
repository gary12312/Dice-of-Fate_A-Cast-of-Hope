using DiceFate.EventBus;
using DiceFate.Units;

namespace DiceFate.Events
{
    public struct OnDamageEvent : IEvent
    {
        public IDamageable Damage { get; private set; }

        public OnDamageEvent(IDamageable damageObject)
        {
            this.Damage = damageObject;
        }



        //public ISelectable Enemy { get; private set; }

        //public EnemySelectedEvent(ISelectable enemy)
        //{
        //    this.Enemy = enemy;
        //}
    }

}