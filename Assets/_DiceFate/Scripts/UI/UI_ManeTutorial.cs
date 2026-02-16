using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DiceFate.Events;
using DiceFate.EventBus;
using DiceFate.UI_Dice;
using System;
using DiceFate.Maine;

namespace DiceFate.UI
{
    public class UI_ManeTutorial : MonoBehaviour
    {

        [Header("Фаза 1 настройки")]


        [Header("Player Card")]
        [SerializeField] private GameObject unitMasterCard;    // Фаза 1       
        [SerializeField] private GameObject inactiveElement;
        [SerializeField] private GameObject inactiveElementMove;
        [SerializeField] private GameObject inactiveElementAttack;
        [SerializeField] private GameObject activeElement;
        [SerializeField] private GameObject textDiceValue;
        [Space]
        //[SerializeField] private TextMeshProUGUI textUnitMovement;
        //[SerializeField] private TextMeshProUGUI textUnitAttack;
        //[SerializeField] private TextMeshProUGUI textUnitShield;
        //[SerializeField] private TextMeshProUGUI textUnitCounterattack;
        [Space]
        [SerializeField] private Button startDice;             // Фаза 1
        [SerializeField] private ParticleSystem psStartDice;

        [Header("Фаза 2 настройки")]
        [SerializeField] private TextMeshProUGUI numberDiceToDtop;
        // [SerializeField] private Button keg;                   // Фаза 2
        [SerializeField] private GameObject diceOnTable;       // Фаза 2
        [SerializeField] private GameObject iBackgroundOffClicker;       // Фаза 2


        [Header("Text on TextDiceValue")]
        [SerializeField] private TextMeshProUGUI textDiceMovement;
        [SerializeField] private TextMeshProUGUI textDiceAttack;
        [SerializeField] private TextMeshProUGUI textDiceShield;
        [SerializeField] private TextMeshProUGUI textDiceCounterattack;
        [Space]
        [SerializeField] private Image diceAttack;
        [SerializeField] private Image diceShield;
        [SerializeField] private Image diceCounterattack;



        //[Header("Enemy Card")]
        //[SerializeField] private GameObject uiCardEnemy;
        //[SerializeField] private TextMeshProUGUI textNameEnemy;
        //[SerializeField] private TextMeshProUGUI textEnemyMovement;
        //[SerializeField] private TextMeshProUGUI textEnemyAttack;
        //[SerializeField] private TextMeshProUGUI texEnemyShield;
        //[SerializeField] private TextMeshProUGUI textEnemyCounterattack;

        [Header("Ссылки на системы")]
        [SerializeField] private UiDiceTargetResult uiDiceTargetResult;
        [SerializeField] private UiDropTargetField uiDropTargetField;     // Ссылка на поле с кубиками

        [Header("Для теста")]
        [SerializeField] private TextMeshProUGUI testNameEnemy;
        [SerializeField] private TextMeshProUGUI testPhase;


        [Header("Доп")]
        [SerializeField] private UI_Mane_Battle uI_Mane_Battle;
        [SerializeField] private PrologScenario prologScenario;

        private Color colorTextDefault;
        private Color defoultColorDiceAttack;
        private Color defoultColorDiceShield;
        private Color defoultColorDiceCounterattack;       
        private Color colorAlfaZero = new Color(1f, 1f, 1f, 0f);





        private void Awake()
        {
            //  uiCardEnemy.SetActive(false);

            Bus<UnitSelectedEvent>.OnEvent += HandelUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent += HandeleUnitDeselect;
        }

        private void OnDestroy()
        {
            Bus<UnitSelectedEvent>.OnEvent -= HandelUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent -= HandeleUnitDeselect;
        }


        private void Start()
        {
            //colorTextDefault = textUnitMovement.color;
            defoultColorDiceAttack = diceAttack.color;
            defoultColorDiceShield = diceShield.color;
            defoultColorDiceCounterattack = diceCounterattack.color;

            ValidateScriptsAndObject();

            HideAllUIElements(); // Начальное состояние: всё скрыто
        }



        private void UpdateAvatarBattleFotUI()  // Запустить через событие
        {
            uI_Mane_Battle.UpdateAllAvatars();
        }


        private void Update()
        {
            TestingGameStats();

            // Обновляем значения кубиков в интерфейсе (каждую кадр, но без нагрузки)
            UpdateDiceValuesDisplayToTasting();

            // Тестовые клавиши (можно убрать в релизе)
            if (Input.GetKeyDown(KeyCode.Q))
            {
                //UiEnableResultDisplay();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                //SetResultToCard();
            }
        }

        private void TestingGameStats()
        {
            testNameEnemy.text = GameStats.currentUser;
            testPhase.text = $"Текущая фаза {GameStats.currentPhasePlayer}";
        }



        // Показываем в UI какие кубики были брошены
        //public void UiEnableResultDisplay()
        //{
        //    //uiDiceTargetResult.InitializeResultDisplay();
        //}

        // записываем в GameStats и обновляем значения на карточке в UI 
        //public void SetResultToCard()
        //{
        //    uiDiceTargetResult.SaveResultsToGameStats();
        //    uiDiceTargetResult.UpdateResultDisplay();
        //}

        private void UpdateDiceValuesDisplayToTasting()
        {
            textDiceMovement.text = "Move - " + GameStats.diceMovement.ToString();
            textDiceAttack.text = "Attack - " + GameStats.diceAttack.ToString();
            textDiceShield.text = "Shield - " + GameStats.diceShield.ToString();
            textDiceCounterattack.text = "Counter - " + GameStats.diceCounterattack.ToString();

        }

