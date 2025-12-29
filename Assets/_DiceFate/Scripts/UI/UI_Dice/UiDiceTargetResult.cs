using UnityEngine;
using System.Collections.Generic;
using DiceFate.Dice;
using TMPro;
using DiceFate.EventBus;
using DiceFate.Events;

namespace DiceFate.UI_Dice
{
    public class UiDiceTargetResult : MonoBehaviour
    {
        [Header("Настройки UI")]
        [SerializeField] private GameObject resultContainer; // Родительский контейнер для результатов
        [SerializeField] private GameObject uiResultDicePrefab; // Префаб UI-кубика

        [SerializeField] private TextMeshProUGUI textMovment;
        [SerializeField] private TextMeshProUGUI textAttack;
        [SerializeField] private TextMeshProUGUI textShield;
        [SerializeField] private TextMeshProUGUI textCounterattack;

        private List<DiceCube> diceCubes = new List<DiceCube>();
        private Dictionary<string, int> diceResultsDict = new Dictionary<string, int>()
        {
            { "Movement", 0 },
            { "Attack", 0 },
            { "Shield", 0 },
            { "Counterattack", 0 }
        };

        private void Awake()
        { 
           resultContainer.SetActive(true); // - было  фолс Разобраться

           Bus<OnDiceReadyEvent>.OnEvent += OnDiceReady;
        }
        private void OnDestroy()
        {         
            Bus<OnDiceReadyEvent>.OnEvent -= OnDiceReady;
        }
        private void Start() => Checked();

        // Обработчик события: кубик остановился и готов
        private void OnDiceReady(OnDiceReadyEvent evt)
        {
            DiceCube dice = evt.Dice;

            // Проверяем, что кубик ещё не обработан
            if (diceCubes.Contains(dice))
                return;

            diceCubes.Add(dice);

            // Создаём UI-элемент для этого кубика
            CreateResultDisplayForDice(dice);

            // Обновляем суммарные значения в интерфейсе
            UpdateResultDisplay();
        }

        // Создаёт UI-элемент для конкретного кубика
        private void CreateResultDisplayForDice(DiceCube dice)
        {
            // Создаём экземпляр префаба
            GameObject uiInstance = Instantiate(uiResultDicePrefab, resultContainer.transform);
     
            // Получаем компонент UI
            UiResultDice uiResultDice = uiInstance.GetComponent<UiResultDice>();
            if (uiResultDice == null)
            {
                Debug.LogWarning("Созданный префаб не содержит компонент UiResultDice");
                return;
            }

            // Получаем значение кубика
            int currentValue = dice.GetLastResult();     
            Debug.Log($"Кубик {dice.gameObject.name} остановился. Тип: {dice.diceType}, значение: {currentValue}");

            // Отображаем значение и окрашиваем по типу
            uiResultDice.OnEnabledValue(currentValue);
            uiResultDice.ColorDiceResult(dice.diceType.ToString());

            // Обновляем словарь результатов
            string type = dice.diceType.ToString();
            if (diceResultsDict.ContainsKey(type))
            {
                diceResultsDict[type] += currentValue;
            }
        }


        public void SaveResultsToGameStats()
        {
            GameStats.diceMovement = diceResultsDict["Movement"];
            GameStats.diceAttack = diceResultsDict["Attack"];
            GameStats.diceShield = diceResultsDict["Shield"];
            GameStats.diceCounterattack = diceResultsDict["Counterattack"];
        }

        public void UpdateResultDisplay()
        {           
            textMovment.text = GameStats.diceMovement.ToString();
            textAttack.text = GameStats.diceAttack.ToString();
            textShield.text = GameStats.diceShield.ToString();
            textCounterattack.text = GameStats.diceCounterattack.ToString();  
        }

        public void ClearAll()
        {
            foreach (Transform child in resultContainer.transform)
            {
                Destroy(child.gameObject);
            }
            diceCubes.Clear();
            ClearDictionaryResult();
        }

        private void ClearDictionaryResult()
        {
            diceResultsDict["Movement"] = 0;
            diceResultsDict["Attack"] = 0;
            diceResultsDict["Shield"] = 0;
            diceResultsDict["Counterattack"] = 0;
        }

        public void OffResultOnDisplays()
        {
            resultContainer.SetActive(false);
        }

        void Checked()
        {
            if (resultContainer == null)
            {
                Debug.LogError("Не назначен контейнер для результатов (C_ResultDice)!");
            }

            if (uiResultDicePrefab == null)
            {
                Debug.LogError("Не назначен префаб UiResultDice в инспекторе!");
            }

            ClearAll();
        }

    }
}