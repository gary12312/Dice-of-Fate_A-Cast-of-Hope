using UnityEngine;

public class MouseWallMove : MonoBehaviour
{
    [Header("Ограничения движения")]
    [SerializeField] private float minX = -5f;   // Минимальная координата X
    [SerializeField] private float maxX = 5f;    // Максимальная координата X
    [SerializeField] private float minZ = -5f;   // Минимальная координата Z (вперед)
    [SerializeField] private float maxZ = 5f;    // Максимальная координата Z (вперед)

    [Header("Настройки чувствительности")]
    [SerializeField] private float sensitivity = 0.1f; // Чувствительность перемещения

    private bool isDragging = false; // Флаг, указывающий, что мы тащим стену
    private Vector3 initialMousePosition; // Начальная позиция мыши при нажатии
    private Vector3 initialPlatformPosition; // Начальная позиция стены при нажатии

    void Update()
    {
        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        // Начало перетаскивания при нажатии ЛКМ
        if (Input.GetMouseButtonDown(0))
        {
            StartDragging();
        }

        // Перетаскивание стены при зажатой ЛКМ
        if (isDragging && Input.GetMouseButton(0))
        {
            DragWall();
        }

        // Завершение перетаскивания при отпускании ЛКМ
        if (Input.GetMouseButtonUp(0))
        {
            StopDragging();
        }
    }

    private void StartDragging()
    {
        isDragging = true;
        initialMousePosition = Input.mousePosition;
        initialPlatformPosition = transform.position;
    }

    private void DragWall()
    {
        // Получаем разницу в позиции мыши
        Vector3 currentMousePosition = Input.mousePosition;
        Vector3 mouseDelta = currentMousePosition - initialMousePosition;

        // Вычисляем новую позицию стены
        Vector3 newPosition = initialPlatformPosition +
                             new Vector3(mouseDelta.x * sensitivity, 0f, mouseDelta.y * sensitivity);

        // Применяем ограничения движения
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);

        // Устанавливаем новую позицию
        transform.position = newPosition;
    }

    private void StopDragging()
    {
        isDragging = false;
    }

    // Визуализация ограничений в редакторе (опционально)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = new Vector3((minX + maxX) / 2f, transform.position.y, (minZ + maxZ) / 2f);
        Vector3 size = new Vector3(maxX - minX, 0.1f, maxZ - minZ);
        Gizmos.DrawWireCube(center, size);
    }
}