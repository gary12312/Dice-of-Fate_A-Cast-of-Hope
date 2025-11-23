using UnityEngine;
using System.Collections.Generic;

public class DF_MiniFigureGridDecalFor : MonoBehaviour
{
    [Header("Настройки круговой сетки")]
    public float gridStep = 2f; // Шаг между круговыми линиями
    public int circleCount = 4; // Количество круговых линий
    public float lineLength = 15f; // Длина прямых линий
    public int lineCount = 8; // Количество прямых линий

    [Header("Настройки отображения")]
    public Color gridColor = Color.blue; // Цвет сетки
    public float previewHeight = 0.1f; // Высота отображения префаба над поверхностью

    [Header("Ссылки")]
    public GameObject previewPrefab; // Префаб для предпросмотра (куб2)
    public GameObject decalPoint; // Префаб декали точки для узлов сетки

    private GameObject currentPreview; // Текущий объект предпросмотра
    private bool isActive = false; // Активен ли режим предпросмотра
    private Vector3 gridCenter; // Центр сетки (позиция выделенного куба)
    private List<GameObject> decalPoints = new List<GameObject>(); // Список декалей точек

    void Update()
    {
        // Обновляем позицию префаба предпросмотра, если режим активен
        if (isActive)
        {
            UpdatePreviewPosition();
        }
    }

