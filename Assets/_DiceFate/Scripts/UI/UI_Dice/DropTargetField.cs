using UnityEngine;
using UnityEngine.EventSystems;

public class DropTargetField : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform[] dropPoints; // Массив точек, куда можно поместить кубики
    public GameObject fieldOutline; // Ссылка на объект с контуром поля

    private bool isPointerOver = false; // Флаг, указывающий что курсор над полем

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
                // Размещаем кубик на соответствующей точке
                PlaceDiceAtPoint(dice, dice.targetPointNumber);
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

    // Метод для размещения кубика на определенной точке
    public void PlaceDiceAtPoint(DragAndDropDice dice, int pointNumber)
    {
        // Проверяем что номер точки корректен
        if (pointNumber > 0 && pointNumber <= dropPoints.Length)
        {
            RectTransform targetPoint = dropPoints[pointNumber - 1]; // -1 потому что массив с 0

            // Устанавливаем позицию кубика
            dice.PlaceAtPoint(targetPoint.anchoredPosition);

            Debug.Log($"Кубик размещен на точке {pointNumber}");
        }
        else
        {
            Debug.LogError($"Некорректный номер точки: {pointNumber}");
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
}