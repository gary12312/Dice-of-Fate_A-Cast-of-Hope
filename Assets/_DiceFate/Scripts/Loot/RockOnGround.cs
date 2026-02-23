using DiceFate.Units;
using UnityEngine;
using DiceFate.EventBus;
using DiceFate.Events;


namespace DiceFate.Loot
{
    public class RockOnGround : MonoBehaviour, ISelectable, IHover
    {
        [field: SerializeField] public bool IsSelected { get; private set; }   // ISelectable
        [field: SerializeField] public bool IsHover { get; private set; }   // IHover

        [SerializeField] private GameObject particle;
        [SerializeField] private string nameRock;

        //[SerializeField] private LootTwinAnimation lootTwinAnimation;
        [SerializeField] private ObjectOutline Outline; //обводка

        private bool isActivate;


        private void Start()
        {
            InitializationСheck();
            InitializationStart();
        }


        private void InitializationStart()
        {
            isActivate = false;
            particle.SetActive(false);
            Outline?.DisableOutline();                // 2. отключить Обводку          
        }


        public void ActivateRock()
        {
            isActivate = true;
            particle.SetActive(true);
        }



        //-------------- IHover реализация --------------

        public void OnEnterHover()
        {
            if (isActivate)
            {
                if (IsSelected == false) { Outline?.EnableOutline(); }
                IsHover = true;

                Bus<OnTooltipHoverEvent>.Raise(new OnTooltipHoverEvent(nameRock));
            }
        }
        public void OnExitHover()
        {
            if (isActivate)
            {
                if (IsSelected == false) { Outline?.DisableOutline(); }
                IsHover = false;

                Bus<OnTooltipHoverExitEvent>.Raise(new OnTooltipHoverExitEvent(nameRock));
            }
        }
        //-------------- Управление обводкой юнита --------------
        public void OutlineOnSelected()
        {
            Outline?.EnableOutline();
            // Outline?.ChangeColorOutline(UnitSO.colorSelected);
        }
        public void OutlineOffSelected()
        {
            // Outline?.ResetColorOutline();
            Outline?.DisableOutline();
        }




        //-------------- ISelectable реализация --------------
        public void Select()
        {
            //if (lootTwinAnimation != null)
            //{
            //    lootTwinAnimation.OpenLoot();
            //}
        }
        public void Deselect()
        {
            throw new System.NotImplementedException();
        }

        public void JumpBeforeAttack()
        {
            throw new System.NotImplementedException();
        }

        public void UnitsSOCurrent(string userName)
        {
            throw new System.NotImplementedException();
        }

        private void InitializationСheck()
        {
            //if (lootTwinAnimation == null) { Debug.LogError($"Установить lootTwinAnimation для {this} "); }
            if (Outline == null) { Debug.LogError($"Установить Outline для {this} "); }
        }

    }
}
