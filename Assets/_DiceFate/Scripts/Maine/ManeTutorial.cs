using DiceFate;
using DiceFate.Dice;
using DiceFate.EventBus;
using DiceFate.Events;
using DiceFate.MouseW;
using DiceFate.UI;
using DiceFate.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DiceFate.Maine
{
    public class ManeTutorial : MonoBehaviour
    {
        [Header("Параметры игрока")]
        [SerializeField] private int numberDiceToDrop = 3;


        [Header("Фаза 1 настройки")]
        [SerializeField] private GameObject unitMasterCard;
        [SerializeField] private PrologScenario prologScenario;
        [SerializeField] private Button dice;

        [Header("Фаза 2 настройки")]
        [SerializeField] private Button buttonKeg;
        [SerializeField] private Button buttonBackToPhaseOne;
        [SerializeField] private GameObject diceGroup;

        [Header("Фаза 3 настройки")]
        [SerializeField] private GameObject kegAndDice; // Группа на сцене для Боченка и кубиков
        [SerializeField] private Transform spawnPointKeg;
        [SerializeField] private Transform spawnPointMovement;
        [SerializeField] private Transform spawnPointAttac;
        [SerializeField] private Transform spawnPointShield;
        [SerializeField] private Transform spawnPointCounterattack;
        [SerializeField] private GameObject prefabKeg;
        [SerializeField] private GameObject prefabDiceMovement;
        [SerializeField] private GameObject prefabDiceAttack;
        [SerializeField] private GameObject prefabDiceShield;
        [SerializeField] private GameObject prefabDiceCounterattack;
        [SerializeField] private float moveSpeedToMouse = 30f;

        [SerializeField] private float prefabScale = 0.2f;
        [SerializeField] private float spacingBetweenDice = 0.5f;     // Расстояние между кубиками одного типа
        [SerializeField] private KegCylinderVirtual kegCylinderSystem; // Ссылка виртуальный цылиндр
        [SerializeField] private UiDropTargetField uiDropTargetField;     // Ссылка на поле с кубиками

        [Header("Фаза 3 настройки")]
        [SerializeField] private UI_ManeTutorial uiManeTutorial;      


        [Header("Доп. настройки")]
        [SerializeField] private Button buttonReturne;
        [SerializeField] private PhaseNumberText PhaseNumberText;
        [SerializeField] private CursorTutorial cursorTutorial;



        private bool isDiceGroupActive = false;
        private ISelectable selectedUnit; // Текущий выделенный юнит который имеет интерфейс ISelectable
        private ISelectable selectedEnemy;

        // Словарь для хранения префабов по типам кубиков
        private Dictionary<UiDragAndDropDice.DiceType, GameObject> uiDicePrefabs;
        // Словарь для хранения точек спавна по типам кубиков
        private Dictionary<UiDragAndDropDice.DiceType, Transform> uiSpawnPoints;

        private void Awake()
        {
            Bus<UnitSelectedEvent>.OnEvent += HandelUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent += HandeleUnitDeselect;
            Bus<EnemySelectedEvent>.OnEvent += HandelEnemySelected;
            Bus<EnemyDeselectedEvent>.OnEvent += HandelEnemyDeselected;
            Bus<OnDropEvent>.OnEvent += HandelDropDice;
            Bus<OnMoveEvent>.OnEvent += HandelMove;
            Bus<OnDamageEvent>.OnEvent += HandelDamageEvent;
            Bus<OnTestingEvent>.OnEvent += TestingEvent;



            buttonReturne.onClick.AddListener(ClickButtoneToReturne); // подписывается на щелчок
            dice.onClick.AddListener(PhaseTwoSelectedUnit); // подписывается на щелчок
            buttonKeg.onClick.AddListener(PhaseThreeSelectedUnit); // подписывается на щелчок
            buttonBackToPhaseOne.onClick.AddListener(BackToPhaseOne);

        }

        private void OnDestroy()
        {
            Bus<UnitSelectedEvent>.OnEvent -= HandelUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent -= HandeleUnitDeselect;
            Bus<EnemySelectedEvent>.OnEvent -= HandelEnemySelected;
            Bus<EnemyDeselectedEvent>.OnEvent -= HandelEnemyDeselected;
            Bus<OnDropEvent>.OnEvent -= HandelDropDice;
            Bus<OnMoveEvent>.OnEvent -= HandelMove;
            Bus<OnDamageEvent>.OnEvent -= HandelDamageEvent;
            Bus<OnTestingEvent>.OnEvent -= TestingEvent;

            buttonReturne.onClick.RemoveListener(ClickButtoneToReturne);
            dice.onClick.RemoveListener(PhaseTwoSelectedUnit);
            buttonKeg.onClick.RemoveListener(PhaseThreeSelectedUnit);
            buttonBackToPhaseOne.onClick.RemoveListener(BackToPhaseOne);
        }



        private void Update()
        {
            PhaseNumberText.UpdatePhaseNumberText(GameStats.currentPhasePlayer);
        }

        private void Start()
        {
            GameStats.numberDiceToDrop = numberDiceToDrop;

            ValidateScripts();

            BattleToBegin(); // временно устанавливает что бой начался

            // Инициализируем словари
            UIInitializeDicePrefabs();
            UIInitializeSpawnPoints();

            HideAllScenrElements();
            HideAllUIElements();

            kegCylinderSystem.cylinderFollowSpeed = moveSpeedToMouse;

        }

        // Инициализация словаря префабов
        private void UIInitializeDicePrefabs()
        {
            uiDicePrefabs = new Dictionary<UiDragAndDropDice.DiceType, GameObject>
            {
                { UiDragAndDropDice.DiceType.Movement, prefabDiceMovement },
                { UiDragAndDropDice.DiceType.Attack, prefabDiceAttack },
                { UiDragAndDropDice.DiceType.Shield, prefabDiceShield },
                { UiDragAndDropDice.DiceType.Counterattack, prefabDiceCounterattack }
            };

            // Проверяем, что все префабы установлены
            foreach (var kvp in uiDicePrefabs)
            {
                if (kvp.Value == null)
                {
                    Debug.LogError($"Не установлен префаб для кубика типа: {kvp.Key}");
                }
            }
        }

        // Инициализация словаря точек спавна
        private void UIInitializeSpawnPoints()
        {
            uiSpawnPoints = new Dictionary<UiDragAndDropDice.DiceType, Transform>
            {
                { UiDragAndDropDice.DiceType.Movement, spawnPointMovement },
                { UiDragAndDropDice.DiceType.Attack, spawnPointAttac },
                { UiDragAndDropDice.DiceType.Shield, spawnPointShield },
                { UiDragAndDropDice.DiceType.Counterattack, spawnPointCounterattack }
            };

            // Проверяем, что все точки спавна установлены
            foreach (var kvp in uiSpawnPoints)
            {
                if (kvp.Value == null)
                {
                    Debug.LogError($"Не установлена точка спавна для кубика типа: {kvp.Key}");
                }
            }
        }



        private void SetSceneElementsVisible(bool isKegAndDice)
        {
            kegAndDice.SetActive(isKegAndDice);
        }

        private void HideAllScenrElements() => SetSceneElementsVisible(false);
        private void ShowPhaseThreeSceneElements() => SetSceneElementsVisible(true);

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

        private void Phase(int number) => GameStats.currentPhasePlayer = number;

        //---------------------------------- События  ----------------------------------
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

        private void HandelEnemySelected(EnemySelectedEvent evt) => selectedEnemy = evt.Enemy; // Обновить текущий выделенный Враг      
        private void HandelEnemyDeselected(EnemyDeselectedEvent evt) => selectedEnemy = null;  // Сбросить текущий выделенный Враг 

        private void HandelDropDice(OnDropEvent evt)
        {
            switch (GameStats.currentPhasePlayer)
            {
                case 3:
                    PhaseFourSelectedUnit();
                    break;
            }
        }
        private void HandelMove(OnMoveEvent args) => PhaseFiveSelectedUnit();
        private void HandelDamageEvent(OnDamageEvent damageObject) => PhaseSevenSelectedUnit(damageObject);

        //   PhaseSixSelectedUnit//


        private void TestingEvent(OnTestingEvent evt)
        {
            // Запуск любых методов для теста пл W
            ClearAllCreatedDice();
        }

        //-------------- Ход игрока --------------
        private void BattleToBegin()
        {
            GameStats.isBattle = true;
        }
        private void BattleToEnd()
        {
            GameStats.isBattle = false;
        }

        //-------------- 1 Фаза - Выбрать игрока --------------
        public void PhaseOneSelectedUnit()
        {
            //if (GameStats.currentUser != "Player")
            //    return;

      

            if (prologScenario.prologNumber >= 4 || prologScenario.isTesting == false)
            {
                Phase(1);
                uiManeTutorial.UiShowMasterCadrDependPrologNumber();

                MovementAndGridEnable();                
            }

            //ShowPhaseOneUIElements();
            Debug.Log($"Фаза = {GameStats.currentPhasePlayer}");
        }

        //-------------- 2 Фаза - Активация кубиков для выбора --------------
        public void PhaseTwoSelectedUnit()
        {
            //if (GameStats.currentUser != "Player")
            //    return;

            Phase(2);
            Bus<OnIsActiveGridEvent>.Raise(new OnIsActiveGridEvent(false));
            uiManeTutorial.UiShowPhaseTwoPlayer();

            //ShowPhaseTwoUIElements();
            Debug.Log($"Фаза = {GameStats.currentPhasePlayer}");
        }

        public void BackToPhaseOne()
        {
            uiManeTutorial.HideAllUIElements();
            cursorTutorial.StopAnimation();
            PhaseOneSelectedUnit();
        }

        //-------------- 3 Фаза Бросить кости выбранные кости --------------
        public void PhaseThreeSelectedUnit()
        {
            //if (GameStats.currentUser != "Player")
            //    return;

            Phase(3);
            cursorTutorial.StopAnimation();
            uiManeTutorial.UiShowPhaseThreePlayer();
            prologScenario.StartScenarioSeven();



            //ShowPhaseThreeUIElements();
            ShowPhaseThreeSceneElements();
            CreatePrefabKeg();
            kegCylinderSystem.EnableVirtualCylinder();
            CreateDiceFromField();
            kegCylinderSystem.AddDiceToList();
            Debug.Log($"Фаза = {GameStats.currentPhasePlayer}");

        }

        // Создание кубиков из списка кубиков на поле
        private void CreateDiceFromField()
        {
            // Получаем все кубики с поля
            List<UiDragAndDropDice> diceOnField = uiDropTargetField.GetDiceOnField();

            if (diceOnField == null || diceOnField.Count == 0)
            {
                Debug.Log("На поле нет кубиков для создания"); return;
            }

            Debug.Log($"Создание {diceOnField.Count} кубика(ов) из поля...");

            // Создаем кубики по типам
            Dictionary<UiDragAndDropDice.DiceType, int> typeCounters = new Dictionary<UiDragAndDropDice.DiceType, int>
            {
                { UiDragAndDropDice.DiceType.Movement, 0 },
                { UiDragAndDropDice.DiceType.Attack, 0 },
                { UiDragAndDropDice.DiceType.Shield, 0 },
                { UiDragAndDropDice.DiceType.Counterattack, 0 }
            };

            foreach (var dice in diceOnField)
            {
                if (dice == null) continue;

                UiDragAndDropDice.DiceType diceType = dice.GetDiceType();

                // Увеличиваем счетчик для этого типа
                typeCounters[diceType]++;

                // Создаем кубик этого типа
                CreateDiceByType(diceType, 1);
            }

            // Выводим статистику
            foreach (var kvp in typeCounters)
            {
                if (kvp.Value > 0)
                {
                    Debug.Log($"Создано {kvp.Value} кубика(ов) типа {kvp.Key}");
                }
            }

            uiDropTargetField.ResetAllDiceAndClearField(); // Сбросить все кубики с поля после создания
        }
        // Метод для создания кубиков определенного типа
        private void CreateDiceByType(UiDragAndDropDice.DiceType diceType, int count)
        {
            if (count <= 0) return;

            if (!uiDicePrefabs.TryGetValue(diceType, out GameObject prefab) || prefab == null)
            { Debug.LogError($"Не найден префаб для типа кубика: {diceType}"); return; }

            if (!uiSpawnPoints.TryGetValue(diceType, out Transform spawnPoint) || spawnPoint == null)
            { Debug.LogError($"Не найдена точка спавна для типа кубика: {diceType}"); return; }

            // Получаем префаб и точку спавна для этого типа
            for (int i = 0; i < count; i++)
            {
                CreateSingleDice(prefab, spawnPoint, diceType, i);
            }
        }

        // Метод для создания одного кубика
        private void CreateSingleDice(GameObject prefab, Transform baseSpawnPoint, UiDragAndDropDice.DiceType diceType, int index)
        {
            // Создаем экземпляр префаба
            GameObject spawnedObject = Instantiate(prefab);

            // Рассчитываем позицию с учетом индекса (для нескольких кубиков одного типа)
            Vector3 positionOffset = Vector3.right * (index * spacingBetweenDice);
            Vector3 spawnPosition = baseSpawnPoint.position + positionOffset;

            // Устанавливаем позицию и поворот
            spawnedObject.transform.position = spawnPosition;
            spawnedObject.transform.rotation = baseSpawnPoint.rotation;

            // Устанавливаем масштаб префаба
            spawnedObject.transform.localScale = new Vector3(prefabScale, prefabScale, prefabScale);

            // Устанавливаем родителя
            spawnedObject.transform.SetParent(baseSpawnPoint);

            // Устанавливаем скорость перемещения Кубика с боченком к мыши  (для исключения вылиатания кубика сквозь боченок)
            DiceCube diceCubeValue = prefab.GetComponent<DiceCube>();
            diceCubeValue.moveSpeed = moveSpeedToMouse;

            // Настраиваем компонент кубика (если есть)
            var diceComponent = spawnedObject.GetComponent<UiDragAndDropDice>();
            if (diceComponent != null)
            {
                diceComponent.diceType = diceType;
            }
            Debug.Log($"Создан кубик типа {diceType} #{index + 1} в позиции {spawnPosition}");
        }

        private void CreatePrefabKeg()
        {
            GameObject spawnedObject = Instantiate(prefabKeg);

            spawnedObject.transform.position = spawnPointKeg.position;
            spawnedObject.transform.rotation = spawnPointKeg.rotation;
            //spawnedObject.transform.localScale = new Vector3(prefabScale, prefabScale, prefabScale);

            spawnedObject.transform.SetParent(spawnPointKeg);  // Устанавливаем родителя 

            KegTremble kegScript = prefabKeg.GetComponent<KegTremble>();
            kegScript.moveSpeed = moveSpeedToMouse;
        }



        // Метод для очистки всех созданных кубиков
        public void ClearAllCreatedDice()
        {
            // Очищаем все дочерние объекты точек спавна
            ClearChildren(spawnPointMovement);
            ClearChildren(spawnPointAttac);
            ClearChildren(spawnPointShield);
            ClearChildren(spawnPointCounterattack);
            ClearChildren(spawnPointKeg);
        }

        private void ClearChildren(Transform parent)
        {
            if (parent == null) return;

            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }

        //--------------------------- 4 Фаза  -------------------------

        public void PhaseFourSelectedUnit()
        {
            //if (GameStats.currentUser != "Player")
            //    return;
            Phase(4);
            GameStats.isPlayerMoveble = false; // Блокируем движение игрока пока кубики не остановятся
            G.isLeftClickBlock = false;

            uiManeTutorial.UiShowPhaseFourPlayer();
            uiManeTutorial.UiClearAllDiceOnField(); // Очистить список кубиков на платке
            uiManeTutorial.TestingListToDiceTargetResult();
            prologScenario.StartScenarioEight();

            //Получаем значение кубиков на столе            
            // StartCoroutine(PhaseFourCoroutine());
            // uiDropTargetField.ResetAllDiceAndClearField();

            Debug.Log($"Фаза = {GameStats.currentPhasePlayer}");
        }

        public void MovementAndGridEnable()
        {
            GameStats.isPlayerMoveble = true;
            // int movementDiceCount = GameStats.diceMovement; // Получаем количество кубиков движения из GameStats
            int movementDiceCount = GameStats.moveUser; // Получаем количество кубиков движения из GameStats

            Bus<OnMovmentValueEvent>.Raise(new OnMovmentValueEvent(movementDiceCount));
            Bus<OnGridEvent>.Raise(new OnGridEvent(1)); // Сообщаем что можновключить сетку для движения
            //G.isLeftClickBlock = true;
        }

        private IEnumerator PhaseFourCoroutine()
        {
            //Ждем, пока кубики остановятся и показываем результат в UI
            //yield return new WaitForSeconds(4f);
            //uiMane.UiEnableResultDisplay();

            // Записываем результат на карточку и в static
            yield return new WaitForSeconds(5f);
            //uiMane.SetResultToCard();
            MovementValueFromDice(); // Назначить количество очков движений

            //yield return new WaitForSeconds(1f);
            //uiMane.UiDisableResultDisplay();

            //Bus<OnGridEvent>.Raise(new OnGridEvent(1)); // Сообщаем что можновключить сетку для движения
            //GameStats.isPlayerMoveble = true;

        }

        private void MovementValueFromDice()
        {
            // Получаем количество кубиков движения из GameStats
            int movementDiceCount = GameStats.diceMovement;
            Bus<OnMovmentValueEvent>.Raise(new OnMovmentValueEvent(movementDiceCount));
        }


        //-------------- 5 Фаза (перемещение фигурки)  --------------

        public void PhaseFiveSelectedUnit()
        {
            Phase(5);
            // uiMane.UiShowPhaseFivePlayer();
            GameStats.isPlayerMoveble = false; // Блокируем движение игрока 

            RemoveDiceOnScene();


            //uiMane.UiDisplayClearAll(); // Очистить UI от старых кубиков и сбросить значения на 0 -  yt hf,jnftn

            Debug.Log($"Фаза = {GameStats.currentPhasePlayer}");
        }

        private void RemoveDiceOnScene()
        {
            GameObject[] allDices = GameObject.FindGameObjectsWithTag("Cube");

            foreach (GameObject dice in allDices)
            {
                Destroy(dice);
            }

        }

        //--------------------------- 6 Фаза наведение на противника  -------------------------
        public void PhaseSixSelectedUnit(OnDamageEvent damageObject)
        {
            Phase(6);
            GameStats.isPlayerMoveble = false; // Блокируем движение игрока 


            Debug.Log($"Фаза = {GameStats.currentPhasePlayer}");
        }

        //-------------------------- 7 Фаза - Атака  и Завершение хода игрока  ------------------------
        public void PhaseSevenSelectedUnit(OnDamageEvent damageObject)
        {
            Phase(7);
            GameStats.isPlayerMoveble = false; // Блокируем движение игрока 

            // uiMane.UiShowPhaseSixPlayer();


            if (selectedUnit != null)
            {
                // selectedUnit.HoverBeforAttack(); // Поднять юнита для атаки через интерфейс
                selectedUnit.JumpBeforeAttack(); // Поднять юнита для атаки через интерфейс
            }

            // Наносим урон через интерфейс

            Debug.Log($"Фаза = {GameStats.currentPhasePlayer}");

        }






        //-------------------------- 8 Фаза - Ожидание контр атаки  ------------------------




        // -------------------------- Enemy  Защита и контратака  ------------------------





        // -------------------------- Enemy  ход  ------------------------





        //-------------- Вспомогательные методы --------------
        public void ClickButtoneToReturne()
        {
            switch (GameStats.currentPhasePlayer)
            {
                case 0:
                    // Ничего не делаем
                    break;
                case 1:
                    DeselectPlayerUnit();
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
                case 5:
                    PhaseFourSelectedUnit();
                    break;
                case 6:
                    PhaseFiveSelectedUnit();
                    break;
                default:
                    Debug.Log($"Неизвестная фаза: {GameStats.currentPhasePlayer}");
                    break;
            }
        }

        // Метод для снятия выделения
        public void DeselectPlayerUnit()
        {
            if (selectedUnit != null)
            {
                selectedUnit.Deselect();
                selectedUnit = null;
            }
        }


        // проверка установки скриптов  
        private void ValidateScripts()
        {
            if (uiDropTargetField == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на DropTargetField!");
            if (kegCylinderSystem == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на KegCylinderSystem!");
            if (uiManeTutorial == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на UI_Mane!");
            if (prologScenario == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на prologScenario!");
        }
    }
}