using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
//using System.Collections.Generic;
//using System.Windows.Input;
using DiceFate.Units;
using DiceFate.EventBus;
using DiceFate.Events;




namespace DiceFate.Player
{
    public class DF_PlayerInput : MonoBehaviour
    {
        [Header("Камера")]
        [SerializeField] private new Camera camera;
        [SerializeField] private Rigidbody cameraTarget;
        [SerializeField] private CinemachineCamera cinemachineCamera;
        //[SerializeField] private LayerMask selectibleUnitLayers;
        [SerializeField] private LayerMask floorLayers;
        //[SerializeField] private DF_CameraConfig cameraConfig;

        private CinemachineFollow cinemachineFollow;
        private Vector3 startingFollowOffset;
        private Vector2 mouseDelta; // Изменение положения мыши

        private ISelectable selectedUnit; // Текущий выделенный юнит который имеет интерфейс ISelectable

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
            LeftClick();
            MiddleClickAndHold();
            RightClickAndHold();

        }


        //---------------------------------- Клики мышы  -------------------------------------------------------------
        private void LeftClick()
        {
            if (camera == null) { return; }

            Ray cameraRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());   // Получение точки на экране

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {

                if (selectedUnit != null)
                {
                    selectedUnit.Deselect();
                    // selectedUnit = null; перенесено в HandelUnitDeselected
                }

                if (Physics.Raycast(cameraRay, out RaycastHit hit, float.MaxValue, LayerMask.GetMask("Default"))
                && hit.collider.TryGetComponent(out ISelectable selectable))
                {
                    selectable.Select();

                    //  selectedUnit = selectable;  перенесено в HandelUnitSelected и записано ка  selectedUnit = evt.Unit;
                }
            }
        }





        private void MiddleClickAndHold()
        {
            if (Mouse.current.middleButton.isPressed) { RotationCamera(); }
        }

        private void RightClickAndHold()
        {
            if (Mouse.current.rightButton.isPressed) { MoveCamera(); }
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









        // ---------------------------------------------------------------------------------------

















        //private void HendelMouseUp()
        //{
        //    if (activeAction == null && !Keyboard.current.shiftKey.isPressed) // Если шифт не нажат то сбрасывать выделение
        //    {
        //        DeselectAllUnits();
        //    }


        //    HandleLeftClick();
        //    foreach (AbstractUnit unit in addedUnits)
        //    {
        //        unit.Select();
        //    }


        //    selectionBox.gameObject.SetActive(false);
        //    isDragging = false;
        //}

        //private void HandelMouseDrag()
        //{
        //    if (activeAction != null || wasMouseDownOnUI) return;


        //    Debug.Log("Удержание");
        //    Bounds selectionBoxBounds = ResizeSelectoinBox();

        //    foreach (AbstractUnit unit in aliveUnits)
        //    {
        //        Vector2 unitPosition = camera.WorldToScreenPoint(unit.transform.position);
        //        if (selectionBoxBounds.Contains(unitPosition))
        //        {
        //            addedUnits.Add(unit);
        //        }
        //    }
        //}

        //private void HandelMouseDown()
        //{
        //    selectionBox.sizeDelta = Vector2.zero;
        //    selectionBox.gameObject.SetActive(true);
        //    addedUnits.Clear();
        //    isDragging = true;
        //    wasMouseDownOnUI = EventSystem.current.IsPointerOverGameObject();
        //}

        //private void DeselectAllUnits()
        //{
        //    ISelectable[] currentlySelectedUnits = selectedUnits.ToArray();

        //    foreach (ISelectable selectable in currentlySelectedUnits)
        //    {
        //        selectable.Deselect();

        //    }
        //}




    }
}