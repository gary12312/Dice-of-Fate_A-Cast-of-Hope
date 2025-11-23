using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleMouseTarget : MonoBehaviour
{
    //void Update()
    //{
    //    // Создаем луч от камеры к курсору мыши
    //    Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
    //    RaycastHit hit;

    //    // Если луч попал в этот объект - выводим сообщение
    //    if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
    //    {
    //        Debug.Log("Наведение");
    //    }



    //}

    private void OnMouseEnter()
    {
        // Выводим в консоль сообщение "Наведение"
        Debug.Log("Наведение");
    }
}