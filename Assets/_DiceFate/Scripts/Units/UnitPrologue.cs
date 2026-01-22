using DiceFate.Loot;
using UnityEngine;

namespace DiceFate.Units
{
    public class UnitPrologue : MonoBehaviour, ISelectable, IHover
    {
        [field: SerializeField] public bool IsSelected { get; private set; }   // ISelectable
        [field: SerializeField] public bool IsHover { get; private set; }   // IHover


        [SerializeField] private ObjectOutline Outline;




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
            //if (lootTwinAnimation == null) { Debug.LogError($"Установить lootTwinAnimation для {this} "); }
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
        }
        public void OutlineOffSelected()
        {            
            Outline?.DisableOutline();
        }


        //-------------- ISelectable реализация --------------
        public void Select()
        {
           // lootTwinAnimation.OpenLootBox();
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
