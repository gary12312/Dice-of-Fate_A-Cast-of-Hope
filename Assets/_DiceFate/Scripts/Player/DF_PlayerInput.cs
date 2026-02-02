using DiceFate.EventBus;
using DiceFate.Events;
using DiceFate.Maine;
using DiceFate.Units;
using System;
using System.Drawing;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


namespace DiceFate.Player
{
    public class DF_PlayerInput : MonoBehaviour
    {
        [Header("Камера")]
        [SerializeField] private new Camera camera;
        [SerializeField] private Rigidbody cameraTarget;
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private LayerMask floorLayers;
        [SerializeField] private LayerMask movmentLayers;
        [SerializeField] private LayerMask enemyLayers;
        [SerializeField] private LayerMask lootLayers;

      

        //private CinemachineFollow cinemachineFollow;
        //private Vector3 startingFollowOffset;
        private Vector3 pointMousePosition;
        private Vector2 mouseDelta;       // Изменение положения мыши
        private bool isWasMouseDownUI;   //  был ли клик по UI

        private ISelectable selectedUnit; // Текущий выделенный юнит который имеет интерфейс Vector3 unitPosition 
        private Vector3 selectUnitPosition;

        private ISelectableForVisibleUi selectableObjectForUi; // Текущий выделенный юнит который имеет интерфейс ISelectableForVisibleUi
        //private List<ISelectableDice> selectableDice = new(10); //емкость для выделенных кубиков
        //private ISelectableDice selectableDiceTest;  //емкость для выделенных кубиков
        private IHover hoverableUnit;     // Текущее наведение юнит
        private IMoveable moveable;       // Текущий перемещаемый юнит

        // Управление частотой обновления
        [SerializeField] private float updateInterval = 0.05f;
        private float accumulatedTime;



        // провкрить и удалить если не нужно
        private Vector3 previousMousePosition;
        private float totalDistanceMoved = 0f;
        private bool isFirstShakeCall = true;


        // Дополнительные параметры для управления камерой
        [Header("Настройки движения камеры")]
        [SerializeField] private float cameraMoveSpeedX = 2.1f; // Скорость движения камеры
        [SerializeField] private float cameraMoveSpeedY = 4f; // Скорость движения камеры
        [SerializeField] private float cameraRotationSpeed = 10f; // Скорость вращения камеры

        private bool isDragging; // Флаг, что сейчас идёт выделение

        private void Awake()
        {
            Bus<UnitSelectedEvent>.OnEvent += HandelUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent += HandeleUnitDeselect;
            Bus<OnSelectedForUiEvent>.OnEvent += HandelSelectedForUi;
            Bus<OnDeselectedForUiEvent>.OnEvent += HandeleDeselectForUi;
        }

        private void OnDestroy()
        {
            Bus<UnitSelectedEvent>.OnEvent -= HandelUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent -= HandeleUnitDeselect;
            Bus<OnSelectedForUiEvent>.OnEvent -= HandelSelectedForUi;
            Bus<OnDeselectedForUiEvent>.OnEvent -= HandeleDeselectForUi;
        }

        private void Start() => ValidateScripts();
      
        private void Update()
        {
            MiddleClickAndHold();  // Обработка центрального клика (поворот)
            RightClickAndHold();   // Обработка правого клика (перемещение)
            LeftClick();           // Обработка правого клика

            // UpdateForHoverable();  // Уменьшение частоты Обновление FPS - для Outline
            IsMouseOnObjectOrNot(); // Без уменьшения частоты - оставить чтото одно после теста прозводительности

            if (Input.GetKeyDown(KeyCode.W))     
                Bus<OnTestingEvent>.Raise(new OnTestingEvent(1));  // для теста       
        }

        //---------------------------------- Наведение мышы  ---------------------------------------------------------

        private void HandelMpuseDown()
        {
            isWasMouseDownUI = EventSystem.current.IsPointerOverGameObject();
        }

        //---------------------------------- Клики мышы  -------------------------------------------------------------
        private void MiddleClickAndHold()
        {
            if (Mouse.current.middleButton.isPressed) { RotationCamera(); }
        }

