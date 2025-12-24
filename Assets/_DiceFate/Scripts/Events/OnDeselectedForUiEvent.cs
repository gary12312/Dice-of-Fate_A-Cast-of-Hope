using DiceFate.EventBus;
using DiceFate.Units;

namespace DiceFate.Events
{
    public struct OnDeselectedForUiEvent : IEvent
    {
        public ISelectableForVisibleUi ObjectOnScene { get; private set; }

        public OnDeselectedForUiEvent(ISelectableForVisibleUi objectOnScene)
        {
            ObjectOnScene = objectOnScene;
        }
    }
}