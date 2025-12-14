using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using DiceFate.Dice;
using DiceFate.UI;
using DiceFate.Units;
using DiceFate.Events;
using DiceFate.EventBus;
using System.Collections;

namespace DiceFate.Maine
{
    public class Mane : MonoBehaviour
    {
        [Header("Фаза 1 настройки")]
        [SerializeField] private GameObject unitMasterCard;
        [SerializeField] private Button dice;

        [Header("Фаза 2 настройки")]
        [SerializeField] private Button keg;
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
        [SerializeField] private KegCylinderSystem kegCylinderSystem; // Ссылка виртуальный цылиндр
        [SerializeField] private UiDropTargetField uiDropTargetField;     // Ссылка на поле с кубиками

        [Header("Фаза 3 настройки")]
        [SerializeField] private UI_Mane uiMane;


        [Header("Доп. настройки")]
        [SerializeField] private Button buttonReturne;

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
            Bus<OnTestingEvent>.OnEvent += TestingEvent;


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
            Bus<OnDropEvent>.OnEvent -= HandelDropDice;
            Bus<OnTestingEvent>.OnEvent -= TestingEvent;

            buttonReturne.onClick.RemoveListener(ClickButtoneToReturne);
            dice.onClick.RemoveListener(PhaseTwoSelectedUnit);
            keg.onClick.RemoveListener(PhaseThreeSelectedUnit);
        }

        private void Start()
        {
            ValidateScripts();

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

        // проверка установки скриптов  
        private void ValidateScripts()
        {
            if (uiDropTargetField == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на DropTargetField!");

            if (kegCylinderSystem == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на KegCylinderSystem!");

            if (uiMane == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на UI_Mane!");
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

        private void Phase(int number) => PhaseNumber.currentPhase = number;

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

        private void HandelEnemySelected(EnemySelectedEvent evt)
        {
            selectedEnemy = evt.Enemy; // Обновить текущий выделенный Враг
        }

        private void HandelEnemyDeselected(EnemyDeselectedEvent evt)
        {
            selectedEnemy = null;  // Сбросить текущий выделенный Враг          
        }

        private void HandelDropDice(OnDropEvent evt)
        {
            PhaseFourSelectedUnit();
        }



        private void TestingEvent(OnTestingEvent evt)
        {
            // Запуск любых методов для теста пл W
            ClearAllCreatedDice();
        }

        //-------------- Ход игрока --------------

        //-------------- 1 Фаза - Выбрать игрока --------------
        public void PhaseOneSelectedUnit()
        {
            Phase(1);
            ShowPhaseOneUIElements();
            Debug.Log($"Фаза = {PhaseNumber.currentPhase}");
        }

        //-------------- 2 Фаза - Активация кубиков для выбора --------------
        public void PhaseTwoSelectedUnit()
        {
            Phase(2);
            ShowPhaseTwoUIElements();
            Debug.Log($"Фаза = {PhaseNumber.currentPhase}");
        }

        //-------------- 3 Фаза Бросить кости выбранные кости --------------
        public void PhaseThreeSelectedUnit()
        {
            Phase(3);
            ShowPhaseThreeUIElements();
            ShowPhaseThreeSceneElements();
            CreatePrefabKeg();
            CreateDiceFromField();
            kegCylinderSystem.AddDiceToList();
            Debug.Log($"Фаза = {PhaseNumber.currentPhase}");

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
            DiceCubeValue diceCubeValue = prefab.GetComponent<DiceCubeValue>();
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

        //-------------- 4 Фаза  --------------

        public void PhaseFourSelectedUnit()
        {
            Phase(4);

            //Получаем значение кубиков на столе            
            StartCoroutine(PhaseFourCoroutine());





            Debug.Log($"Фаза = {PhaseNumber.currentPhase}");
        }

        private IEnumerator PhaseFourCoroutine()
        {
            //Ждем, пока кубики остановятся
            yield return new WaitForSeconds(4f);
            uiMane.UiEnableResultDisplay();

            yield return new WaitForSeconds(1f);
            uiMane.UiSetResultToCard();

            yield return new WaitForSeconds(1f);
            uiMane.UiDisableResultDisplay();
        }





        //-------------- 5 Фаза  --------------



        //-------------- Вспомогательные методы --------------
        public void ClickButtoneToReturne()
        {
            switch (PhaseNumber.currentPhase)
            {
                case 0:
                    // Ничего не делаем
                    break;
                case 1:
                    if (selectedUnit != null)
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
                case 5:
                    PhaseFourSelectedUnit();
                    break;
                default:
                    Debug.Log($"Неизвестная фаза: {PhaseNumber.currentPhase}");
                    break;
            }
        }

        //// Метод для тестирования создания кубиков
        //public void TestCreateDices()
        //{
        //    // Пример: создаем тестовый набор кубиков
        //    Debug.Log("Тестовое создание кубиков...");

        //    // Создаем по одному кубику каждого типа
        //    CreateDiceByType(DragAndDropDice.DiceType.Movement, 1);
        //    CreateDiceByType(DragAndDropDice.DiceType.Attack, 2);
        //    CreateDiceByType(DragAndDropDice.DiceType.Shield, 1);
        //    CreateDiceByType(DragAndDropDice.DiceType.Counterattack, 0);
        //}

        //// Метод для проверки правильности создания кубиков
        //public void ValidateCreatedDices()
        //{
        //    int totalCreated = CountCreatedDices();
        //    Debug.Log($"Всего создано кубиков: {totalCreated}");

        //    // Подсчет по типам
        //    int movementCount = CountDicesByType(DragAndDropDice.DiceType.Movement);
        //    int attackCount = CountDicesByType(DragAndDropDice.DiceType.Attack);
        //    int shieldCount = CountDicesByType(DragAndDropDice.DiceType.Shield);
        //    int counterattackCount = CountDicesByType(DragAndDropDice.DiceType.Counterattack);

        //    Debug.Log($"Создано Movement: {movementCount}");
        //    Debug.Log($"Создано Attack: {attackCount}");
        //    Debug.Log($"Создано Shield: {shieldCount}");
        //    Debug.Log($"Создано Counterattack: {counterattackCount}");
        //}

        //private int CountCreatedDices()
        //{
        //    int count = 0;
        //    count += spawnPointMovement?.childCount ?? 0;
        //    count += spawnPointAttac?.childCount ?? 0;
        //    count += spawnPointShield?.childCount ?? 0;
        //    count += spawnPointCounterattack?.childCount ?? 0;
        //    return count;
        //}

        //private int CountDicesByType(DragAndDropDice.DiceType diceType)
        //{
        //    Transform spawnPoint = null;
        //    switch (diceType)
        //    {
        //        case DragAndDropDice.DiceType.Movement:
        //            spawnPoint = spawnPointMovement;
        //            break;
        //        case DragAndDropDice.DiceType.Attack:
        //            spawnPoint = spawnPointAttac;
        //            break;
        //        case DragAndDropDice.DiceType.Shield:
        //            spawnPoint = spawnPointShield;
        //            break;
        //        case DragAndDropDice.DiceType.Counterattack:
        //            spawnPoint = spawnPointCounterattack;
        //            break;
        //    }

        //    return spawnPoint?.childCount ?? 0;
        //}
    }
}