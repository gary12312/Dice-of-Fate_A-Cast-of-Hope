using DiceFate.EventBus;
using DiceFate.Units;

namespace DiceFate.Events
{
    public struct OnSelectedForUiEvent : IEvent
    {
        public ISelectableForVisibleUi ObjectOnScene { get; private set; }

        public OnSelectedForUiEvent(ISelectableForVisibleUi objectOnScene)
        {
            ObjectOnScene = objectOnScene;
        }
    }
}