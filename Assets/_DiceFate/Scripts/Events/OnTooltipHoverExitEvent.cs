using DiceFate.EventBus;

namespace DiceFate.Events
{
    public struct OnTooltipHoverExitEvent : IEvent
    {
        public string NameTool { get; private set; }

        public OnTooltipHoverExitEvent(string value)
        {
            NameTool = value; ;
        }
    }
}