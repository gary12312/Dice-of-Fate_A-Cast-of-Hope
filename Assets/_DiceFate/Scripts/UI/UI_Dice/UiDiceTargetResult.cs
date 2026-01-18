using DiceFate.Dice;
using DiceFate.EventBus;
using DiceFate.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DiceFate.Maine;
using DG.Tweening;



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

        [Header("Точки спавна и цели")]
        [SerializeField] private GameObject resultPosition; // Родительский контейнер для результатов
        [SerializeField] private UIParticalArrey UI_Particals_M;
        [SerializeField] private UIParticalArrey UI_Particals_A;
        [SerializeField] private UIParticalArrey UI_Particals_S;
        [SerializeField] private UIParticalArrey UI_Particals_C;


        [SerializeField] private int ferstPS = 1;
        [SerializeField] private int secondPS =2;

        [SerializeField] private RectTransform spawnPointMovment;
        [SerializeField] private RectTransform spawnPointAttack;
        [SerializeField] private RectTransform spawnPointShield;
        [SerializeField] private RectTransform spawnPointCounterattack;

        [SerializeField] private RectTransform targetPointMovment;
        [SerializeField] private RectTransform targetPointAttack;
        [SerializeField] private RectTransform targetPointShield;
        [SerializeField] private RectTransform targetPointCounterattack;

        [SerializeField] private AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f)); // Кривая для плавности
        [SerializeField] private float animationDuration = 0.5f;


        private bool isDiceReady = false;
        private Sequence _sequenceAnimation;

        //private Transform spawnPoint = null;
        //private RectTransform targetPoint = null;

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
            ResetPlayerDiceResultsToGameStats();
        }

        // Rубик остановился и готов
        private void OnDiceReady(OnDiceReadyEvent evt)
        {
            DiceCube dice = evt.Dice;

            if (!isDiceReady) return;
            if (diceCubes.Contains(dice)) return;      // Проверяем, что кубик ещё не обработан

            diceCubes.Add(dice);

            // Создаём UI-элемент для этого кубика
           // CreateUiResultDisplayForDice(dice);
            CreateUiResultInPointForDice(dice);
        }



        //---------------------------------- Логика  ---------------------------------- 


        // Создаёт UI-элемент для конкретного кубика
        private void CreateUiResultDisplayForDice(DiceCube dice)
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

                // Немедленно сохраняем в GameStats
                SaveToGameStats(type, currentValue);
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
                //SaveResultsDiseToGameStats();
                mane.MovementAndGridEnable();
            }
            else
            {
                yield break;
            }

            //UpdateResultDisplay();
            //SaveResultsToGameStats();
            //CheckListDiceCube();

            yield return new WaitForSeconds(3f);
            OffResultOnDisplays();

        }



        // V2    Создаёт UI-элемент для конкретного кубика 
        private void CreateUiResultInPointForDice(DiceCube dice)
        {
            RectTransform spawnPoint = null;
            RectTransform targetPoint = null;
            UIParticalArrey particals = null;

            switch (dice.diceType)
            {
                case DiceCube.DiceType.Movement:
                    spawnPoint = spawnPointMovment;
                    targetPoint = targetPointMovment;
                    particals = UI_Particals_M;
                    break;
                case DiceCube.DiceType.Attack:
                    spawnPoint = spawnPointAttack;
                    targetPoint = targetPointAttack;
                    particals = UI_Particals_A;
                    break;
                case DiceCube.DiceType.Shield:
                    spawnPoint = spawnPointShield;
                    targetPoint = targetPointShield;
                    particals = UI_Particals_S;
                    break;
                case DiceCube.DiceType.Counterattack:
                    spawnPoint = spawnPointCounterattack;
                    targetPoint = targetPointCounterattack;
                    particals = UI_Particals_C;
                    break;
            }

            if (spawnPoint == null || targetPoint == null)
            { return; }

            ActiveParticalSystem (particals, ferstPS, spawnPoint);           

            // Создаём экземпляр префаба
            GameObject uiInstance = Instantiate(uiResultDicePrefab, spawnPoint.position,
                                               Quaternion.identity, resultPosition.transform);

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

            // uiInstance.DOAnchorPos(targetPointMovment, 0.5f).SetEase(animationCurve).Play();

            // Проверьте, что resultPosition имеет Canvas как родителя
            RectTransform rectTransform = uiInstance.GetComponent<RectTransform>();
            if (rectTransform == null)  { return; }

            SaveToGameStats(dice.diceType.ToString(), currentValue);

            StartCoroutine(TweemResultAnimation(particals, rectTransform, targetPoint, dice.diceType.ToString(), currentValue));

        }



        IEnumerator TweemResultAnimation(UIParticalArrey partical, RectTransform rectTransform, RectTransform targetPoint, string type, int currentValue)
        {
            yield return new WaitForSeconds(2f);
            
                //rectTransform.DOAnchorPos(targetPoint.anchoredPosition, 0.5f)
                //             .SetEase(animationCurve)
                //             .Play();
           

            _sequenceAnimation = DOTween.Sequence();
            _sequenceAnimation 
                .Append(rectTransform.DOAnchorPos(targetPoint.anchoredPosition, animationDuration).SetEase(animationCurve))
                .Join(rectTransform.transform.DOScale(0, 0.6f).SetEase(Ease.InBounce))
               // .OnComplete(() => ActiveParticalSystem(partical, secondPS, targetPoint))
                .OnComplete(() => SaveToUnitGameStats(type, partical, secondPS, targetPoint))
                .Play();
        }
           
        public void ActiveParticalSystem(UIParticalArrey partical, int index, RectTransform rectTransform)
        {                    
            if (partical == null) return;
            partical.ActivePartical(index, rectTransform);            
        }


        private void SaveToGameStats(string diceType, int value)
        {
            switch (diceType)
            {
                case "Movement":
                    GameStats.diceMovement = value;
                    break;
                case "Attack":
                    GameStats.diceAttack = value;
                    break;
                case "Shield":
                    GameStats.diceShield = value;
                    break;
                case "Counterattack":
                    GameStats.diceCounterattack = value;
                    break;
            }
        }
        private void SaveToUnitGameStats(string diceType, UIParticalArrey partical, int index, RectTransform targetPoint)
        {
            ActiveParticalSystem(partical, secondPS, targetPoint);


            switch (diceType)
            {
                case "Movement":
                    GameStats.moveUser = GameStats.moveUser + GameStats.diceMovement;
                    textMovment.text = GameStats.moveUser.ToString();
                    break;
                case "Attack":
                    GameStats.attackUser = GameStats.attackUser + GameStats.diceAttack;
                    textAttack.text = GameStats.attackUser.ToString();
                    break;
                case "Shield":
                    GameStats.shildUser = GameStats.shildUser + GameStats.diceAttack;
                    textShield.text = GameStats.shildUser.ToString();
                    break;
                case "Counterattack":
                    GameStats.conterAttackUser = GameStats.conterAttackUser + GameStats.diceShield;
                    textCounterattack.text = GameStats.conterAttackUser.ToString();
                    break;
            }
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


        private void ResetPlayerDiceResultsToGameStats()
        {
            GameStats.diceMovement = 0;
            GameStats.diceAttack = 0;
            GameStats.diceShield = 0;
            GameStats.diceCounterattack = 0;
        }



        public void UpdateResultValuePlayerToGameStats()
        {
            GameStats.moveUser = GameStats.moveUser + GameStats.diceMovement;
            GameStats.attackUser = GameStats.attackUser + GameStats.diceAttack;
            GameStats.shildUser = GameStats.shildUser + GameStats.diceAttack;
            GameStats.conterAttackUser = GameStats.conterAttackUser + GameStats.diceShield;

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