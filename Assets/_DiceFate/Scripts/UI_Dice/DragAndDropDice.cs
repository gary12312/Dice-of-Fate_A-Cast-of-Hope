using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropDice : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private Vector3 startPosition; // Начальная позиция кубика (вне поля)
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    [Header("Настройки")]
    public int targetPointNumber = 1;
    public DropTargetField dropField;

    // Состояние кубика
    private bool isOnField = false; // Находится ли кубик на поле
    private bool wasOnFieldBeforeDrag = false; // Был ли кубик на поле перед началом перетаскивания

    void Start()
    {
        // Получаем компоненты при старте
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        // Если CanvasGroup нет - добавляем его
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Запоминаем начальную позицию
        startPosition = rectTransform.anchoredPosition;
    }

    // Вызывается когда начинаем перетаскивание
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Делаем кубик полупрозрачным при перетаскивании
        canvasGroup.alpha = 0.6f;

        // Отключаем блокировку лучей
        canvasGroup.blocksRaycasts = false;

        // Запоминаем, где был кубик перед началом перетаскивания
        wasOnFieldBeforeDrag = isOnField;

        Debug.Log($"Начали перетаскивание. Кубик был на поле: {wasOnFieldBeforeDrag}");
    }

    // Вызывается во время перетаскивания
    public void OnDrag(PointerEventData eventData)
    {
        // Перемещаем кубик вслед за курсором мыши
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    // Вызывается когда отпускаем кнопку мыши
    public void OnEndDrag(PointerEventData eventData)
    {
        // Возвращаем нормальную прозрачность
        canvasGroup.alpha = 1f;

        // Включаем обратно блокировку лучей
        canvasGroup.blocksRaycasts = true;

        // Проверяем, находится ли кубик над полем в момент отпускания
        bool isOverFieldNow = dropField != null && dropField.IsPointerOverField();

        Debug.Log($"Завершили перетаскивание. Над полем сейчас: {isOverFieldNow}, был на поле до перетаскивания: {wasOnFieldBeforeDrag}");

        if (isOverFieldNow)
        {
            // Если отпустили над полем - размещаем на поле
            PlaceOnField();
        }
        else
        {
            // Если отпустили НЕ над полем - ВСЕГДА возвращаем на стартовую позицию
            ReturnToStart();
        }
    }

    // Вызывается при клике на кубик
    public void OnPointerClick(PointerEventData eventData)
    {
        // Обрабатываем только клик левой кнопкой мыши
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (isOnField)
            {
                // Если кубик на поле - возвращаем на стартовую позицию
                ReturnToStart();
            }
            else
            {
                // Если кубик вне поля - перемещаем на поле
                PlaceOnField();
            }
        }
    }

    // Метод для размещения кубика на поле
    private void PlaceOnField()
    {
        if (dropField != null)
        {
            dropField.PlaceDiceAtPoint(this, targetPointNumber);
            isOnField = true;
            Debug.Log("Кубик размещен на поле");
        }
    }

    // Метод для размещения кубика в конкретной позиции
    public void PlaceAtPoint(Vector2 position)
    {
        rectTransform.anchoredPosition = position;
        isOnField = true;
    }

    // Метод для возврата кубика на начальную позицию (вне поля)
    public void ReturnToStart()
    {
        rectTransform.anchoredPosition = startPosition;
        isOnField = false;
        Debug.Log("Кубик возвращен на стартовую позицию");
    }

    // Метод для проверки, находится ли кубик на поле
    public bool IsOnField()
    {
        return isOnField;
    }
}