using DiceFate.Units;
using UnityEngine;
using UnityEngine.UIElements;


namespace DiceFate.Loot
{
    public class LootOnGround : MonoBehaviour, ISelectable, IHover
    {
        [field: SerializeField] public bool IsSelected { get; private set; }   // ISelectable
        [field: SerializeField] public bool IsHover { get; private set; }   // IHover

        [SerializeField] private LootTwinAnimation lootTwinAnimation;
        [SerializeField] private ObjectOutline Outline; //обводка



        private void Start()
        {
            InitializationСheck();
            InitializationStart();
        }


        private void InitializationStart()
        {                 
            Outline?.DisableOutline();                // 2. отключить Обводку          
        }

     
        private void InitializationСheck()
        {
            if (lootTwinAnimation == null) { Debug.LogError($"Установить lootTwinAnimation для {this} "); }
            if (Outline == null) { Debug.LogError($"Установить Outline для {this} "); }

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
            lootTwinAnimation.OpenLootBox();
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
    }
}
