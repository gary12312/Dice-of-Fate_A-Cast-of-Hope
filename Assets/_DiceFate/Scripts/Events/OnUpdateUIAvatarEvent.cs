using DiceFate.EventBus;
using DiceFate.Dice;

namespace DiceFate.Events
{
    public struct OnUpdateUIAvatarEvent : IEvent
    {
        public int TestValue { get; private set; }

        public OnUpdateUIAvatarEvent(int value)
        {
            TestValue = value;
        }
    }
}