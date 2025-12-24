using UnityEngine;

namespace DiceFate.Units
{
    public interface IMoveable
    {
        bool isIMoveable { get; }


        void MoveTo(Vector3 position);
        void StopMove();
    }
}
