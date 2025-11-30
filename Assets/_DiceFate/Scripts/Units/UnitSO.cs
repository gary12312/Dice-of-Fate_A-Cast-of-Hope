using UnityEngine;

namespace DiceFate.Units
{
    [CreateAssetMenu(fileName = "New Unit", menuName = "DiceFate/Unit")]
    public class UnitSO : ScriptableObject
    {
        [field: SerializeField] public int Health { get; private set; } = 100;
    }
}
