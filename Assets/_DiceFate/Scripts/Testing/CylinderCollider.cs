using UnityEngine;
using System.Collections.Generic;

public class CylinderCollider : MonoBehaviour
{
    [SerializeField] private float radius = 2f;
    [SerializeField] private float height = 3f;

    // Список кубов внутри цилиндра
    private List<T_MoveCube> cubesInside = new List<T_MoveCube>();
    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;

        // Убедимся, что это триггер
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            // Добавляем MeshCollider и настраиваем как триггер
            MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
            meshCollider.convex = true;
            meshCollider.isTrigger = true;
        }
        else
        {
            collider.isTrigger = true;
        }
    }

    void Update()
    {
        // Перемещаем все кубы внутри цилиндра
        for (int i = cubesInside.Count - 1; i >= 0; i--)
        {
            if (cubesInside[i] != null)
            {
                MoveCubeWithCylinder(cubesInside[i]);
            }
            else
            {
                cubesInside.RemoveAt(i);
            }
        }

        lastPosition = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        // Если куб входит в цилиндр
        T_MoveCube cube = other.GetComponent<T_MoveCube>();
        if (cube != null && !cubesInside.Contains(cube))
        {
            cubesInside.Add(cube);
            cube.SetInsideCylinder(true, this);
            Debug.Log("Куб добавлен в цилиндр: " + other.name);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Если куб выходит из цилиндра
        T_MoveCube cube = other.GetComponent<T_MoveCube>();
        if (cube != null && cubesInside.Contains(cube))
        {
            cubesInside.Remove(cube);
            cube.SetInsideCylinder(false, null);
            Debug.Log("Куб удален из цилиндра: " + other.name);
        }
    }

    private void MoveCubeWithCylinder(T_MoveCube cube)
    {
        Vector3 cylinderCenter = transform.position;
        Vector3 cubePosition = cube.transform.position;

        // Вычисляем вектор от центра цилиндра к кубу (игнорируем высоту)
        Vector3 toCube = cubePosition - cylinderCenter;
        Vector3 toCubeFlat = new Vector3(toCube.x, 0, toCube.z);

        // Если куб находится за пределами радиуса
        if (toCubeFlat.magnitude > radius)
        {
            // Толкаем куб к границе цилиндра
            Vector3 targetPosition = cylinderCenter + toCubeFlat.normalized * radius;
            targetPosition.y = cubePosition.y; // Сохраняем высоту куба

            cube.MoveWithCylinder(targetPosition);
        }
        else
        {
            // Если куб внутри радиуса и цилиндр движется
            Vector3 cylinderMovement = (transform.position - lastPosition) / Time.deltaTime;
            if (cylinderMovement.magnitude > 0.01f)
            {
                // Слегка толкаем куб в направлении движения цилиндра
                Vector3 pushDirection = new Vector3(cylinderMovement.x, 0, cylinderMovement.z).normalized;
                cube.transform.position += pushDirection * 0.5f * Time.deltaTime;
            }
        }
    }

    // Методы для настройки параметров
    public void SetRadius(float newRadius)
    {
        radius = newRadius;
        UpdateScale();
    }

    public void SetHeight(float newHeight)
    {
        height = newHeight;
        UpdateScale();
    }

    private void UpdateScale()
    {
        transform.localScale = new Vector3(radius * 2, height / 2, radius * 2);
    }

    // Получение центра цилиндра
    public Vector3 GetCenter()
    {
        return transform.position;
    }

    // Получение параметров
    public float GetRadius()
    {
        return radius;
    }

    public float GetHeight()
    {
        return height;
    }

    // Визуализация в редакторе
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // Рисуем верхний и нижний круги
        Vector3 bottomCenter = transform.position - Vector3.up * height / 2;
        Vector3 topCenter = transform.position + Vector3.up * height / 2;

        Gizmos.DrawWireSphere(bottomCenter, radius);
        Gizmos.DrawWireSphere(topCenter, radius);

        // Рисуем вертикальные линии
        Gizmos.DrawLine(
            bottomCenter + Vector3.right * radius,
            topCenter + Vector3.right * radius
        );
        Gizmos.DrawLine(
            bottomCenter - Vector3.right * radius,
            topCenter - Vector3.right * radius
        );
        Gizmos.DrawLine(
            bottomCenter + Vector3.forward * radius,
            topCenter + Vector3.forward * radius
        );
        Gizmos.DrawLine(
            bottomCenter - Vector3.forward * radius,
            topCenter - Vector3.forward * radius
        );
    }
}