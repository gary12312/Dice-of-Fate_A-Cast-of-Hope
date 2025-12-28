using DiceFate.UI_Dice;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceFate.UI
{
    public class UI_Mane : MonoBehaviour
    {
        [Header("Фаза 1 настройки")]


        [Header("PlayerCard")]
        [SerializeField] private GameObject unitMasterCard;    // Фаза 1
        [SerializeField] private GameObject diceOnTable;       // Фаза 2
        [SerializeField] private GameObject inactiveElement;
        [SerializeField] private GameObject activeElement;
        [SerializeField] private GameObject textDiceValue;
        [SerializeField] private Button startDice;             // Фаза 1
        [SerializeField] private Button keg;                   // Фаза 2

        [Header("Text on TextDiceValue")]
        [SerializeField] private TextMeshProUGUI textDiceMovement;
        [SerializeField] private TextMeshProUGUI textDiceAttack;
        [SerializeField] private TextMeshProUGUI textDiceShield;
        [SerializeField] private TextMeshProUGUI textDiceCounterattack;


        [Header("Дополнительно")]
        [SerializeField] private UiDiceTargetResult uiDiceTargetResult;





        private void Start()
        {
            ValidateScriptsAndObject();
        }

        private void FixedUpdate()
        {
            UpdateDiceValuesDisplay();


            if (Input.GetKeyDown(KeyCode.Q))
            {
                uiDiceTargetResult.InitializeResultDisplay();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                uiDiceTargetResult.UpdateResultDisplay();
            }
        }



        // Показываем в UI какие кубики были брошены
        public void UiEnableResultDisplay()
        {
            uiDiceTargetResult.InitializeResultDisplay();
        }

        // записываем в GameStats и обновляем значения на карточке в UI 
        public void SetResultToCard()
        {
            uiDiceTargetResult.SaveResultsToGameStats();
            uiDiceTargetResult.UpdateResultDisplay();
        }

        private void UpdateDiceValuesDisplay()
        {
            textDiceMovement.text = "Move - " + GameStats.diceMovement.ToString();
            textDiceAttack.text = "Attack - " + GameStats.diceAttack.ToString();
            textDiceShield.text = "Shield - " + GameStats.diceShield.ToString();
            textDiceCounterattack.text = "Counter - " + GameStats.diceCounterattack.ToString();

        }

        // ------------------------ UI управление ---------------------------------

        private void SetUiPlayerCard(
            bool isUnitMasterCard,
            bool isDiceOnTable,
            bool isStartDice,
            bool isInactiveElement,
            bool isActiveElement,
            bool isTextDiceValue)
        {
            unitMasterCard.SetActive(isUnitMasterCard);   // 1 - юнит карта
            diceOnTable.SetActive(isDiceOnTable);         // 2 - кубики на столе
            startDice.gameObject.SetActive(isStartDice);  // 3 - кнопка старт броска кубиков
            inactiveElement.SetActive(isInactiveElement); // 4 - неактивный элемент 
            activeElement.SetActive(isActiveElement);     // 5 - активный элемент
            textDiceValue.SetActive(isTextDiceValue);     // 6 - текстовое значение кубиков
        }



        public void HideAllUIElements() => SetUiPlayerCard(false, false, false, false, false, false);
        public void UiShowPhaseOnePlayer() => SetUiPlayerCard(true, false, true, true, false, false);
        public void UiShowPhaseTwoPlayer() => SetUiPlayerCard(true, true, false, false, true, false);
        public void UiShowPhaseThreePlayer() => SetUiPlayerCard(true, false, false, false, true, false);
        public void UiShowPhaseFourPlayer() => SetUiPlayerCard(true, false, false, false, false, true);
        public void UiShowPhaseFivePlayer() => SetUiPlayerCard(false, false, false, false, false, false);


        public void TestingListToDiceTargetResult()
        {
            uiDiceTargetResult.EnableTestListUpdate();
        }




        //--------- Управление отображением результатов броска кубиков --------------
        public void UiDisableResultDisplay() => uiDiceTargetResult.OffResultOnDisplays();
        public void UiDisplayClearAll() => uiDiceTargetResult.ClearAll();


        // ------------------------ Проверки ---------------------------------
        private void ValidateScriptsAndObject()
        {
            if (uiDiceTargetResult == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на uiResultDisplay!");
            if (inactiveElement == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на inactiveElement!");
            if (activeElement == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на activeElement!");
            if (textDiceValue == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на textDiceValue!");
            if (textDiceMovement == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на textDiceMovement!");
            if (textDiceAttack == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на textDiceAttack!");
            if (textDiceShield == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на textDiceShield!");
            if (textDiceCounterattack == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на textDiceCounterattack!");
        }
    }
}
