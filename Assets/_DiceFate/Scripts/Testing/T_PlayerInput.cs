using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using DiceFate.Units;

public class T_PlayerInput : MonoBehaviour
{
    [SerializeField] private new Camera camera; // Камера для преобразования координат мыши в лучи
    [SerializeField] private CinemachineCamera CinemachineCamera; // Виртуальная камера Cinemachine
    [SerializeField] private LayerMask selectableLayers; // Слои для выбираемых объектов
    [SerializeField] private LayerMask floorLayers; // Слои для пола/поверхности перемещения

    private ISelectable selectableUnit; // Текущий выбранный юнит
    private IHover hoverable;       // Текущее наведение

    // Управление частотой обновления
    [SerializeField] private float updateInterval = 0.05f;
    private float accumulatedTime;




    // Update вызывается каждый кадр
    void Update()
    {
        HandelLeftClick();      // Обработка левого клика (выбор)
        HandelRightClick();     // Обработка правого клика (перемещение)
        HandelRightClick2();    // Дополнительная обработка правого клика (отладка)
        UpdateForHoverable(); // Уменьшение частоты Обновление FPS - для Outline
    }





    // Обрабатывает правый клик для перемещения выбранного юнита
    private void HandelRightClick()
    {
        // Если нет выбранного юнита или юнит не может перемещаться - выходим
        if (selectableUnit == null || selectableUnit is not IMoveable moveable) { return; }

        // Создаем луч из камеры в позицию курсора мыши
        Ray cameraRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        // Если была отпущена правая кнопка мыши и луч попал в пол
        if (Mouse.current.rightButton.wasReleasedThisFrame
            && Physics.Raycast(cameraRay, out RaycastHit hit, float.MaxValue, floorLayers))
        {
            // Перемещаем юнита в точку попадания
            moveable.MoveTo(hit.point);
        }
    }


    // Дополнительная обработка правого клика для отладки
    private void HandelRightClick2()
    {
        // Просто логируем позицию правого клика для отладки
        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            Debug.Log("Right Clicked at position: " + Mouse.current.position.ReadValue());
        }
    }

    // Обрабатывает левый клик для выбора юнитов
    private void HandelLeftClick()
    {
        // Если камера не назначена - выходим
        if (camera == null) { return; }

        // Создаем луч из камеры в позицию курсора мыши
        Ray cameraRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        // Если была отпущена левая кнопка мыши
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            // Если до этого был выделен какой-то юнит, с него снимается выделение
            if (selectableUnit != null)
            {
                selectableUnit.Deselect();
                selectableUnit = null;
            }

            // Если луч попал в выбираемый объект и у него есть компонент ISelectable
            if (Physics.Raycast(cameraRay, out RaycastHit hit, float.MaxValue, selectableLayers)
            && hit.collider.TryGetComponent(out ISelectable selectable))
            {
                // Выбираем новый юнит
                selectable.Select();
                selectableUnit = selectable;
            }
        }
    }

    //-------------- Обработка наведения мыши --------------

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

    private void IsMouseOnObjectOrNot()
    {
        hoverable.OnExitHover(); // Вызываем метод OnExitHover - чтобы снять обводку 

        Ray cameraRay = camera.ScreenPointToRay(Input.mousePosition);

        // Если луч попал в объект и у него есть компонент IHover
        if (Physics.Raycast(cameraRay, out RaycastHit hit, 100) && hit.collider.TryGetComponent(out IHover hover))
        {
            hover.OnEnterHover();  // Вызываем метод через интерфейс IHover 
        }      
    }


    






}