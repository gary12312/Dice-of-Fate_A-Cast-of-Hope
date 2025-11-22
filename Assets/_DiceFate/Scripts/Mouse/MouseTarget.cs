using UnityEngine;

public class MouseTarget : MonoBehaviour
{
    // Этот метод вызывается, когда курсор мыши попадает на коллайдер объекта
    private void OnMouseEnter()
    {
        // Выводим в консоль сообщение "Наведение"
        Debug.Log("Наведение");
    }
}