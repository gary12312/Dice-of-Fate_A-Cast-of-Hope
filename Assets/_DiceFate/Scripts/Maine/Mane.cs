using Unity.Cinemachine;
using UnityEngine;
using DiceFate.Units;
using DiceFate.Events;
using DiceFate.EventBus;


namespace DiceFate.Maine
{
    public class Mane : MonoBehaviour
    {


        private ISelectable selectedUnit; // Текущий выделенный юнит который имеет интерфейс ISelectable
        private ISelectable selectedEnemy; // Текущий выделенный юнит который имеет интерфейс ISelectable


        private void Awake()
        {
            Bus<UnitSelectedEvent>.OnEvent += HandelUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent += HandeleUnitDeselect;
            Bus<EnemySelectedEvent>.OnEvent += HandelEnemySelected;
            Bus<EnemyDeselectedEvent>.OnEvent += HandelEnemyDeselected;
        }
        private void OnDestroy()
        {
            Bus<UnitSelectedEvent>.OnEvent -= HandelUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent -= HandeleUnitDeselect;
            Bus<EnemySelectedEvent>.OnEvent -= HandelEnemySelected;
            Bus<EnemyDeselectedEvent>.OnEvent -= HandelEnemyDeselected;
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
            selectedEnemy = evt.Enemy; // Обновить текущий выделенный Враг
        }
        private void HandelEnemyDeselected(EnemyDeselectedEvent evt)
        {
            selectedEnemy = null;  // Сбросить текущий выделенный Враг          
        }




        //-------------- Ход игрока --------------

        //-------------- 1 Фаза Выбрать игрока --------------


        //-------------- 2 Бросить кости  --------------



        //-------------- 1 Фаза Движение --------------



        //-------------- 2 Фаза Подготовкаа к Атаке  --------------



        //-------------- 3 Фаза Противник  -  значения Защита + Контр удар --------------



        //-------------- 4 Фаза Процесс атаки --------------



        //-------------- 5 Фаза Противник  контрудар  --------------



    }
}
