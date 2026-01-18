using UnityEngine;

public class DF_MiniFigureIsSelectable : MonoBehaviour
{
    [Header("Ссылки")]
    public GameObject decal; // Декаль выделения
    public DF_MiniFigureGridDecalFor gridSystem; // Система сетки

    private bool isSelected = false; // Выбран ли куб

    // Событие для уведомления о изменении состояния выделения
    public System.Action<bool> OnSelectionChanged;

    void Start()
    {
        // Скрываем декаль при старте
        if (decal != null)
            decal.SetActive(false);

        // Ищем систему сетки если не назначена вручную
        if (gridSystem == null)
            gridSystem = FindObjectOfType<DF_MiniFigureGridDecalFor>();
    }

    void Update()
    {
        // Обработка левого клика мыши (0 = левая кнопка мыши)
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Если кликнули по самому кубу - выделяем его
                if (hit.collider.gameObject == gameObject)
                {
                    SelectFigure();
                }
                // Если кликнули по другому объекту и куб выделен - НЕ снимаем выделение сразу
                // (перемещение обработается в DF_MiniFigureController)
                // Выделение снимается только при явной отмене
            }
        }

        // Отмена выделения по правой кнопке мыши или Escape
        if (isSelected && (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape)))
        {
            DeselectFigure();
        }
    }

    // Метод выделения фигуры
    public void SelectFigure()
    {
        if (isSelected) return; // Уже выделен

        isSelected = true;

        // Показываем декаль выделения
        if (decal != null)
            decal.SetActive(true);

        // Включаем систему предпросмотра круговой сетки с центром в позиции фигуры
        if (gridSystem != null)
            gridSystem.EnablePreview(transform.position);

        // Уведомляем подписчиков о изменении состояния
        OnSelectionChanged?.Invoke(true);

        Debug.Log("Фигура выделена!");
    }

    // Метод отмены выделения
    public void DeselectFigure()
    {
        if (!isSelected) return; // Уже не выделен

        isSelected = false;

        // Скрываем декаль
        if (decal != null)
            decal.SetActive(false);

        // Выключаем систему предпросмотра сетки
        if (gridSystem != null)
            gridSystem.DisablePreview();

        // Уведомляем подписчиков о изменении состояния
        OnSelectionChanged?.Invoke(false);

        Debug.Log("Фигура снята с выделения");
    }

    // Проверка, выделена ли фигура
    public bool IsSelected()
    {
        return isSelected;
    }

    // Принудительная установка состояния выделения
    public void SetSelected(bool selected)
    {
        if (selected)
            SelectFigure();
        else
            DeselectFigure();
    }
}