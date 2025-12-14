using DiceFate.EventBus;
using DiceFate.Dice;
using UnityEngine;

namespace DiceFate.Events
{
    public struct OnMoveToMouseEvent : IEvent
    {
        public Vector3 Point { get; private set; }

        public OnMoveToMouseEvent(Vector3 value)
        {
            Point = value;
        }
    }
}