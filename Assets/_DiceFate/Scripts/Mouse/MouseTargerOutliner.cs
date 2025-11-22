using UnityEngine;

public class MouseTargerOutliner : MonoBehaviour
{
    private Outline outline;
    private Camera mainCamera;
    //private QueryTriggerInteraction,le

    void Start()
    {
        outline = GetComponent<Outline>();
        mainCamera = Camera.main; // Получаем главную камеру

        if (outline != null)
        {
            outline.enabled = false;
        }

        // Проверка компонентов
        Debug.Log("Collider: " + (GetComponent<Collider>() != null));
        Debug.Log("Outline: " + (outline != null));
        Debug.Log("Camera: " + (mainCamera != null));
    }

    void Update()
    {
        // Создаем луч из камеры в направлении курсора мыши
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            Debug.Log("Raycast: Mouse over ");
        }

        //    Проверяем пересечение луча с коллайдером
        //    if (Physics.Raycast(ray, out hit, 100))
        //    {
        //        var interacteble = hit.collider.GetComponent<Interactable>();
        //        Если луч попал в этот объект
        //    if (hit.collider.gameObject == gameObject)
        //        {
        //            Включаем обводку
        //        if (outline != null && !outline.enabled)
        //            {
        //                outline.enabled = true;
        //                Debug.Log("Raycast: Mouse over " + gameObject.name);
        //            }
        //        }
        //        else
        //        {
        //            Выключаем обводку если луч попал в другой объект
        //            if (outline != null && outline.enabled)
        //            {
        //                outline.enabled = false;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        Выключаем обводку если луч никуда не попал
        //        if (outline != null && outline.enabled)
        //        {
        //            outline.enabled = false;
        //        }
        //    }
        //}
    }
}