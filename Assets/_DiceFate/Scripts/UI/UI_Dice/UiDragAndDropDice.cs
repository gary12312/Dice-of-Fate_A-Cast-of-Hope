using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiDragAndDropDice : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    // Перечисление для типов кубиков
    public enum DiceType
    {
        Movement,
        Attack,
        Shield,
        Counterattack
    }

    private Vector3 startPosition; // Начальная позиция кубика (вне поля)
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    [Header("Настройки")]
    public UiDropTargetField uiDropField; // Ссылка на поле

    // Публичный параметр типа кубика с использованием enum
    [Header("Тип кубика")]
    [Tooltip("Тип кубика: Movement, Attack, Shield или Counterattack")]
    public DiceType diceType = DiceType.Movement; // Значение по умолчанию

    // Свойство для получения имени кубика как строки
    public string DiceName
    {
        get { return diceType.ToString(); }
    }

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

        // Проверяем ссылку на поле
        if (uiDropField == null)
        {
            Debug.LogError($"У кубика {DiceName} не установлена ссылка на DropTargetField!");
        }
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

        Debug.Log($"Начали перетаскивание кубика {DiceName}. Кубик был на поле: {wasOnFieldBeforeDrag}");
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
        bool isOverFieldNow = uiDropField != null && uiDropField.IsPointerOverField();

        Debug.Log($"Завершили перетаскивание кубика {DiceName}. Над полем сейчас: {isOverFieldNow}, был на поле до перетаскивания: {wasOnFieldBeforeDrag}");

        if (isOverFieldNow)
        {
            // Если отпустили над полем - размещаем на поле
            PlaceOnField();
        }
        else
        {
            // Если отпустили НЕ над полем - возвращаем в исходное состояние
            if (wasOnFieldBeforeDrag)
            {
                // Если кубик был на поле - возвращаем на ту же позицию на поле
                // (но для простоты пока возвращаем на стартовую позицию)
                ReturnToStart();
            }
            else
            {
                // Если кубик не был на поле - возвращаем на стартовую позицию
                ReturnToStart();
            }
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
        if (uiDropField != null)
        {
            // Проверяем, не находится ли кубик уже на поле
            if (isOnField)
            {
                Debug.LogWarning($"Кубик {DiceName} уже на поле!");
                return;
            }

            // Поле само выберет случайную свободную точку
            uiDropField.PlaceDiceAtRandomPoint(this);
            // isOnField устанавливается в true внутри PlaceAtPoint
        }
        else
        {
            Debug.LogError($"Не могу разместить кубик {DiceName} - не установлено поле!");
        }
    }

    // Метод для размещения кубика в конкретной позиции
    public void PlaceAtPoint(Vector2 position)
    {
        rectTransform.anchoredPosition = position;
        isOnField = true;
        Debug.Log($"Кубик {DiceName} размещен в позиции {position}");
    }

    // Метод для возврата кубика на начальную позицию (вне поля)
    public void ReturnToStart()
    {
        rectTransform.anchoredPosition = startPosition;
        isOnField = false;

        // Уведомляем поле, что кубик убран (освобождаем точку)
        if (uiDropField != null)
        {
            uiDropField.RemoveDiceFromField(this);
        }

        Debug.Log($"Кубик {DiceName} возвращен на стартовую позицию");
    }

    // Метод для проверки, находится ли кубик на поле
    public bool IsOnField()
    {
        return isOnField;
    }

    // Метод для получения имени кубика
    public string GetDiceName()
    {
        return DiceName;
    }

    // Метод для получения типа кубика
    public DiceType GetDiceType()
    {
        return diceType;
    }
}