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

    // Update вызывается каждый кадр
    void Update()
    {
        HandelLeftClick();   // Обработка левого клика (выбор)
        HandelRightClick();  // Обработка правого клика (перемещение)
        HandelRightClick2(); // Дополнительная обработка правого клика (отладка)
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
}