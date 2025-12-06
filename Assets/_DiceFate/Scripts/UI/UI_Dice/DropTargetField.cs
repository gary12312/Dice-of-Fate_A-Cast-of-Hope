using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

public class DropTargetField : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform[] dropPoints; // Массив точек, куда можно поместить кубики
    public GameObject fieldOutline; // Ссылка на объект с контуром поля

    private bool isPointerOver = false; // Флаг, указывающий что курсор над полем
    private List<DragAndDropDice> diceOnField = new List<DragAndDropDice>(); // Список кубиков на поле
    private Dictionary<RectTransform, DragAndDropDice> occupiedPoints = new Dictionary<RectTransform, DragAndDropDice>(); // Словарь занятых точек

    void Start()
    {
        // Отключаем Image компонент самого поля (прозрачность)
        UnityEngine.UI.Image fieldImage = GetComponent<UnityEngine.UI.Image>();
        if (fieldImage != null)
        {
            fieldImage.enabled = false;
        }

        // Изначально скрываем контур поля
        if (fieldOutline != null)
        {
            fieldOutline.SetActive(false);
        }

        // Проверяем что точки установлены
        if (dropPoints == null || dropPoints.Length == 0)
        {
            Debug.LogError("Не установлены точки для размещения кубиков на поле!");
        }

        // Инициализируем словарь занятых точек (все точки свободны)
        if (dropPoints != null)
        {
            foreach (var point in dropPoints)
            {
                if (point != null)
                {
                    occupiedPoints[point] = null;
                }
            }
        }
    }

    // Вызывается когда объект отпускают над этим полем
    public void OnDrop(PointerEventData eventData)
    {
        // Получаем перетаскиваемый кубик
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject != null)
        {
            DragAndDropDice dice = droppedObject.GetComponent<DragAndDropDice>();
            if (dice != null)
            {
                // Если кубик уже на поле - ничего не делаем (уже размещен)
                if (dice.IsOnField())
                {
                    Debug.Log($"Кубик {dice.GetDiceName()} уже на поле");
                    return;
                }

                // Размещаем кубик на случайной свободной точке
                PlaceDiceAtRandomPoint(dice);
            }
        }
    }

    // Вызывается когда курсор входит в область поля
    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;

        // Включаем контур поля при наведении
        if (fieldOutline != null)
        {
            fieldOutline.SetActive(true);
        }
    }

    // Вызывается когда курсор покидает область поля
    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;

        // Выключаем контур поля
        if (fieldOutline != null)
        {
            fieldOutline.SetActive(false);
        }
    }

    // Метод для проверки, находится ли курсор над полем
    public bool IsPointerOverField()
    {
        return isPointerOver;
    }

    // Публичный метод для размещения кубика на случайной свободной точке
    public void PlaceDiceAtRandomPoint(DragAndDropDice dice)
    {
        if (dice == null)
        {
            Debug.LogError("Попытка разместить null кубик!");
            return;
        }

        // Проверяем, не находится ли кубик уже на поле
        if (dice.IsOnField())
        {
            Debug.LogWarning($"Кубик {dice.GetDiceName()} уже на поле!");
            return;
        }

        // Находим все свободные точки
        List<RectTransform> freePoints = GetFreePoints();
        Debug.Log($"Свободных точек: {freePoints.Count} из {dropPoints.Length}");

        if (freePoints.Count > 0)
        {
            // Выбираем случайную свободную точку
            int randomIndex = Random.Range(0, freePoints.Count);
            RectTransform selectedPoint = freePoints[randomIndex];

            // Размещаем кубик на выбранной точке
            PlaceDiceAtSpecificPoint(dice, selectedPoint);
        }
        else
        {
            Debug.LogWarning("Нет свободных точек для размещения кубика!");
            // Возвращаем кубик на стартовую позицию
            dice.ReturnToStart();
        }
    }

    // Метод для размещения кубика на конкретной точке
    private void PlaceDiceAtSpecificPoint(DragAndDropDice dice, RectTransform point)
    {
        if (point != null && dice != null)
        {
            // Устанавливаем позицию кубика
            dice.PlaceAtPoint(point.anchoredPosition);

            // Помечаем точку как занятую
            if (occupiedPoints.ContainsKey(point))
            {
                occupiedPoints[point] = dice;
            }
            else
            {
                Debug.LogError($"Точка не найдена в словаре occupiedPoints!");
                return;
            }

            // Добавляем кубик в список кубиков на поле (если его там еще нет)
            if (!diceOnField.Contains(dice))
            {
                diceOnField.Add(dice);

                // Находим номер точки для отладочного сообщения
                int pointNumber = System.Array.IndexOf(dropPoints, point) + 1;
                Debug.Log($"Кубик {dice.GetDiceName()} размещен на точке {pointNumber}");
            }
        }
    }

    // Метод для получения списка свободных точек
    public List<RectTransform> GetFreePoints()
    {
        List<RectTransform> freePoints = new List<RectTransform>();

        if (dropPoints == null)
        {
            Debug.LogError("dropPoints не инициализирован!");
            return freePoints;
        }

        foreach (var point in dropPoints)
        {
            if (point == null) continue;

            // Если точка есть в словаре и она свободна (null)
            if (occupiedPoints.ContainsKey(point) && occupiedPoints[point] == null)
            {
                freePoints.Add(point);
            }
        }

        return freePoints;
    }

    // Метод для получения списка занятых точек
    public List<RectTransform> GetOccupiedPoints()
    {
        List<RectTransform> occupied = new List<RectTransform>();

        foreach (var kvp in occupiedPoints)
        {
            if (kvp.Value != null)
            {
                occupied.Add(kvp.Key);
            }
        }

        return occupied;
    }

    // Метод для освобождения точки, когда кубик убран
    public void FreePoint(DragAndDropDice dice)
    {
        if (dice == null) return;

        // Находим точку, занятую этим кубиком
        RectTransform pointToFree = null;
        foreach (var kvp in occupiedPoints)
        {
            if (kvp.Value == dice)
            {
                pointToFree = kvp.Key;
                break;
            }
        }

        // Освобождаем точку
        if (pointToFree != null)
        {
            occupiedPoints[pointToFree] = null;
            Debug.Log($"Точка освобождена для кубика {dice.GetDiceName()}");
        }
        else
        {
            Debug.LogWarning($"Не найдена точка для кубика {dice.GetDiceName()}");
        }

        // Удаляем кубик из списка
        if (diceOnField.Contains(dice))
        {
            diceOnField.Remove(dice);
        }
    }

    // Метод для получения позиции точки по номеру
    public Vector2 GetPointPosition(int pointNumber)
    {
        if (pointNumber > 0 && pointNumber <= dropPoints.Length)
        {
            return dropPoints[pointNumber - 1].anchoredPosition;
        }
        return Vector2.zero;
    }

    // Публичный метод для получения списка кубиков на поле
    public List<DragAndDropDice> GetDiceOnField()
    {
        // Очищаем список от null-ссылок
        diceOnField.RemoveAll(dice => dice == null);

        // Также очищаем словарь занятых точек от null кубиков
        CleanOccupiedPoints();

        return diceOnField;
    }

    // Метод для очистки словаря занятых точек
    private void CleanOccupiedPoints()
    {
        List<RectTransform> pointsToClear = new List<RectTransform>();

        foreach (var kvp in occupiedPoints)
        {
            if (kvp.Value == null)
            {
                // Точка уже свободна, ничего не делаем
                continue;
            }

            // Если кубик был уничтожен или больше не на поле
            if (kvp.Value == null || !kvp.Value.IsOnField())
            {
                pointsToClear.Add(kvp.Key);
            }
        }

        // Освобождаем точки
        foreach (var point in pointsToClear)
        {
            occupiedPoints[point] = null;
        }
    }

    // Публичный метод для получения имен кубиков на поле
    public List<string> GetDiceNamesOnField()
    {
        // Очищаем списки
        GetDiceOnField();

        // Создаем список для имен
        List<string> diceNames = new List<string>();

        foreach (var dice in diceOnField)
        {
            if (dice != null)
            {
                diceNames.Add(dice.GetDiceName());
            }
        }

        return diceNames;
    }

    // Получение типов кубиков на поле
    public List<DragAndDropDice.DiceType> GetDiceTypesOnField()
    {
        // Очищаем списки
        GetDiceOnField();

        List<DragAndDropDice.DiceType> diceTypes = new List<DragAndDropDice.DiceType>();

        foreach (var dice in diceOnField)
        {
            if (dice != null)
            {
                diceTypes.Add(dice.GetDiceType());
            }
        }

        return diceTypes;
    }

    // Метод для удаления кубика из списка (если он возвращен на стартовую позицию)
    public void RemoveDiceFromField(DragAndDropDice dice)
    {
        if (dice == null) return;

        // Освобождаем точку, которую занимал кубик
        FreePoint(dice);

        Debug.Log($"Кубик {dice.GetDiceName()} удален из поля");
    }

    // Метод для проверки наличия конкретного типа кубика на поле
    public bool HasDiceType(DragAndDropDice.DiceType diceType)
    {
        // Очищаем списки
        GetDiceOnField();

        foreach (var dice in diceOnField)
        {
            if (dice != null && dice.GetDiceType() == diceType)
            {
                return true;
            }
        }
        return false;
    }

    // Метод для получения количества кубиков определенного типа на поле
    public int GetDiceTypeCount(DragAndDropDice.DiceType diceType)
    {
        // Очищаем списки
        GetDiceOnField();

        int count = 0;
        foreach (var dice in diceOnField)
        {
            if (dice != null && dice.GetDiceType() == diceType)
            {
                count++;
            }
        }
        return count;
    }

    // Проверить, есть ли свободные точки
    public bool HasFreePoints()
    {
        return GetFreePoints().Count > 0;
    }

    // Получить количество свободных точек
    public int GetFreePointsCount()
    {
        return GetFreePoints().Count;
    }

    // Получить количество занятых точек
    public int GetOccupiedPointsCount()
    {
        return GetOccupiedPoints().Count;
    }
}