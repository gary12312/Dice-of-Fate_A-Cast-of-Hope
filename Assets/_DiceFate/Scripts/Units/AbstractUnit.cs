using DiceFate.EventBus;
using DiceFate.Events;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace DiceFate.Units
{
    public class AbstractUnit : MonoBehaviour, ISelectable, IDamageable, IMoveable, IHover
    {

        [field: SerializeField] public bool IsSelected { get; private set; }   // ISelectable
        [field: SerializeField] public int CurrentHealth { get; private set; } // IDamageable
        [field: SerializeField] public int MaxHealth { get; private set; }     // IDamageable

        [SerializeField] private GameObject decal;
        [SerializeField] private UnitSO UnitSO;
        [SerializeField] private ObjectOutline Outline; //обводка

        public Transform Transform => transform; // IDamageable


        protected virtual void Start()
        {
            InitializationСheck();
            InitializationStart();
        }
       
        //Проверки на null
       private void  InitializationСheck()
        {
            if (decal == null) { Debug.LogError($"Установить decal для {this} "); }
            if (UnitSO == null) { Debug.LogError($"Установить UnitSO для {this} "); }
            if (Outline == null) { Debug.LogError($"Установить Outline для {this} "); }
        }

        // Настройка при запуске 
        private void InitializationStart()
        {       
            decal.SetActive(false);                   // 1. отключть Decal        
            Outline?.DisableOutline();                // 2. отключить Обводку       
          //  Outline?.ChangeColorOutline(Color.green); // 3. Назначить цвет для обводки при выделении / цвет для обводки при наведении это цвет настроенный по умолчанию/
            MaxHealth = UnitSO.Health;                // 4. Назначить Здоровье
            CurrentHealth = MaxHealth;
    

        }


        //-------------- ISelectable реализация --------------
        public void Select()
        {
            Bus<UnitSelectedEvent>.Raise(new UnitSelectedEvent(this)); // Вызвать событие, изаписать себя как выбранный юнит слушает DF_PlayerInput

            decal.SetActive(true);
           
            OutlineOnSelected();
        }

        public void Deselect()
        {
            Bus<UnitDeselectedEvent>.Raise(new UnitDeselectedEvent(this)); // Вызвать событие, изаписать себя как отмененный юнит слушает DF_PlayerInput

            decal.SetActive(false);

            OutlineOffSelected();  
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



        //-------------- Управление обводкой юнита --------------
        public void OnEnterHover()
        {
            //if ( )
            //{
                
            //}
            Outline?.EnableOutline();
        }
        public void OnExitHover()
        {
            Outline?.DisableOutline();
        }
        public void OutlineOnSelected()
        {
            Outline?.EnableOutline();
            Outline?.ChangeColorOutline(UnitSO.colorSelected);
        }
        public void OutlineOffSelected()
        {
           Outline?.DefaultColorOutline();
           Outline?.DisableOutline();
        }
    }
}
