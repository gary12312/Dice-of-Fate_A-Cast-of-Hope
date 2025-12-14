using DiceFate.EventBus;
using DiceFate.Dice;
using UnityEngine;

namespace DiceFate.Events
{
    public struct OnShakeEvent : IEvent
    {
        public float Power { get; private set; }

        public OnShakeEvent(float value)
        {
            Power = value;
        }
    }
}