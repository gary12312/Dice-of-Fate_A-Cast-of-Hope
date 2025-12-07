using DiceFate.EventBus;
using DiceFate.Dice;
using UnityEngine;

namespace DiceFate.Events
{
    public struct OnShakeEvent : IEvent
    {
        public Vector3 Point { get; private set; }

        public OnShakeEvent(Vector3 value)
        {
            Point = value;
        }
    }
}