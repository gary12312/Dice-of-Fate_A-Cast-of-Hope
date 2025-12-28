using UnityEngine;
using System.Collections.Generic;
using DiceFate.Dice;
using TMPro;

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

        [SerializeField] private TextMeshProUGUI textLightList;

        private List<DiceCube> diceCubes = new List<DiceCube>();
        private List<DiceCube> diceCubesTestList = new List<DiceCube>();

        private bool isTestList = false;
        private int amountDiceInTestList = 0;


        private Dictionary<string, int> diceResultsDict = new Dictionary<string, int>()
        {
            { "Movement", 0 },
            { "Attack", 0 },
            { "Shield", 0 },
            { "Counterattack", 0 }
        };

        private void Awake() => resultContainer.SetActive(false);      

        private void Start() => Checked();

        private void FixedUpdate()
        {
            if (isTestList)
            { 
                Debug.Log("Обновление тестового списка кубиков...");
                AddDiceToTestList();           
            }
            



            // Для тестирования: показать список типов кубиков в текстовом поле
            if (Input.GetKeyDown(KeyCode.T))
            {
                EnableTestListUpdate();
            }
        }

        //------ Добавление кубиков в тестовый список

        public void EnableTestListUpdate()
        {
            diceCubesTestList.Clear();
           
            GameObject[] cubeObjects = GameObject.FindGameObjectsWithTag("Cube");
            amountDiceInTestList = cubeObjects.Length;

            isTestList = true;
        }




        private void AddDiceToTestList()
        {
            diceCubesTestList.Clear(); // Придумать другой способ, чтобы не очищать список каждый кадр

            GameObject[] cubeObjects = GameObject.FindGameObjectsWithTag("Cube");

            foreach (GameObject cube in cubeObjects)
            {
                DiceCube diceValue = cube.GetComponent<DiceCube>();
                if (diceValue != null && diceValue.isDiceReady)
                {
                    diceCubesTestList.Add(diceValue);
                }
            }

            textLightList.text = "Кубики в списке: " + diceCubesTestList.Count;

            if (diceCubesTestList.Count >= amountDiceInTestList)
            {
                isTestList = false;
                Debug.Log("Тестовый список кубиков заполнен.");
                amountDiceInTestList= 0;
            }

        }    

        //------------

        public void InitializeResultDisplay()
        {
            resultContainer.SetActive(true);

            // 1. Найти все кубики на сцене с тегом "Cube"
            GameObject[] cubeObjects = GameObject.FindGameObjectsWithTag("Cube");

            if (cubeObjects.Length == 0)
            {
                Debug.LogWarning("Не найдены объекты с тегом 'Cube' на сцене");
                return;
            }

            // 2. Получить компоненты DiceCubeValue
            diceCubes.Clear(); // Очищаем список перед заполнением
            foreach (GameObject cube in cubeObjects)
            {
                DiceCube diceValue = cube.GetComponent<DiceCube>();
                if (diceValue != null)
                {
                    diceCubes.Add(diceValue);
                }
            }

            Debug.Log($"Найдено кубиков: {diceCubes.Count}");

            // 3. Создать UI-элементы для каждого кубика
            CreateResultDisplays();
        }

        void CreateResultDisplays()
        {
            ClearAll();

            // Создать новые UI-элементы
            foreach (DiceCube dice in diceCubes)
            {
                // Создать экземпляр префаба
                GameObject uiInstance = Instantiate(uiResultDicePrefab, resultContainer.transform);

                // Получить компонент UiResultDice_q (изменили на новую версию)
                UiResultDice uiResultDice = uiInstance.GetComponent<UiResultDice>();
                if (uiResultDice == null)
                {
                    Debug.LogWarning("Созданный префаб не содержит компонент UiResultDice_q");
                    continue;
                }

                // Получить текущее значение кубика
                int currentValue = dice.GetLastResult();

                Debug.Log($"Кубик {dice.gameObject.name}, тип: {dice.diceType}, значение: {currentValue}");

                //Показать значение кубика сразу при создании
                uiResultDice.OnEnabledValue(currentValue);
                uiResultDice.ColorDiceResult(dice.diceType.ToString());

                // Обновить словарь результатов по типам кубиков
                diceResultsDict[dice.diceType.ToString()] += currentValue;
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
            Debug.Log("Обновление результатов кубиков...");
            textMovment.text = GameStats.diceMovement.ToString();
            textAttack.text = GameStats.diceAttack.ToString();
            textShield.text = GameStats.diceShield.ToString();
            textCounterattack.text = GameStats.diceCounterattack.ToString();
        }

        public void ClearAll()
        {
            foreach (Transform child in resultContainer.transform) { Destroy(child.gameObject); } // Очистить старые элементы
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