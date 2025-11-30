//using UnityEngine;

//public class MouseTarget : MonoBehaviour
//{
//    // Этот метод вызывается, когда курсор мыши попадает на коллайдер объекта
//    private void OnMouseEnter()
//    {
//        // Выводим в консоль сообщение "Наведение"
//        Debug.Log("Наведение");
//    }
//}

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MouseTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [field: SerializeField] public bool IsSelected { get; private set; }
    [field: SerializeField] public bool IsHovered { get; private set; }

    // СОВРЕМЕННЫЕ МЕТОДЫ ДЛЯ НОВОЙ INPUT SYSTEM:

    // 1. КОГДА КУРСОР НАВОДИТСЯ НА ОБЪЕКТ
    public void OnPointerEnter(PointerEventData eventData)
    {
        IsHovered = true;
        EnableHoverOutline();

        Debug.Log($"Курсор наведен на {gameObject.name}");
    }

    // 2. КОГДА КУРСОР УХОДИТ С ОБЪЕКТА
    public void OnPointerExit(PointerEventData eventData)
    {
        IsHovered = false;
        if (!IsSelected) DisableOutline();

        Debug.Log($"Курсор ушел с {gameObject.name}");
    }

    // 3. КОГДА КЛИКАЮТ ПО ОБЪЕКТУ
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // ЛКМ - выбор объекта
            IsSelected = !IsSelected; // Переключаем выбор

            if (IsSelected) EnableSelectOutline();
            else DisableOutline();

            Debug.Log($"Объект {gameObject.name} {(IsSelected ? "выбран" : "снят")}");
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // ПКМ - дополнительное действие
            Debug.Log($"Правый клик по {gameObject.name}");
        }
    }

    // ТВОИ МЕТОДЫ ОБВОДКИ
    public void EnableHoverOutline() { /* твой код */ }
    public void EnableSelectOutline() { /* твой код */ }
    public void DisableOutline() { /* твой код */ }
}