using DiceFate.EventBus;

namespace DiceFate.Events
{
    public struct OnTooltipHoverEvent : IEvent
    {
        public string NameTool { get; private set; }

        public OnTooltipHoverEvent(string value)
        {
            NameTool = value;
        }
    }
}