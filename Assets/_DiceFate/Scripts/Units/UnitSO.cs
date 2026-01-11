using UnityEngine;

namespace DiceFate.Units
{
    [CreateAssetMenu(fileName = "New Unit", menuName = "DiceFate/Unit")]
    public class UnitSO : ScriptableObject
    {
        [field: SerializeField] public string Unit { get; private set; } = "Player"; // Player, Enemy, Other
        [field: SerializeField] public string NameUnit { get; private set; } = "имя";
        [field: SerializeField] public GameObject PrefabAvatarBoard { get; private set; }
        [field: SerializeField] public int Health { get; private set; } = 20;
        [field: SerializeField] public int DistanceToAttack { get; private set; } = 3;
        [field: SerializeField] public int Move { get; private set; } = 3;
        [field: SerializeField] public int Attack { get; private set; } = 4;
        [field: SerializeField] public int Shild { get; private set; } = 2;
        [field: SerializeField] public int ConterAttack { get; private set; } = 2;
        [field: SerializeField] public bool Protection { get; private set; } = true; // Может защищаться       

        [field: SerializeField] public Color colorSelected { get; private set; } = Color.white;


    }
}
