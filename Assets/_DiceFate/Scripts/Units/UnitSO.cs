using UnityEngine;

namespace DiceFate.Units
{
    [CreateAssetMenu(fileName = "New Unit", menuName = "DiceFate/Unit")]
    public class UnitSO : ScriptableObject
    {
        [field: SerializeField] public int Health { get; private set; } = 100;
        [field: SerializeField] public int Armor { get; private set; } = 10;

        [field: SerializeField] public Color colorSelected { get; private set; } = Color.white;


    }
}
