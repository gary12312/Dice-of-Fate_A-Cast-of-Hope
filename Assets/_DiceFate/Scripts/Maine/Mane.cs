using DiceFate.EventBus;
using DiceFate.Events;
using DiceFate.Units;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;


namespace DiceFate.Maine
{
    public class Mane : MonoBehaviour
    {
        [SerializeField] private GameObject unitMasterCard;
        [SerializeField] private Button dice;
        [SerializeField] private Button keg;
        [SerializeField] private GameObject diceGroup;

        [SerializeField] private Button buttonReturne;

        [Header("Ссылка на поле")]
        public DropTargetField dropTargetField; // Ссылка на поле с кубиками

        private bool isDiceGroupActive = false;

        private ISelectable selectedUnit; // Текущий выделенный юнит который имеет интерфейс ISelectable
        private ISelectable selectedEnemy;


        

        private void Awake()
        {
            Bus<UnitSelectedEvent>.OnEvent += HandelUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent += HandeleUnitDeselect;
            Bus<EnemySelectedEvent>.OnEvent += HandelEnemySelected;
            Bus<EnemyDeselectedEvent>.OnEvent += HandelEnemyDeselected;

            buttonReturne.onClick.AddListener(ClickButtoneToReturne); // подписывается на щелчок
            dice.onClick.AddListener(PhaseTwoSelectedUnit); // подписывается на щелчок
            keg.onClick.AddListener(PhaseThreeSelectedUnit); // подписывается на щелчок


        }
        private void OnDestroy()
        {
            Bus<UnitSelectedEvent>.OnEvent -= HandelUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent -= HandeleUnitDeselect;
            Bus<EnemySelectedEvent>.OnEvent -= HandelEnemySelected;
            Bus<EnemyDeselectedEvent>.OnEvent -= HandelEnemyDeselected;

            buttonReturne.onClick.RemoveListener(ClickButtoneToReturne);
            dice.onClick.RemoveListener(PhaseTwoSelectedUnit);
            keg.onClick.RemoveListener(PhaseThreeSelectedUnit);
        }

        private void Start()
        {
            if (dropTargetField == null) Debug.LogError($" для {this.name} Не установлена ссылка на DropTargetField!");

            HideAllUIElements();
        }


        private void SetUIElementsVisible(bool isUnitMasterCard, bool isDice, bool isDiceGroup)
        {
            unitMasterCard.SetActive(isUnitMasterCard);
            dice.gameObject.SetActive(isDice);
            diceGroup.SetActive(isDiceGroup);
        }
        private void HideAllUIElements() => SetUIElementsVisible(false, false, false);
        private void ShowPhaseOneUIElements() => SetUIElementsVisible(isUnitMasterCard: true, isDice: true, isDiceGroup: false);
        private void ShowPhaseTwoUIElements() => SetUIElementsVisible(isUnitMasterCard: true, isDice: false, isDiceGroup: true);
        private void ShowPhaseThreeUIElements() => SetUIElementsVisible(isUnitMasterCard: true, isDice: false, isDiceGroup: false);

        private void Phase(int number) => PhaseNumber.currentPhase = number;



        //---------------------------------- Выбор и выделение   ----------------------------------
        private void HandelUnitSelected(UnitSelectedEvent evt)
        {
            selectedUnit = evt.Unit; // Обновить текущий выделенный юнит
            PhaseOneSelectedUnit();
        }

        private void HandeleUnitDeselect(UnitDeselectedEvent evt)
        {
            selectedUnit = null;  // Сбросить текущий выделенный юнит
            HideAllUIElements();
            Phase(0);
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

        //-------------- 1 Фаза - Выбрать игрока --------------
        public void PhaseOneSelectedUnit()
        {
            Phase(1);
            ShowPhaseOneUIElements();

            Debug.Log($"Фаза = {PhaseNumber.currentPhase}");
        }

        //-------------- 2 Фаза - Активация кубиков для выбора  --------------
        public void PhaseTwoSelectedUnit()
        {
            Phase(2);
            ShowPhaseTwoUIElements();

            Debug.Log($"Фаза = {PhaseNumber.currentPhase}");
        }

        //-------------- 3 Бросить кости выбранные кости  --------------
        public void PhaseThreeSelectedUnit()
        {
            Phase(3);
            ShowPhaseThreeUIElements();
            CheckCountAndNameDiceOnField();

            Debug.Log($"Фаза = {PhaseNumber.currentPhase}");
        }


        public void CheckCountAndNameDiceOnField()
        {
            List<string> diceNames = dropTargetField?.GetDiceNamesOnField();
            List<string> diceCount = dropTargetField?.GetDiceNamesOnField();

            if (diceCount.Count > 0)
            {
                // Подсчитываем количество каждого типа
                int movementCount = dropTargetField.GetDiceTypeCount(DragAndDropDice.DiceType.Movement);
                int attackCount = dropTargetField.GetDiceTypeCount(DragAndDropDice.DiceType.Attack);
                int shieldCount = dropTargetField.GetDiceTypeCount(DragAndDropDice.DiceType.Shield);
                int counterattackCount = dropTargetField.GetDiceTypeCount(DragAndDropDice.DiceType.Counterattack);

                // Формируем детальный отчет
                string details = $"Детальная информация о кубиках на поле:\n" +
                               $"Всего кубиков: {diceCount.Count}\n" +
                               $"Movement: {movementCount}\n" +
                               $"Attack: {attackCount}\n" +
                               $"Shield: {shieldCount}\n" +
                               $"Counterattack: {counterattackCount}";

                // Объединяем имена кубиков через запятую
              //  string nameDices = " На поле кубики: " + string.Join(", ", diceNames);

                string finalReport = $"На поле Всего кубиков: {diceCount.Count}, кубики: {string.Join(", ", diceNames)} ";

                Debug.Log(finalReport);
            }
        }





        //-------------- 1 Фаза Движение --------------



        //-------------- 2 Фаза Подготовкаа к Атаке  --------------



        //-------------- 3 Фаза Противник  -  значения Защита + Контр удар --------------



        //-------------- 4 Фаза Процесс атаки --------------



        //-------------- 5 Фаза Противник  контрудар  --------------



        //-------------- Вспомогательные методы  --------------

        public void ClickButtoneToReturne()
        {
            switch (PhaseNumber.currentPhase)
            {
                case 0:

                    break;
                case 1:
                    selectedUnit.Deselect();
                    break;
                case 2:
                    PhaseOneSelectedUnit();
                    break;
                case 3:
                    PhaseTwoSelectedUnit();
                    break;
                case 4:
                    PhaseThreeSelectedUnit();
                    break;


                    //default:
                    //    HideAllUIElements();
                    //    PhaseNumber.currentPhase = 0;
                    //    Debug.Log(debugNumberPhase);
                    //    break;
            }
        }
    }
}