        private void RightClickAndHold()
        {
            if (cameraTarget == null) return;

            if (Mouse.current.rightButton.isPressed)
            {
                MoveCamera();
            }
            else
            {
                // Останавливаем камеру при отпускании кнопки
                cameraTarget.linearVelocity = Vector3.zero;
            }
        }

        private void LeftClick()
        {
          
            if (G.isLeftClickBlock) { return; }

            switch (GameStats.currentPhasePlayer)
            {
                case 0:
                    LeftClickToHandleSelectionAndAttack();
                    break;

                case 1:
                    LeftClickToHandleSelectionAndAttack();
                    break;

                case 3: // Фаза броска кубиков                   
                    LeftClickToShakeAndDropDice();
                    break;

                case 4: // Фаза перемещения юнита
                    
                    LeftClickToMove();
                    //LeftClickSelectOther();
                    break;
                case 5: // Фаза атаки юнита
                    LeftClickToHandleSelectionAndAttack();
                    break;

                default:
                    break;
            }
        }

        private void LeftClickToHandleSelectionAndAttack()
        {           
            if (EventSystem.current.IsPointerOverGameObject())  // Проверяем, был ли клик по UI элементу
                return;      

            if (!Mouse.current.leftButton.wasPressedThisFrame) return;

            Ray cameraRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            // Определяем слои для проверки в зависимости от фазы игры
            LayerMask targetLayers = GameStats.currentPhasePlayer switch
            {
                5 => enemyLayers | lootLayers, // Фаза атаки: враги + лут
                _ => LayerMask.GetMask("Default") // Обычное выделение
            };

            // Проверяем луч на целевые слои
            if (Physics.Raycast(cameraRay, out hit, float.MaxValue, targetLayers))
            {
                // Обработка врагов в фазе атаки
                if (GameStats.currentPhasePlayer == 5 &&
                    ((1 << hit.collider.gameObject.layer) & enemyLayers) != 0)
                {
                    if (selectedUnit != null &&
                        hit.collider.TryGetComponent(out IDamageable damageable))
                    {
                        // Наносим урон
                        damageable.TakeDamage(15, selectUnitPosition);
                        DeselectCurrentUnit();
                        return;
                    }
                }
                
                if (hit.collider.TryGetComponent(out ISelectable selectable)) // ISelect
                {
                    if (selectedUnit != null && selectedUnit == selectable) return;

                    if (selectedUnit != null && selectedUnit != selectable)
                    {
                        DeselectCurrentUnit();
                    }

                    selectable.Select();
                    Bus<OnUpdateUIAvatarEvent>.Raise(new OnUpdateUIAvatarEvent(1));
                }

                if (Mouse.current.leftButton.wasPressedThisFrame)  // Loot
                {
                    if (Physics.Raycast(cameraRay, out hit, float.MaxValue, lootLayers)
                    && hit.collider.TryGetComponent(out ILooter looter))
                    {
                        looter.LootSelection();
                    }
                }
                else
                {
                    // Клик по невыделяемому объекту в целевых слоях
                    DeselectCurrentUnit();
                }
            }
            else
            {
                // Клик в пустом месте
                DeselectCurrentUnit();
            }
        }

        private void LeftClickToLoot()
        {
            if (camera == null) { return; }

            Ray cameraRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());   // Получение точки на экране
            RaycastHit hit;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {        
                if (Physics.Raycast(cameraRay, out hit, float.MaxValue, lootLayers)
                && hit.collider.TryGetComponent(out ILooter selectable))
                {
                    selectable.LootSelection();
                }
            }
        }

