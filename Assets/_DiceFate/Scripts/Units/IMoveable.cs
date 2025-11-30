using UnityEngine;

namespace DiceFate.Units
{
    public interface IMoveable
    {
        void MoveTo(Vector3 position);
        void StopMove();
    }
}
