using DiceFate.EventBus;
using DiceFate.Events;
using DiceFate.Player;
using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace DiceFate.Units
{
    public class AbstractUnit : MonoBehaviour, ISelectable, IDamageable, IHover, ISelectableForVisibleUi
    {

        [field: SerializeField] public bool IsSelected { get; private set; }   // ISelectable
        [field: SerializeField] public bool IsSelectedForVisibleUi { get; private set; }   // IsSelectedForVisibleUi
        [field: SerializeField] public int CurrentHealth { get; private set; } // IDamageable
        [field: SerializeField] public int MaxHealth { get; private set; }     // IDamageable

        [SerializeField] public string unitType;

        [field: SerializeField] public GameObject avatarPrefab; // Префаб аватара для UI
        [field: SerializeField] public GameObject vfxLevelUP; // VFX Level UP
        [field: SerializeField] public bool IsHover { get; private set; }   // IHover

        // private string unit;          // player, enemy, other
        private string nameUnit;
        private int distanceToAttack; // Расстояние атаки юнита
        private int move;
        private int attack;
        private int shild;
        private int conterAttack;


        [SerializeField] private GameObject detectorZone;
        [SerializeField] private GameObject objectTestUi;

        //[SerializeField] private UnitHoverBeforAttackController unitHoverBeforAttackController;
        [SerializeField] private TweenAnimationUnit TweenAnimationUnit;
        // [SerializeField] private JumpOnSpot jumpOnSpot;   // или tweenAnimationUnit или jumpOnSpot
        [SerializeField] private UnitSO UnitSO;
        [SerializeField] private ObjectOutline Outline; //обводка

        public Transform Transform => transform; // IDamageable

        protected bool isTargeted = false; // Начался ли юнит быть атакованным

        protected virtual void Start()
        {
            InitializationСheck();
            InitializationStart();
        }

        //Проверки на null
        private void InitializationСheck()
        {
            if (detectorZone == null) { Debug.LogError($"Установить decal для {this} "); }
            if (UnitSO == null) { Debug.LogError($"Установить UnitSO для {this} "); }
            if (Outline == null) { Debug.LogError($"Установить Outline для {this} "); }
            if (TweenAnimationUnit == null) { Debug.LogError($"Установить TweenAnimationUnit для {this} "); }
        }

        // Настройка при запуске 
        private void InitializationStart()
        {
            detectorZone.SetActive(false);            // 1. отключть Decal        
            Outline?.DisableOutline();                // 2. отключить Обводку
            VFXLevelUpSetActive(false);
            //  Outline?.ChangeColorOutline(Color.green); // 3. Назначить цвет для обводки при выделении / цвет для обводки при наведении это цвет настроенный по умолчанию/
            MaxHealth = UnitSO.Health;                // 4. Назначить Здоровье
            CurrentHealth = MaxHealth;

            unitType = UnitSO.UnitType;
            nameUnit = UnitSO.NameUnit;
            distanceToAttack = UnitSO.DistanceToAttack; // 5. Назначить Расстояние атаки
            move = UnitSO.Move;
            attack = UnitSO.Attack;
            shild = UnitSO.Shild;
            conterAttack = UnitSO.ConterAttack;

        }


        //-------------- ISelectable реализация --------------
        public void Select()
        {
            RecValueInGameStaticForUIMane();

            ReadyToMove();

            Bus<UnitSelectedEvent>.Raise(new UnitSelectedEvent(this)); // Вызвать событие, изаписать себя как выбранный юнит слушает DF_PlayerInput
        }

        public void UnitsSOCurrent(string userName)
        {
            throw new System.NotImplementedException();
        }

        public void Deselect()
        {
            Bus<UnitDeselectedEvent>.Raise(new UnitDeselectedEvent(this)); // Вызвать событие, изаписать себя как отмененный юнит слушает DF_PlayerInput

            detectorZone.SetActive(false);

            OutlineOffSelected();

            IsSelected = false;
        }
        public void JumpBeforeAttack()
        {
            // jumpOnSpot.StartJump();
            TweenAnimationUnit.JampAnimation();
        }

        //-------------- ISelectableForVisibleUi реализация --------------
        public void SelectForUi()
        {
            Bus<OnSelectedForUiEvent>.Raise(new OnSelectedForUiEvent(this));
            IsSelectedForVisibleUi = true;
            objectTestUi.SetActive(true);

        }

        public void DeselectForUi()
        {
            Bus<OnSelectedForUiEvent>.Raise(new OnSelectedForUiEvent(this));
            IsSelectedForVisibleUi = false;
            objectTestUi.SetActive(false);
        }



        //-------------- IDamageable реализация --------------
        public void TakeDamage(int damage, Vector3 positionUnit)
        {
            isTargeted = true;

            if (UnitSO.Protection)   // Для босса или игрока  Дополнительная защита перед атакой
            {
                ProtectedBeforeToAttack();
            }

            int lastHealth = CurrentHealth;
            CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, CurrentHealth);
            //  OnHealthUpdate?.Invoke(this, lastHealth, CurrentHealth);

            Bus<OnDamageEvent>.Raise(new OnDamageEvent(this));

            if (CurrentHealth != 0)
            {
                StartCoroutine(TweenHitAnamation(positionUnit));
            }
            else
            {
                Outline?.DisableOutline();
                Die();
            }
        }

        private IEnumerator TweenHitAnamation(Vector3 positionUnit)
        {
            yield return new WaitForSeconds(0.5f);

            TweenAnimationUnit.TargetHitAnimation(positionUnit);
        }

        protected virtual void ProtectedBeforeToAttack()
        { } // Базовая реализация ничего не делает, Дочерние классы могут переопределить этот метод




        public void Heal(int amount)  // разобраться
        {
            int lastHealth = CurrentHealth;
            CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);

            // ---
        }

        //-------------- Размер декали --------------



        //-------------- Основные методы --------------

        public void Die() => Destroy(gameObject);

        private void RecValueInGameStaticForUIMane()
        {
            GameStats.currentUser = unitType;
            GameStats.nameUnit = nameUnit;
            GameStats.moveUser = move;
            GameStats.attackUser = attack;
            GameStats.shildUser = shild;
            GameStats.conterAttackUser = conterAttack;
            Debug.Log("Запись завершена");
        }


        //-------------- IHover реализация --------------

        public void OnEnterHover()
        {
            if (IsSelected == false) { Outline?.EnableOutline(); }
            IsHover = true;

        }
        public void OnExitHover()
        {
            if (IsSelected == false) { Outline?.DisableOutline(); }
            IsHover = false;
        }
        //-------------- Управление обводкой юнита --------------
        public void OutlineOnSelected()
        {
            Outline?.EnableOutline();
            Outline?.ChangeColorOutline(UnitSO.colorSelected);
        }
        public void OutlineOffSelected()
        {
            Outline?.ResetColorOutline();
            Outline?.DisableOutline();
        }

        //-------------- Фазы --------------
        private void ReadyToMove()
        {
            detectorZone.SetActive(true);

            OutlineOnSelected();

            IsSelected = true;
        }


        // -------------- Вспомогательные методы --------------
        private void OnDestroy()
        {

            //   Bus<UnitDeadEvent>.Raise(new UnitDeadEvent(this));

        }


        public void VFXLevelUpStart(float delay) => StartCoroutine(VfxLevelUp(delay));     
        private IEnumerator VfxLevelUp(float delay)
        {
            yield return new WaitForSeconds(delay);
            VFXLevelUpSetActive(true);

            yield return new WaitForSeconds(3f);
            VFXLevelUpSetActive(false);
        }

        private void VFXLevelUpSetActive(bool isActive)
        {
            if (vfxLevelUP !=null) 
            {
                vfxLevelUP.SetActive(isActive);
            }
        }

    }
}
