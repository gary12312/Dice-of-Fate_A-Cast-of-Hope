using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using DiceFate.Units;
using DiceFate.Events;
using DiceFate.EventBus;
using UnityEngine.EventSystems;
using DiceFate.Maine;


namespace DiceFate.Player
{
    public class DF_PlayerInput : MonoBehaviour
    {
        [Header("Камера")]
        [SerializeField] private new Camera camera;
        [SerializeField] private Rigidbody cameraTarget;
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private LayerMask floorLayers;

        private CinemachineFollow cinemachineFollow;
        private Vector3 startingFollowOffset;
        private Vector2 mouseDelta;       // Изменение положения мыши
        private bool isWasMouseDownUI;   //  был ли клик по UI

        private ISelectable selectedUnit; // Текущий выделенный юнит который имеет интерфейс ISelectable
        private IHover hoverableUnit;     // Текущее наведение юнит
        private IMoveable moveable;       // Текущий перемещаемый юнит

        // Управление частотой обновления
        [SerializeField] private float updateInterval = 0.05f;
        private float accumulatedTime;


        // Дополнительные параметры для управления камерой
        [Header("Настройки движения камеры")]
        [SerializeField] private float cameraMoveSpeedX = 6f; // Скорость движения камеры
        [SerializeField] private float cameraMoveSpeedY = 3f; // Скорость движения камеры
        [SerializeField] private float cameraRotationSpeed = 2f; // Скорость вращения камеры
                                                                 //[SerializeField] private float cameraZoomSpeed = 10f; // Скорость зума камеры
                                                                 //[SerializeField] private float minZoomDistance = 2f; // Минимальное расстояние зума
                                                                 //[SerializeField] private float maxZoomDistance = 20f; // Максимальное расстояние зума

        //  private bool isCameraMoving = false; // Флаг движения камеры

        //-------------------------



        //private HashSet<AbstractUnit> aliveUnits = new(100); //параметр жизни
        //private HashSet<AbstractUnit> addedUnits = new(25);
        //private List<ISelectable> selectedUnits = new(12); // Текущий выделенный объект

        private Vector3 worldStartingMousePosition; // Начальная точка выделения в мировых координатах
        private bool isDragging; // Флаг, что сейчас идёт выделение

        private void Awake()
        {
            if (!cinemachineCamera.TryGetComponent(out cinemachineFollow))
            {
                Debug.LogError("У камеры Cinemachine не было функции Cinemachine Follow. Функция масштабирования не будет работать!");
            }

            startingFollowOffset = cinemachineFollow.FollowOffset;


            Bus<UnitSelectedEvent>.OnEvent += HandelUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent += HandeleUnitDeselect;
            //Bus<UnitSpawnEvent>.OnEvent += HandeleUnitSpawn;
            //Bus<ActionSelectedEvent>.OnEvent += HandleActionSelected;
        }


        private void OnDestroy()
        {
            Bus<UnitSelectedEvent>.OnEvent -= HandelUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent -= HandeleUnitDeselect;
            //Bus<UnitSpawnEvent>.OnEvent -= HandeleUnitSpawn;
            //Bus<ActionSelectedEvent>.OnEvent -= HandleActionSelected;
        }

        //private void HandelUnitSelected(UnitSelectiedEvent evt) => selectedUnits.Add(evt.Unit); //Обработка выбора объекта        
        //private void HandeleUnitDeselect(UnitDeSelectiedEvent evt) => selectedUnits.Remove(evt.Unit); //Обработка отмены выбора объекта
        //private void HandeleUnitSpawn(UnitSpawnEvent evt) => aliveUnits.Add(evt.Unit);
        //private void HandleActionSelected(ActionSelectedEvent evt) => activeAction = evt.Action;