        // Обработка левого клика Для ВЫБОРА юнита // TargetDecal
        private void LeftClickToSelected()
        {
            if (camera == null) { return; }

            Ray cameraRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());   // Получение точки на экране
            RaycastHit hit;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // Проверяем, был ли клик по UI элементу
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return; // Не обрабатываем клик по UI
                }

                // Проверяем клик по юниту
                if (Physics.Raycast(cameraRay, out hit, float.MaxValue, LayerMask.GetMask("Default"))
                && hit.collider.TryGetComponent(out ISelectable selectable))
                {
                    if (selectedUnit != null && selectedUnit == selectable) { return; }
                    if (selectedUnit != null && selectedUnit != selectable) { DeselectCurrentUnit(); }  // Если есть уже выделенный юнит, снимаем с него выделение

                    selectable.Select();
                    Bus<OnUpdateUIAvatarEvent>.Raise(new OnUpdateUIAvatarEvent(1));

                }
                else { DeselectCurrentUnit(); } // Клик в пустом месте 
            }
        }

        // Метод для снятия выделения
        private void DeselectCurrentUnit()
        {
            if (selectedUnit != null)
            {
                selectedUnit.Deselect();
                selectedUnit = null;

                Bus<OnUpdateUIAvatarEvent>.Raise(new OnUpdateUIAvatarEvent(1));
            }
        }

        private void LeftClickToShakeAndDropDice()
        {
            if (selectedUnit == null) { return; }

            if (GameStats.currentPhasePlayer != 3) { return; }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                isDragging = true;
            }
            else if (isDragging && Mouse.current.leftButton.isPressed)
            {
                ShakeDiceIsMouseDown();
                MoveToMouseDiceAndKeg();
            }
            else if (isDragging && Mouse.current.leftButton.wasReleasedThisFrame)
            {
                DropDiceIsMouseUp();
                isDragging = false;
            }
        }

        private void LeftClickToMove()
        {
        if (camera == null || selectedUnit == null)
            { return; }

            if (isWasMouseDownUI = EventSystem.current.IsPointerOverGameObject()) // проверяем находится ли курсор над UI элементом
            { return; }


            Ray cameraRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // Тест: бросаем луч на все слои
                if (Physics.Raycast(cameraRay, out hit, float.MaxValue, LayerMask.GetMask("Default"))) //movmentLayers //LayerMask.GetMask("Default"))
                {
                    // Прямой доступ через GameObject
                    MonoBehaviour selectedMono = selectedUnit as MonoBehaviour;
                    if (selectedMono != null && selectedMono.TryGetComponent(out IMoveable moveable))
                    {
                        moveable.MoveTo(hit.point);

                        Bus<OnMoveEvent>.Raise(new OnMoveEvent(1)); //  события 5 в майне

                        selectUnitPosition = hit.point;

                    }
                }
            }
        }

        private void LeftClickToAttackOrLoot()
        {
            if (camera == null || selectedUnit == null)
            { return; }

            if (isWasMouseDownUI = EventSystem.current.IsPointerOverGameObject()) // проверяем находится ли курсор над UI элементом
            { return; }

            Ray cameraRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;


            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // Проверяем, был ли клик по UI элементу
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return; // Не обрабатываем клик по UI
                }

                // Тест: бросаем луч на слои Врага
                if (Physics.Raycast(cameraRay, out hit, float.MaxValue, enemyLayers)) //movmentLayers //LayerMask.GetMask("Default"))
                {

                    // Проверяем клик по юниту
                    if (Physics.Raycast(cameraRay, out hit, float.MaxValue, enemyLayers)
                    && hit.collider.TryGetComponent(out ISelectable selectable))
                    {
                        if (selectedUnit != null && selectedUnit == selectable) { return; } // Если клик по самому себе, ничего не делаем

                        if (hit.collider.TryGetComponent(out IDamageable damageable))
                        {

                            // Наносим урон через интерфейс
                            damageable.TakeDamage(15, selectUnitPosition);
                            //  damageable.TakeDamage(GameStats.diceAttack);
                            DeselectCurrentUnit();
                        }
                    }
                }
                else if (Physics.Raycast(cameraRay, out hit, float.MaxValue, lootLayers)
                && hit.collider.TryGetComponent(out ISelectable selectable))
                {
                    selectable.Select();
                    Bus<OnUpdateUIAvatarEvent>.Raise(new OnUpdateUIAvatarEvent(1));
                }
            }
        }




        private void LeftClickSelectOther()
        {
            if (camera == null || selectedUnit == null)
            { return; }
            if (isWasMouseDownUI = EventSystem.current.IsPointerOverGameObject()) // проверяем находится ли курсор над UI элементом
            { return; }



            Ray cameraRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // if (EventSystem.current.IsPointerOverGameObject()) { return; } // Проверяем, был ли клик по UI элементу

                // Проверяем клик по объекту
                if (Physics.Raycast(cameraRay, out hit, float.MaxValue, LayerMask.GetMask("Default"))
                && hit.collider.TryGetComponent(out ISelectableForVisibleUi selectableForUi))
                {
                    if (selectedUnit == selectableForUi) { return; }
                    if (selectableObjectForUi != null && selectableObjectForUi == selectableForUi) { return; }
                    if (selectableObjectForUi != null && selectableObjectForUi != selectableForUi) { DeselectCurrentObjectForUi(); }  // Если есть уже выделенный юнит для UI, снимаем с него выделение

                    selectableForUi.SelectForUi();
                }
                else { DeselectCurrentObjectForUi(); } // Клик в пустом месте

                if (Physics.Raycast(cameraRay, out hit, float.MaxValue, LayerMask.GetMask("Default"))
             && hit.collider.TryGetComponent(out ISelectable selectable))
                {
                    if (selectedUnit != null && selectedUnit == selectable) { return; }
                    if (selectedUnit != null && selectedUnit != selectable) { DeselectCurrentUnit(); }  // Если есть уже выделенный юнит, снимаем с него выделение

                    selectable.Select();
                }
                else { DeselectCurrentUnit(); } // Клик в пустом месте 


            }
        }

        private void DeselectCurrentObjectForUi()
        {
            if (selectableObjectForUi != null)
            {
                selectableObjectForUi.DeselectForUi();
                selectableObjectForUi = null;
            }
        }

        //---------------------------------- Функции Перемещение и вращение камеры  ----------------------------------
        private void MoveCamera()
        {
            mouseDelta = Mouse.current.delta.ReadValue();                     // Получаем изменение положения мыши
            float deltaX = -mouseDelta.x * cameraMoveSpeedX * Time.deltaTime; // сколсть по X  Инвертируем
            float deltaY = -mouseDelta.y * cameraMoveSpeedY * Time.deltaTime;

            if (camera == null) return;

            // Получаем направления камеры и игнорируем вертикальную ось
            Vector3 moveDirection =
                camera.transform.right * deltaX +
                camera.transform.forward * deltaY; // Вычисляем направление движения на основе положения мыши
            moveDirection.y = 0;

            cameraTarget.linearVelocity = moveDirection; // Применяем движение к цели камеры
        }

        // Вращение камеры за счет вращения цели камеры ( cameraTarget )
        private void RotationCamera()
        {
            mouseDelta = Mouse.current.delta.ReadValue();                       // Получаем изменение положения мыши            
            float deltaY = mouseDelta.x * cameraRotationSpeed * Time.deltaTime; // Вращаем только по горизонтали (оси Y) на основе движения мыши по X

            if (cameraTarget != null)
            {
                Vector3 currentRotation = cameraTarget.rotation.eulerAngles;    // Получаем текущее вращение cameraTarget

                float newYRotation = currentRotation.y + deltaY;                // Добавляем вращение только по оси Y
                Quaternion newRotation = Quaternion.Euler(currentRotation.x, newYRotation, currentRotation.z);  // Создаем новое вращение, сохраняя X и Z неизменными

                cameraTarget.rotation = newRotation;                            // Применяем вращение к cameraTarget
            }
        }

        //---------------------------------- События Выбор и выделение   ----------------------------------
        private void HandelUnitSelected(UnitSelectedEvent evt) => selectedUnit = evt.Unit; // Обновить текущий выделенный юнит
        private void HandeleUnitDeselect(UnitDeselectedEvent evt) => selectedUnit = null;  // Сбросить текущий выделенный юнит 
        private void HandeleDeselectForUi(OnDeselectedForUiEvent args) => selectableObjectForUi = null;
        private void HandelSelectedForUi(OnSelectedForUiEvent args) => selectableObjectForUi = args.ObjectOnScene;

        //---------------------------------- Обработка наведения мыши  ----------------------------

        // Уменьшаем частоту обновления
        private void UpdateForHoverable()
        {
            accumulatedTime += Time.deltaTime;

            if (accumulatedTime >= updateInterval)
            {
                IsMouseOnObjectOrNot();
                accumulatedTime = 0;
            }
        }

        // Проверяем, находится ли мышь над объектом с IHover
        private void IsMouseOnObjectOrNot()
        {
            Ray cameraRay = camera.ScreenPointToRay(Input.mousePosition);

            // Проверяем, закрываеи ли  UI элемент
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return; // Не обрабатываем наведение через UI
            }

            // Если луч попал в объект и у него есть компонент IHover или ISelectable
            if (Physics.Raycast(cameraRay, out RaycastHit hit, 100) && hit.collider.TryGetComponent(out IHover hover))
            {

                if (hoverableUnit != null && hoverableUnit == hover) //|| selectedUnit != null
                {
                    return; // Если уже есть наведение, ничего не делаем
                }


                hover.OnEnterHover();  // Включаем обводку через интерфейс IHover 
                hoverableUnit = hover; // Сохраняем текущий наведение юнит
                Bus<OnUpdateUIAvatarEvent>.Raise(new OnUpdateUIAvatarEvent(1));

            }            
            else if (hoverableUnit != null) // Если луч не попал в объект с IHover, снимаем обводку
            {
                hoverableUnit.OnExitHover(); // Снимаем обводку через интерфейс IHover 
                hoverableUnit = null;        // Сбрасываем текущее наведение юнит
                Bus<OnUpdateUIAvatarEvent>.Raise(new OnUpdateUIAvatarEvent(1));
            }

        }

        //---------------------------------- Тряска и бросание кубиков  ----------------------------------
        public void DropDiceIsMouseUp() => Bus<OnDropEvent>.Raise(new OnDropEvent(500));     

        public Vector3? GetPointOnPlaneFromMouse()
        {
            if (camera == null) return null;

            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorLayers))
            {
                if (hit.collider.CompareTag("Ground") || hit.collider.gameObject.name.Contains("Plane"))
                {
                    pointMousePosition = hit.point;
                }
            }
            return null; // Луч не попал в Plane
        }

        public void ShakeDiceIsMouseDown()
        {
            GetPointOnPlaneFromMouse();

            // Если это первый вызов, сохраняем начальную позицию
            if (isFirstShakeCall)
            {
                previousMousePosition = pointMousePosition;
                totalDistanceMoved = 0f;
                isFirstShakeCall = false;
            }

            // Вычисляем дистанцию перемещения мыши с предыдущего кадра
            float frameDistance = 0f;
            if (pointMousePosition != Vector3.zero)
            {
                frameDistance = Vector3.Distance(previousMousePosition, pointMousePosition);
                // totalDistanceMoved += frameDistance;

                // Сохраняем текущую позицию для следующего кадра
                previousMousePosition = pointMousePosition;
            }
            //--------------------Разобраться!!!!!!!!!!!!!!!!!!!!

            // Debug.Log($"Тряска  {power}");
            Bus<OnShakeEvent>.Raise(new OnShakeEvent(frameDistance));
            // Для отладки можно вывести информацию
            // Debug.Log($"Дистанция перемещения мыши: {frameDistance:F2}");
        }

        public void MoveToMouseDiceAndKeg()
        {
            GetPointOnPlaneFromMouse();
            Bus<OnMoveToMouseEvent>.Raise(new OnMoveToMouseEvent(pointMousePosition));
        }


        //---------------------------------- Вспомогательные методы  ----------------------------------
        private void ValidateScripts()
        {
            if (camera == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на camera!");

            if (cameraTarget == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на cameraTarget!");

            if (cinemachineCamera == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на cinemachineCamera!");
        }
    }
}