using UnityEngine;

namespace DiceFate.Units
{
    public interface IDamageable
    {
        int MaxHealth { get; }
        int CurrentHealth { get; }
        Transform Transform { get; }



        void TakeDamage(int damage, Vector3 positionUnit);
        void Die();

    }
}
