using UnityEngine;
using UnityEngine.UI;
using DiceFate.Units;
using DiceFate.Events;
using DiceFate.EventBus;

namespace DiceFate.UI
{

    public class UI_ButtonCancel : MonoBehaviour
    {
        [SerializeField] private Button button;

        private ISelectable selectedUnit; // Текущий выделенный юнит который имеет интерфейс ISelectable

        private void Awake()
        {
            
            button.onClick.AddListener(OnButtonClick); // подписывается на щелчок

            Bus<UnitSelectedEvent>.OnEvent += HandelUnitSelected;
            //Bus<UnitDeselectedEvent>.OnEvent += HandeleUnitDeselect;
            Bus<EnemySelectedEvent>.OnEvent += HandelEnemySelected;
           // Bus<EnemyDeselectedEvent>.OnEvent += HandelEnemyDeselected;
        }
        private void OnDestroy()
        {
            button?.onClick.RemoveListener(OnButtonClick);

            Bus<UnitSelectedEvent>.OnEvent -= HandelUnitSelected;
           // Bus<UnitDeselectedEvent>.OnEvent -= HandeleUnitDeselect;
            Bus<EnemySelectedEvent>.OnEvent -= HandelEnemySelected;
           // Bus<EnemyDeselectedEvent>.OnEvent -= HandelEnemyDeselected;
        }

        void Update()
        {
            // Проверяем нажатие клавиши Q в каждом кадре 
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (selectedUnit != null)
                {
                    //  Bus<UnitDeselectedEvent>.Raise(new UnitDeselectedEvent(selectedUnit));
                    selectedUnit.Deselect(); // Отменить выделение юнита

                }
            }
        }



        //---------------------------------- Выбор и выделение   ----------------------------------
        private void HandelUnitSelected(UnitSelectedEvent evt)
        {
            selectedUnit = evt.Unit; // Обновить текущий выделенный юнит
        }

        private void HandeleUnitDeselect(UnitDeselectedEvent evt)
        {
            selectedUnit = null;  // Сбросить текущий выделенный юнит          
        }

        private void HandelEnemySelected(EnemySelectedEvent evt)
        {
            selectedUnit = evt.Enemy; // Обновить текущий выделенный Враг
        }
        private void HandelEnemyDeselected(EnemyDeselectedEvent evt)
        {
            selectedUnit = null;  // Сбросить текущий выделенный Враг          
        }

        //---------------------------------- Логика   ----------------------------------
        
        private void OnButtonClick()
        {
            if (selectedUnit != null)
            {
                Bus<UnitDeselectedEvent>.Raise(new UnitDeselectedEvent(selectedUnit));

            }
        }


    }
}