        private void Update()
        {
            MiddleClickAndHold();  // Обработка центрального клика (поворот)
            RightClickAndHold();   // Обработка правого клика (перемещение)
            UpdateForHoverable();  // Уменьшение частоты Обновление FPS - для Outline

            if (selectedUnit == null)
            {
                LeftClickToSelected();
            }
            else
            {
                LeftClickToMove();
            }
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
            if (Mouse.current.rightButton.isPressed) { MoveCamera(); }
        }



        // Обработка левого клика Для ВЫБОРА юнита // TargetDecal
        private void LeftClickToSelected()
        {
            if (camera == null) { return; }

            Ray cameraRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());   // Получение точки на экране
            RaycastHit hit;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {

                //  if (selectedUnit != null) { selectedUnit.Deselect(); }

                if (Physics.Raycast(cameraRay, out hit, float.MaxValue, LayerMask.GetMask("Default"))
                && hit.collider.TryGetComponent(out ISelectable selectable))
                {
                    selectable.Select();
                }
            }

        }

        private void LeftClickToMove()
        {
            if (camera == null || selectedUnit == null)
            { return; }

            if (isWasMouseDownUI = EventSystem.current.IsPointerOverGameObject()) // проверяем находится ли курсор над UI элементом
            { return; }

            if (PhaseNumber.currentPhase != 4)
            { return; }

            Ray cameraRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // Тест: бросаем луч на все слои
                if (Physics.Raycast(cameraRay, out hit, float.MaxValue))
                {
                    // Прямой доступ через GameObject
                    MonoBehaviour selectedMono = selectedUnit as MonoBehaviour;
                    if (selectedMono != null && selectedMono.TryGetComponent(out IMoveable moveable))
                    {
                        moveable.MoveTo(hit.point);
                        selectedUnit.Deselect();
                    }

                }
            }
        }




        //---------------------------------- Функции Перемещение и вращение камеры  ----------------------------------

        // Перемещение камеры за счет перемещения цели камеры ( cameraTarget )
        private void MoveCamera()
        {
            mouseDelta = Mouse.current.delta.ReadValue();                    // Получаем изменение положения мыши
            float deltaX = mouseDelta.x * cameraMoveSpeedX * Time.deltaTime; // сколсть по X
            float deltaY = mouseDelta.y * cameraMoveSpeedY * Time.deltaTime; // сколсть по Y

            if (cameraTarget != null)
            {
                Vector3 moveDirection = new Vector3(-deltaX, 0, -deltaY);           // Вычисляем направление движения на основе положения мыши

                moveDirection = camera.transform.TransformDirection(moveDirection); // Преобразуем направление в мировые координаты
                moveDirection.y = 0;                                                // Игнорируем движение по Y

                cameraTarget.MovePosition(cameraTarget.position + moveDirection);   // Применяем движение к цели камеры
            }
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

        //---------------------------------- Выбор и выделение   ----------------------------------
        private void HandelUnitSelected(UnitSelectedEvent evt)
        {
            selectedUnit = evt.Unit; // Обновить текущий выделенный юнит
        }

        private void HandeleUnitDeselect(UnitDeselectedEvent evt)
        {
            selectedUnit = null;  // Сбросить текущий выделенный юнит          
        }


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

            // Если луч попал в объект и у него есть компонент IHover
            if (Physics.Raycast(cameraRay, out RaycastHit hit, 100) && hit.collider.TryGetComponent(out IHover hover))
            {

                if (hoverableUnit != null && hoverableUnit == hover)
                {
                    return; // Если уже есть наведение, ничего не делаем
                }

                hover.OnEnterHover();  // Включаем обводку через интерфейс IHover 
                hoverableUnit = hover; // Сохраняем текущий наведение юнит
            }
            // Если луч не попал в объект с IHover, снимаем обводку
            else if (hoverableUnit != null)
            {
                hoverableUnit.OnExitHover(); // Снимаем обводку через интерфейс IHover 
                hoverableUnit = null;        // Сбрасываем текущее наведение юнит
            }

        }




        // ---------------------------------------------------------------------------------------



    }
}