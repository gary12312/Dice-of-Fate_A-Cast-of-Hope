using DiceFate.EventBus;
using DiceFate.Events;
using UnityEngine;

namespace DiceFate.Units
{
    public class AbstractUnit : MonoBehaviour, ISelectable, IDamageable, IMoveable
    {

        [field: SerializeField] public bool IsSelected { get; private set; }   // ISelectable
        [field: SerializeField] public int CurrentHealth { get; private set; } // IDamageable
        [field: SerializeField] public int MaxHealth { get; private set; }     // IDamageable

        [SerializeField] private GameObject decal;
        [SerializeField] private UnitSO UnitSO;

        public Transform Transform => transform; // IDamageable


        protected virtual void Start()
        {
            MaxHealth = UnitSO.Health;
            CurrentHealth = MaxHealth;

            decal.SetActive(false);
        }

        //-------------- ISelectable реализация --------------
        public void Select()
        {
            Bus<UnitSelectedEvent>.Raise(new UnitSelectedEvent(this)); // Вызвать событие, изаписать себя как выбранный юнит слушает DF_PlayerInput

            decal.SetActive(true);

        }

        public void Deselect()
        {
            Bus<UnitDeselectedEvent>.Raise(new UnitDeselectedEvent(this)); // Вызвать событие, изаписать себя как отмененный юнит слушает DF_PlayerInput

            decal.SetActive(false);

        }

        //-------------- IDamageable реализация --------------
        public void TakeDamage(int damage)
        {
            int lastHealth = CurrentHealth;

            //  OnHealthUpdate?.Invoke(this, lastHealth, CurrentHealth);
            CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, CurrentHealth);
            if (CurrentHealth == 0)
            {
                Die();
            }
        }

        public void Die()
        {
            Destroy(gameObject);
        }

        // -------------- IMoveable реализация --------------
        public void MoveTo(Vector3 position)
        {
            throw new System.NotImplementedException();
            // graphAgent.SetVariableValue("TargetLocation", position);

        }

        public void StopMove()
        {
            throw new System.NotImplementedException();
        }





        public void Heal(int amount)  // разобраться
        {
            int lastHealth = CurrentHealth;
            CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
            // ---
        }


    }

}
