using UnityEngine;
using DiceFate.EventBus;
using DiceFate.Events;


namespace DiceFate.Units  // пока стоп вместо него TestUnit
{

    public class DF_UnitManadger : MonoBehaviour, ISelectable
    {
        public GameObject decal;

        public bool IsSelected => throw new System.NotImplementedException();

        // public DF_Outline outline;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            decal.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void Select()
        {       
            Bus<UnitSelectedEvent>.Raise(new UnitSelectedEvent(this)); // Вызвать событие, изаписать себя как выбранный юнит слушает DF_PlayerInput

            decal.SetActive(true);
            // outline.EnableSelectOutline();
        }

        public void Deselect()
        {
            Bus<UnitDeselectedEvent>.Raise(new UnitDeselectedEvent(this)); // Вызвать событие, изаписать себя как отмененный юнит слушает DF_PlayerInput

            decal.SetActive(false);
            //outline.DisableOutline();
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
