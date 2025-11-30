using UnityEngine;
using DiceFate.EventBus;
using DiceFate.Events;
using System;

namespace DiceFate.UI.UI_Actions
{

    public class ActionsUI : MonoBehaviour
    {
        [SerializeField] private UIActionsButtons[] actionsButtons;

        private void Awake()
        {
            Bus<UnitSelectedEvent>.OnEvent += HandelUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent += HandelUnitDeselected;
        }

        private void HandelUnitDeselected(UnitDeselectedEvent args)
        {
            throw new NotImplementedException();
        }

        private void HandelUnitSelected(UnitSelectedEvent args)
        {
            throw new NotImplementedException();
        }

        private void OnDestroy()
        {
            Bus<UnitSelectedEvent>.OnEvent -= HandelUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent -= HandelUnitDeselected; 
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