        //---------------------------------- События  ----------------------------------
        private void HandelUnitSelected(UnitSelectedEvent evt)
        {
            //selectedUnit = evt.Unit; // Обновить текущий выделенный юнит

            EnableUISelectedEnemyOrOherUnit();
        }
        private void HandeleUnitDeselect(UnitDeselectedEvent args)
        {
            DisableUISelectedEnemyOrOherUnit();
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



        public void HideAllUIElements()
        {
            iBackgroundOffClicker.SetActive(false);
            SetUiPlayerCard(false, false, false, false, false, false);

            psStartDice.Stop(); // Для Туториал        
        }
        public void UiShowPhaseOnePlayer()
        {

            switch (prologScenario.prologNumber)
            {
                case 5: // только движение 
                    iShowMasterCadrPrologFive();
                    inactiveElementMove.SetActive(false);
                    inactiveElementAttack.SetActive(true);
                    psStartDice.gameObject.SetActive(true);
                    break;

                default:
                    inactiveElementMove.SetActive(false);
                    inactiveElementAttack.SetActive(false);
                    break;
            }
            uiDiceTargetResult.UpdateTextUiDisplay();
            SetUiPlayerCard(true, false, true, true, true, true); 
        }

        public void UiShowPhaseTwoPlayer()
        {
            iBackgroundOffClicker.SetActive(true);
            SetUiPlayerCard(true, true, false, false, true, true);
           
            switch (prologScenario.prologNumber)
            {
                case 6:
                    diceAttack.color = colorAlfaZero;
                    diceShield.color = colorAlfaZero;
                    diceCounterattack.color = colorAlfaZero;
                    psStartDice.Stop();
                    break;
            }

        }
        public void UiShowPhaseThreePlayer()
        {
            iBackgroundOffClicker.SetActive(false);
            SetUiPlayerCard(true, false, false, false, true, true);
        }
        public void UiShowPhaseFourPlayer() => SetUiPlayerCard(true, false, false, false, true, true);
        public void UiShowPhaseFivePlayer() => SetUiPlayerCard(false, false, false, false, false, false);

        public void UiBackgroundOffClicker(bool isActive)
        {
            iBackgroundOffClicker.SetActive(isActive);
        }

        //public void UiShowMasterCadrDependPrologNumber()
        //{

        //    switch (prologScenario.prologNumber)
        //    {
        //        case 5: // только движение 
        //            iShowMasterCadrPrologFive();
        //            inactiveElementMove.SetActive(false);
        //            inactiveElementAttack.SetActive(true);
        //            psStartDice.gameObject.SetActive(true);
        //            break;

        //        default:
        //            inactiveElementMove.SetActive(false);
        //            inactiveElementAttack.SetActive(false);
        //            break;
        //    }
        //    uiDiceTargetResult.UpdateTextUiDisplay();
        //    SetUiPlayerCard(true, false, true, true, true, true);               
        //}


        private void iShowMasterCadrPrologFive()
        {
            uiDiceTargetResult.UpdateTextUiDisplay();
            SetUiPlayerCard(true, false, true, true, true, true);

            //textUnitAttack.color = colorAlfaZero;
            //textUnitShield.color = colorAlfaZero;
            //textUnitCounterattack.color = colorAlfaZero;

        }



        public void TestingListToDiceTargetResult()
        {
            // uiDiceTargetResult.EnableTestListUpdate();
        }

        //-------------------- Управление отображением UI для Enemy или Other -------------------
        private void EnableUISelectedEnemyOrOherUnit()
        {
            Debug.Log("Выбор Врвг ЮЙ");

            // testNameEnemy.text = GameStats.currentUser;


            if (GameStats.currentUser == "Enemy")
            {

                //uiCardEnemy.SetActive(true);

                //textNameEnemy.text = GameStats.nameUnit;
                //textEnemyMovement.text = GameStats.moveUser.ToString();
                //textEnemyAttack.text = GameStats.attackUser.ToString();
                //texEnemyShield.text = GameStats.shildUser.ToString();
                //textEnemyCounterattack.text = GameStats.conterAttackUser.ToString();
            }

            if (GameStats.currentUser == "Other")
            {

            }
        }

        private void DisableUISelectedEnemyOrOherUnit()
        {
            Debug.Log("отмена выбота Врвг ЮЙ");
            if (GameStats.currentUser == "Enemy")
            {
                // uiCardEnemy.SetActive(false);
            }

            if (GameStats.currentUser == "Other")
            {

            }
        }

        private void UpdateTextValuePlayer()
        {

        }




        //--------- Управление отображением результатов броска кубиков --------------

        // public void UiDisableResultDisplay() => uiDiceTargetResult.OffResultOnDisplays();
        public void UiDisplayClearAll() => uiDiceTargetResult.ClearAll();
        public void UiClearAllDiceOnField() => uiDropTargetField.ResetAllDiceAndClearField(); // Очистить список кубиков на платке





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
            if (uiDropTargetField == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на uiDropTargetField!");
            if (uI_Mane_Battle == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на uI_Mane_Battle!");
            if (iBackgroundOffClicker == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на iBackgroundOffClicker!");
            //if (uI_BlockOnClick == null)
            //    Debug.LogError($" для {this.name} Не установлена ссылка на uI_BlockOnClick!");

        }
    }
}
