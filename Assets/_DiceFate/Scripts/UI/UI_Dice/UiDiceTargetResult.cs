using DG.Tweening;
using DiceFate.Dice;
using DiceFate.EventBus;
using DiceFate.Events;
using DiceFate.Maine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Архив;



namespace DiceFate.UI_Dice
{
    public class UiDiceTargetResult : MonoBehaviour
    {
        [Header("Настройки UI")]
        //  [SerializeField] private GameObject resultContainer; // Родительский контейнер для результатов
        [SerializeField] private GameObject uiResultDicePrefab; // Префаб UI-кубика
        [SerializeField] private Mane mane;
        //[SerializeField] private int numberDiceToDtop=3;
        [Space]
        [SerializeField] private TextMeshProUGUI textMovment;
        [SerializeField] private TextMeshProUGUI textAttack;
        [SerializeField] private TextMeshProUGUI textShield;
        [SerializeField] private TextMeshProUGUI textCounterattack;

        [Header("Точки спавна и цели")]
        [SerializeField] private GameObject resultPosition; // Родительский контейнер для результатов
        [SerializeField] private UIParticalArrey UI_Particals_M;
        [SerializeField] private UIParticalArrey UI_Particals_A;
        [SerializeField] private UIParticalArrey UI_Particals_S;
        [SerializeField] private UIParticalArrey UI_Particals_C;
        [Space]
        [SerializeField] private int ferstPS = 1;
        [SerializeField] private int secondPS = 2;
        [Space]
        [SerializeField] private RectTransform spawnPointMovment;
        [SerializeField] private RectTransform spawnPointAttack;
        [SerializeField] private RectTransform spawnPointShield;
        [SerializeField] private RectTransform spawnPointCounterattack;
        [Space]
        [SerializeField] private RectTransform targetPointMovment;
        [SerializeField] private RectTransform targetPointAttack;
        [SerializeField] private RectTransform targetPointShield;
        [SerializeField] private RectTransform targetPointCounterattack;
        [Space]
        [SerializeField] private AnimationCurve animCurveMove = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f)); // Кривая для плавности
        [SerializeField] private AnimationCurve animCurveScale = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private float durationBeforeAnimation = 1.5f;

        private bool isDiceReady = false;
        private bool isDiceMoveblRunGridEnable = false;
        private Sequence _sequenceAnimation;
        private int countedCreatedDice = 0;

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
            // resultContainer.SetActive(true); // - было  фолс Разобраться         

            Bus<OnDiceReadyEvent>.OnEvent += OnDiceReady;
            Bus<OnDropEvent>.OnEvent += HandelDropDice;
        }
        private void OnDestroy()
        {
            Bus<OnDiceReadyEvent>.OnEvent -= OnDiceReady;
            Bus<OnDropEvent>.OnEvent -= HandelDropDice;

            _sequenceAnimation?.Kill();
        }
        private void Start()
        {
            ValidateScriptsAndObjects();
            ClearAll();
        }


        //---------------------------------- События  ---------------------------------- 
        private void HandelDropDice(OnDropEvent evt)
        {
            isDiceReady = true;
            isDiceMoveblRunGridEnable = false;
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
            CreateUiResultInPointForDice(dice);
        }



        //---------------------------------- Логика  ---------------------------------- 



        //  Создаём UI-элемент для конкретного кубика 
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

            ActiveParticalSystem(particals, ferstPS, spawnPoint);

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

            // Проверяем, что resultPosition имеет Canvas как родителя
            RectTransform rectTransform = uiInstance.GetComponent<RectTransform>();
            if (rectTransform == null) { return; }

            SaveResultDiceToGameStats(dice.diceType.ToString(), currentValue);
            UpdateUnitValueGameStats(dice.diceType.ToString());

            ReadyAndGoNextPhaseToMane(); // Показываем на поле           

            StartCoroutine(AnimateResultDice(particals, rectTransform, targetPoint, dice.diceType.ToString(), uiInstance));
        }



        IEnumerator AnimateResultDice(UIParticalArrey partical, RectTransform rectTransform, RectTransform targetPoint, string type, GameObject uiInstance)
        {
            yield return new WaitForSeconds(durationBeforeAnimation);

            _sequenceAnimation = DOTween.Sequence();
            _sequenceAnimation
                .Append(rectTransform.DOAnchorPos(targetPoint.anchoredPosition, animationDuration).SetEase(animCurveMove))
                .Join(rectTransform.transform.DOScale(0, animationDuration).SetEase(animCurveScale))
                .OnComplete(() => UpdateTextRunManeGrid(type, partical, targetPoint, uiInstance))
                .Play();
        }
        private void UpdateTextRunManeGrid(string diceType, UIParticalArrey partical, RectTransform targetPoint, GameObject uiInstance)
        {
            ActiveParticalSystem(partical, secondPS, targetPoint);

            //UpdateUnitValueGameStats(diceType); - Если все работает почистить
            UpdateTextUiDisplay();

            Destroy(uiInstance);

            countedCreatedDice++;

            // ReadyAndGoNextPhase(); - Если все работает почистить

            ReadyAndGo();

        }

        private void ReadyAndGoNextPhase()
        {
            if (countedCreatedDice >= diceCubes.Count)
            {
                mane.MovementAndGridEnable();
                countedCreatedDice = 0;
                diceCubes.Clear();
                isDiceReady = false;
            }
        }

        private void ReadyAndGo()
        {
            if (countedCreatedDice >= diceCubes.Count)
            {
                countedCreatedDice = 0;
                diceCubes.Clear();
                isDiceReady = false;
            }
        }
        private void ReadyAndGoNextPhaseToMane()
        {
            bool hasMovementDice = diceCubes.Exists(dice => dice.diceType == DiceCube.DiceType.Movement);

            if (hasMovementDice && isDiceMoveblRunGridEnable == false)
            {
                mane.MovementAndGridEnable();
                isDiceMoveblRunGridEnable = true;
                Debug.Log($"Значение 11111111111111111111111111111111111111111111111111111111 ");
            }
        }



        public void TestMoveAndGreed()
        {
            mane.MovementAndGridEnable();
            countedCreatedDice = 0;
            diceCubes.Clear();
            isDiceReady = false;


        }


        public void ActiveParticalSystem(UIParticalArrey partical, int index, RectTransform rectTransform)
        {
            if (partical == null) return;
            partical.ActivePartical(index, rectTransform);
        }

        private void SaveResultDiceToGameStats(string diceType, int value)
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
        private void UpdateUnitValueGameStats(string diceType)
        {
            switch (diceType)
            {
                case "Movement":
                    GameStats.moveUser += GameStats.diceMovement;
                    break;
                case "Attack":
                    GameStats.attackUser += GameStats.diceAttack;
                    break;
                case "Shield":
                    GameStats.shildUser += GameStats.diceShield;
                    break;
                case "Counterattack":
                    GameStats.conterAttackUser += GameStats.diceCounterattack;
                    break;
            }
        }


        public void SaveResultsDiseToGameStats()
        {
            GameStats.diceMovement = diceResultsDict["Movement"];
            GameStats.diceAttack = diceResultsDict["Attack"];
            GameStats.diceShield = diceResultsDict["Shield"];
            GameStats.diceCounterattack = diceResultsDict["Counterattack"];
        }

        public void UpdateTextUiDisplay()
        {
            textMovment.text = GameStats.moveUser.ToString();
            textAttack.text = GameStats.attackUser.ToString();
            textShield.text = GameStats.shildUser.ToString();
            textCounterattack.text = GameStats.conterAttackUser.ToString();
        }

        public void ClearAll()
        {
            diceCubes.Clear();
            ClearDictionaryResult();
            ResetPlayerDiceResultsToGameStats();
        }
        private void ClearDictionaryResult()
        {
            diceResultsDict["Movement"] = 0;
            diceResultsDict["Attack"] = 0;
            diceResultsDict["Shield"] = 0;
            diceResultsDict["Counterattack"] = 0;
        }
        private void ResetPlayerDiceResultsToGameStats()
        {
            GameStats.diceMovement = 0;
            GameStats.diceAttack = 0;
            GameStats.diceShield = 0;
            GameStats.diceCounterattack = 0;
        }


        private void ValidateScriptsAndObjects()
        {
            if (uiResultDicePrefab == null)
                Debug.LogError($" для {this.name} Не назначен префаб UiResultDice в инспекторе!");
            if (mane == null)
                Debug.LogError($" для {this.name} Не назначен префаб mane в инспекторе!");

            if (textMovment == null)
                Debug.LogError($" для {this.name} Не назначен префаб textMovment в инспекторе!");
            if (textAttack == null)
                Debug.LogError($" для {this.name} Не назначен префаб textAttack в инспекторе!");
            if (textShield == null)
                Debug.LogError($" для {this.name} Не назначен префаб textShield в инспекторе!");
            if (textCounterattack == null)
                Debug.LogError($" для {this.name} Не назначен префаб textCounterattack в инспекторе!");

            if (resultPosition == null)
                Debug.LogError($" для {this.name} Не назначен префаб resultPosition в инспекторе!");
            if (UI_Particals_M == null)
                Debug.LogError($" для {this.name} Не назначен префаб UI_Particals_M в инспекторе!");
            if (UI_Particals_A == null)
                Debug.LogError($" для {this.name} Не назначен префаб UI_Particals_A в инспекторе!");
            if (UI_Particals_S == null)
                Debug.LogError($" для {this.name} Не назначен префаб UI_Particals_S в инспекторе!");
            if (UI_Particals_C == null)
                Debug.LogError($" для {this.name} Не назначен префаб UI_Particals_C в инспекторе!");

            if (spawnPointMovment == null)
                Debug.LogError($" для {this.name} Не назначен префаб spawnPointMovment в инспекторе!");
            if (spawnPointAttack == null)
                Debug.LogError($" для {this.name} Не назначен префаб spawnPointAttack в инспекторе!");
            if (spawnPointShield == null)
                Debug.LogError($" для {this.name} Не назначен префаб spawnPointShield в инспекторе!");
            if (spawnPointCounterattack == null)
                Debug.LogError($" для {this.name} Не назначен префаб spawnPointCounterattack в инспекторе!");

            if (targetPointMovment == null)
                Debug.LogError($" для {this.name} Не назначен префаб targetPointMovment в инспекторе!");
            if (targetPointAttack == null)
                Debug.LogError($" для {this.name} Не назначен префаб targetPointAttack в инспекторе!");
            if (targetPointShield == null)
                Debug.LogError($" для {this.name} Не назначен префаб targetPointShield в инспекторе!");
            if (targetPointCounterattack == null)
                Debug.LogError($" для {this.name} Не назначен префаб targetPointCounterattack в инспекторе!");
        }
    }
}