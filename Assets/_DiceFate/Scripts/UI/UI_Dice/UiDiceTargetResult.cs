using DiceFate.Dice;
using DiceFate.EventBus;
using DiceFate.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DiceFate.Maine;
using NUnit.Framework;

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
        [SerializeField] private Mane mane;

        private bool isDiceReady = false;

        private int moveUser;
        private int attackUser;
        private int shildUser;
        private int conterAttackUser;

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
            Bus<OnDropEvent>.OnEvent += HandelDropDice;
        }
        private void OnDestroy()
        {
            Bus<OnDiceReadyEvent>.OnEvent -= OnDiceReady;
            Bus<OnDropEvent>.OnEvent -= HandelDropDice;
        }
        private void Start() => Checked();




        //---------------------------------- События  ---------------------------------- 
        private void HandelDropDice(OnDropEvent evt)
        {
            isDiceReady = true;
        }

        // Rубик остановился и готов
        private void OnDiceReady(OnDiceReadyEvent evt)
        {
            DiceCube dice = evt.Dice;

            if (!isDiceReady) return;
            if (diceCubes.Contains(dice)) return;      // Проверяем, что кубик ещё не обработан

            diceCubes.Add(dice);

            // Создаём UI-элемент для этого кубика
            CreateResultDisplayForDice(dice);
        }


        //---------------------------------- Логика  ---------------------------------- 


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

            StartCoroutine(DelayOnCreateResult());
            // CheckListDiceCube();
        }

        private IEnumerator DelayOnCreateResult()
        {
            yield return new WaitForSeconds(0.7f);
            if (diceCubes.Count >= 4)
            {
                //  UpdateResultDisplay();
                SaveResultsDiseToGameStats();
                mane.MovementAndGridEnable();
            }
            else
            {
                yield break;
            }




            //UpdateResultDisplay();
            //SaveResultsToGameStats();
            //CheckListDiceCube();

            yield return new WaitForSeconds(5f);
            OffResultOnDisplays();

        }

        //private void CheckListDiceCube()
        //{
        //    if (diceCubes.Count >= 4)
        //    {
        //        UpdateResultDisplay();
        //        SaveResultsDiseToGameStats();
        //        mane.MovementAndGridEnable();
        //    }
        //}

        public void SaveResultsDiseToGameStats()
        {
            GameStats.diceMovement = diceResultsDict["Movement"];
            GameStats.diceAttack = diceResultsDict["Attack"];
            GameStats.diceShield = diceResultsDict["Shield"];
            GameStats.diceCounterattack = diceResultsDict["Counterattack"];

            //moveUser = GameStats.moveUser + GameStats.diceMovement;
            //attackUser = GameStats.attackUser + GameStats.diceAttack;
            //shildUser = GameStats.shildUser + GameStats.attackUser;
            //conterAttackUser = GameStats.conterAttackUser + GameStats.shildUser;
        }

        public void UpdateResultValuePlayerToGameStats()
        {
            GameStats.moveUser = GameStats.moveUser + GameStats.diceMovement;
            GameStats.attackUser = GameStats.attackUser + GameStats.diceAttack;
            GameStats.shildUser = GameStats.shildUser + GameStats.attackUser;
            GameStats.conterAttackUser = GameStats.conterAttackUser + GameStats.shildUser;

            //GameStats.moveUser = moveUser;
            //GameStats.attackUser = attackUser;
            //GameStats.shildUser = shildUser;
            //GameStats.conterAttackUser = conterAttackUser;


        }

        public void UpdateResultDisplay()
        {

            textMovment.text = GameStats.moveUser.ToString();
            textAttack.text = GameStats.attackUser.ToString();
            textShield.text = GameStats.shildUser.ToString();
            textCounterattack.text = GameStats.conterAttackUser.ToString();


            //textMovment.text = GameStats.diceMovement.ToString();
            //textAttack.text = GameStats.diceAttack.ToString();
            //textShield.text = GameStats.diceShield.ToString();
            //textCounterattack.text = GameStats.diceCounterattack.ToString();
        }



        public void ClearAll()
        {
            foreach (Transform child in resultContainer.transform)
            {
                Destroy(child.gameObject);
            }
            diceCubes.Clear();
            ClearDictionaryResult();
            SaveResultsDiseToGameStats();
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

            UpdateResultValuePlayerToGameStats();
            UpdateResultDisplay();

            ClearAll();
        }

        private void Checked()
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