    // Обновление позиции префаба предпросмотра
    void UpdatePreviewPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Бросаем луч на плоскость с тегом "Ground"
        if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Ground"))
        {
            // Получаем позицию на круговой сетке
            Vector3 gridPosition = GetCircularGridPosition(hit.point);

            // Показываем или обновляем префаб предпросмотра
            if (currentPreview == null && previewPrefab != null)
            {
                currentPreview = Instantiate(previewPrefab, gridPosition, Quaternion.identity);
                // Поднимаем немного над поверхностью для видимости
                currentPreview.transform.position += Vector3.up * previewHeight;
            }
            else if (currentPreview != null)
            {
                currentPreview.transform.position = gridPosition + Vector3.up * previewHeight;
            }
        }
        else
        {
            // Если луч не попадает на землю - скрываем префаб
            HidePreview();
        }
    }

    // Получение позиции на круговой сетке
    public Vector3 GetCircularGridPosition(Vector3 worldPosition)
    {
        // Вектор от центра сетки к целевой позиции
        Vector3 direction = worldPosition - gridCenter;
        direction.y = 0; // Игнорируем высоту

        // Вычисляем расстояние от центра
        float distance = direction.magnitude;

        // Округляем расстояние до ближайшего шага сетки
        float roundedDistance = Mathf.Round(distance / gridStep) * gridStep;

        // Ограничиваем максимальное расстояние
        float maxDistance = circleCount * gridStep;
        roundedDistance = Mathf.Clamp(roundedDistance, 0, maxDistance);

        // Вычисляем угол направления
        float angle = Mathf.Atan2(direction.z, direction.x);

        // Округляем угол до ближайшего шага углов
        float angleStep = 360f / lineCount;
        float roundedAngle = Mathf.Round(angle * Mathf.Rad2Deg / angleStep) * angleStep;

        // Преобразуем обратно в радианы
        roundedAngle *= Mathf.Deg2Rad;

        // Вычисляем финальную позицию
        Vector3 gridPosition = gridCenter + new Vector3(
            Mathf.Cos(roundedAngle) * roundedDistance,
            0,
            Mathf.Sin(roundedAngle) * roundedDistance
        );

        Debug.Log($"Сетка: входная позиция {worldPosition}, выходная позиция {gridPosition}, расстояние {distance} -> {roundedDistance}");

        return gridPosition;
    }

    // Включение режима предпросмотра с указанием центра
    public void EnablePreview(Vector3 center)
    {
        isActive = true;
        gridCenter = center;

        // Создаем декали точек в узлах сетки
        CreateDecalPoints();

        Debug.Log($"Режим предпросмотра круговой сетки включен. Центр: {gridCenter}");
    }

    // Выключение режима предпросмотра
    public void DisablePreview()
    {
        isActive = false;
        HidePreview();
        ClearDecalPoints();
        Debug.Log("Режим предпросмотра круговой сетки выключен");
    }

    // Создание декалей точек в узлах сетки
    void CreateDecalPoints()
    {
        if (decalPoint == null)
        {
            Debug.LogWarning("Decal Point префаб не назначен!");
            return;
        }

        ClearDecalPoints(); // Очищаем старые точки

        // Создаем точки на всех пересечениях круговых и прямых линий
        for (int circle = 1; circle <= circleCount; circle++)
        {
            float radius = circle * gridStep;

            for (int line = 0; line < lineCount; line++)
            {
                float angle = line * (360f / lineCount) * Mathf.Deg2Rad;

                // Вычисляем позицию точки
                Vector3 pointPosition = gridCenter + new Vector3(
                    Mathf.Cos(angle) * radius,
                    0,
                    Mathf.Sin(angle) * radius
                );

                // Создаем декаль точки
                GameObject decal = Instantiate(decalPoint, pointPosition, Quaternion.identity);
                decal.transform.Rotate(90f, 0f, 0f); // Поворачиваем для отображения на плоскости

                // Добавляем в список для управления
                decalPoints.Add(decal);
            }
        }

        Debug.Log($"Создано {decalPoints.Count} декалей точек");
    }

    // Очистка всех декалей точек
    void ClearDecalPoints()
    {
        foreach (GameObject decal in decalPoints)
        {
            if (decal != null)
                Destroy(decal);
        }
        decalPoints.Clear();
    }

    // Скрытие префаба предпросмотра
    public void HidePreview()
    {
        if (currentPreview != null)
        {
            Destroy(currentPreview);
            currentPreview = null;
        }
    }

    // Получение текущей позиции предпросмотра на сетке
    public Vector3 GetCurrentPreviewPosition()
    {
        if (currentPreview != null)
        {
            // Возвращаем позицию без смещения по высоте
            return currentPreview.transform.position - Vector3.up * previewHeight;
        }
        return Vector3.zero;
    }

    // Проверка, активен ли предпросмотр в данный момент
    public bool IsPreviewActive()
    {
        return isActive && currentPreview != null;
    }

    // Визуализация сетки в редакторе
    void OnDrawGizmos()
    {
        if (!Application.isPlaying || !isActive) return;

        Gizmos.color = gridColor;

        // Рисуем круговые линии
        for (int i = 1; i <= circleCount; i++)
        {
            float radius = i * gridStep;
            DrawCircleGizmo(gridCenter, radius, 32);
        }

        // Рисуем прямые линии из центра
        float angleStep = 360f / lineCount;
        for (int i = 0; i < lineCount; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 lineEnd = gridCenter + new Vector3(
                Mathf.Cos(angle) * lineLength,
                0,
                Mathf.Sin(angle) * lineLength
            );
            Gizmos.DrawLine(gridCenter, lineEnd);
        }

        // Рисуем точки пересечения (для визуализации в редакторе)
        Gizmos.color = Color.red;
        for (int circle = 1; circle <= circleCount; circle++)
        {
            float radius = circle * gridStep;
            for (int line = 0; line < lineCount; line++)
            {
                float angle = line * angleStep * Mathf.Deg2Rad;
                Vector3 point = gridCenter + new Vector3(
                    Mathf.Cos(angle) * radius,
                    0,
                    Mathf.Sin(angle) * radius
                );
                Gizmos.DrawSphere(point, 0.1f);
            }
        }
    }

    // Вспомогательный метод для рисования круга
    void DrawCircleGizmo(Vector3 center, float radius, int segments)
    {
        float angle = 0f;
        Vector3 lastPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);

        for (int i = 1; i <= segments; i++)
        {
            angle = (float)i / (float)segments * 2f * Mathf.PI;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }
    }

    // Получение радиуса сетки (для ограничения перемещения)
    public float GetGridRadius()
    {
        return circleCount * gridStep;
    }

    // Получение центра сетки
    public Vector3 GetGridCenter()
    {
        return gridCenter;
    }

    // Получение списка всех позиций узлов сетки
    public List<Vector3> GetAllGridPositions()
    {
        List<Vector3> positions = new List<Vector3>();

        for (int circle = 1; circle <= circleCount; circle++)
        {
            float radius = circle * gridStep;

            for (int line = 0; line < lineCount; line++)
            {
                float angle = line * (360f / lineCount) * Mathf.Deg2Rad;

                Vector3 position = gridCenter + new Vector3(
                    Mathf.Cos(angle) * radius,
                    0,
                    Mathf.Sin(angle) * radius
                );

                positions.Add(position);
            }
        }

        return positions;
    }

    // Очистка при уничтожении объекта
    void OnDestroy()
    {
        HidePreview();
        ClearDecalPoints();
    }
}