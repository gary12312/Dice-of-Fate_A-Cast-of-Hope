using DiceFate.EventBus;
using DiceFate.Events;
using System;
using UnityEngine;
using DiceFate.UI.UI_Actions;

namespace DiceFate.UI
{
    public class RuntimeUI : MonoBehaviour
    {
        private ActionsUI actionsUI;


        //    private void Awake()
        //    {
        //        Bus<UnitSelectedEvent>.OnEvent += HandelUnitSelected;
        //        Bus<UnitDeselectedEvent>.OnEvent += HandelUnitDeselected;
        //    }



        //    private void OnDestroy()
        //    {
        //        Bus<UnitSelectedEvent>.OnEvent -= HandelUnitSelected;
        //        Bus<UnitDeselectedEvent>.OnEvent -= HandelUnitDeselected;
        //    }
    }
}

