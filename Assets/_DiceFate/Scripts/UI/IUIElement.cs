using UnityEngine;

namespace DiceFate.UI
{
    public interface IUIElement<T>
    {
        void Enable(T item);
        void Disable();
    }
}