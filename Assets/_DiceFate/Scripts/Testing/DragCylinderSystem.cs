using UnityEngine;
using System.Collections.Generic;

public class SimpleDragSystem : MonoBehaviour
{
    [Header("Настройки")]
    public float radius = 3f;
    public float moveSpeed = 10f;

    [Header("Визуализация")]
    public Color circleColor = Color.green;

    // Переменные
    private Vector3 dragCenter;
    private bool isDragging = false;
    private List<Transform> cubes = new List<Transform>();
    private GameObject circleObject;

    void Start()
    {
        // Находим все кубы
        FindAllCubes();

        Debug.Log($"Найдено кубов: {cubes.Count}");
        Debug.Log("Система готова. Кликните ЛКМ на плоскость для начала перетаскивания.");
    }

    void Update()
    {
        // Нажатие ЛКМ
        if (Input.GetMouseButtonDown(0))
        {
            StartDrag();
        }

        // Движение мыши при нажатой ЛКМ
        if (isDragging)
        {
            UpdateDrag();
        }

        // Отпускание ЛКМ
        if (Input.GetMouseButtonUp(0))
        {
            StopDrag();
        }
    }

    void FindAllCubes()
    {
        cubes.Clear();

        // Простой поиск всех объектов с именем Cube
        foreach (Transform child in FindObjectsOfType<Transform>())
        {
            if (child.name.Contains("Cube") && child != this.transform)
            {
                cubes.Add(child);
            }
        }
    }

    void StartDrag()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            dragCenter = hit.point;
            isDragging = true;

            // Создаем визуализацию круга
            CreateCircle(dragCenter);

            // Сразу размещаем кубы вокруг центра
            PositionCubesAroundCenter();

            Debug.Log($"Начало перетаскивания в {dragCenter}");
        }
    }

    void CreateCircle(Vector3 position)
    {
        // Создаем плоскость для круга
        circleObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        circleObject.name = "DragCircle";
        circleObject.transform.position = position + Vector3.up * 0.01f;
        circleObject.transform.localScale = new Vector3(radius / 5f, 1, radius / 5f); // Plane масштабируется по 10м

        // Настраиваем материал
        Renderer renderer = circleObject.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = circleColor;
        renderer.material = mat;

        // Удаляем коллайдер
        Destroy(circleObject.GetComponent<Collider>());
    }

    void PositionCubesAroundCenter()
    {
        for (int i = 0; i < cubes.Count; i++)
        {
            if (cubes[i] == null) continue;

            float angle = i * (360f / cubes.Count) * Mathf.Deg2Rad;
            Vector3 targetPos = new Vector3(
                dragCenter.x + Mathf.Cos(angle) * radius * 0.8f,
                cubes[i].position.y,
                dragCenter.z + Mathf.Sin(angle) * radius * 0.8f
            );

            cubes[i].position = targetPos;
        }
    }

    void UpdateDrag()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Обновляем центр
            Vector3 newCenter = hit.point;

            // Двигаем круг
            if (circleObject != null)
            {
                circleObject.transform.position = newCenter + Vector3.up * 0.01f;
            }

            // Двигаем кубы
            Vector3 movement = newCenter - dragCenter;
            dragCenter = newCenter;

            foreach (Transform cube in cubes)
            {
                if (cube == null) continue;

                // Двигаем куб вместе с центром
                cube.position += movement;

                // Проверяем расстояние от центра
                Vector3 toCube = cube.position - dragCenter;
                toCube.y = 0;

                if (toCube.magnitude > radius)
                {
                    // Возвращаем куб внутрь круга
                    Vector3 targetPos = dragCenter + toCube.normalized * radius;
                    targetPos.y = cube.position.y;
                    cube.position = Vector3.MoveTowards(cube.position, targetPos, moveSpeed * Time.deltaTime);
                }
            }
        }
    }

    void StopDrag()
    {
        isDragging = false;

        // Удаляем круг
        if (circleObject != null)
        {
            Destroy(circleObject);
        }

        Debug.Log("Перетаскивание остановлено");
    }

    // Рисуем Gizmos для отладки
    void OnDrawGizmos()
    {
        if (isDragging)
        {
            // Рисуем круг
            Gizmos.color = circleColor;
            DrawGizmoCircle(dragCenter, radius, 20);

            // Рисуем радиус
            Gizmos.color = Color.white;
            Gizmos.DrawLine(dragCenter, dragCenter + Vector3.right * radius);

            // Подписываем радиус
#if UNITY_EDITOR
            UnityEditor.Handles.Label(dragCenter + Vector3.right * radius * 0.5f + Vector3.up * 0.5f, $"Radius: {radius:F1}");
#endif
        }
    }

    void DrawGizmoCircle(Vector3 center, float radius, int segments)
    {
        float angle = 0f;
        Vector3 lastPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);

        for (int i = 1; i <= segments; i++)
        {
            angle = (i / (float)segments) * Mathf.PI * 2;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }
    }
}