using UnityEngine;

public class WallRayController : MonoBehaviour
{
    [Header("Настройки лучей")]
    public float rayDistance = 5f; // Длина луча
    public float pushDistance = 0.2f; // Дистанция отталкивания
    public float pushForce = 10f; // Сила отталкивания

    [Header("Смещение и поворот лучей")]
    public float horizontalOffset = 0.5f; // Смещение лучей по горизонтали (по оси Z)
    public float verticalOffset = 0.5f; // Смещение лучей по вертикали (по оси Y)
    public int horizontalRays = 3; // Количество лучей по горизонтали (Z)
    public int verticalRays = 3; // Количество лучей по вертикали (Y)

    [Header("Направление лучей")]
    [Tooltip("Поворот плоскости лучей по оси Y (градусы)")]
    public float rotateFront = 0f; // Поворот плоскости лучей по оси Y
    public bool useLocalDirection = false; // Использовать локальное направление

    [Header("Настройки слоев")]
    public string stoneLayerName = "Stone"; // Название слоя камня

    private int stoneLayerMask; // Маска слоя камня
    private Vector3 rayDirection; // Кэшированное направление лучей

    void Start()
    {
        // Создаем маску слоя для камня (битовую маску)
        stoneLayerMask = 1 << LayerMask.NameToLayer(stoneLayerName);
        UpdateRayDirection();
    }

    void OnValidate()
    {
        // Обновляем направление при изменении в инспекторе
        UpdateRayDirection();
    }

    /// <summary>
    /// Обновляет направление лучей на основе настроек поворота
    /// </summary>
    private void UpdateRayDirection()
    {
        if (useLocalDirection)
        {
            // Используем локальное направление с поворотом
            Quaternion rotation = Quaternion.Euler(0f, rotateFront, 0f);
            rayDirection = rotation * Vector3.forward;
        }
        else
        {
            // Используем глобальное направление с поворотом
            Quaternion rotation = Quaternion.Euler(0f, rotateFront, 0f);
            rayDirection = rotation * Vector3.right;
        }
    }

    /// <summary>
    /// Получить текущее направление лучей
    /// </summary>
    public Vector3 GetRayDirection()
    {
        return rayDirection;
    }

    /// <summary>
    /// Получить направление лучей с учетом поворота объекта
    /// </summary>
    public Vector3 GetWorldRayDirection()
    {
        if (useLocalDirection)
        {
            // Локальное направление с учетом поворота объекта
            Quaternion localRotation = Quaternion.Euler(0f, rotateFront, 0f);
            return transform.rotation * localRotation * Vector3.forward;
        }
        else
        {
            // Глобальное направление (игнорирует поворот объекта)
            Quaternion rotation = Quaternion.Euler(0f, rotateFront, 0f);
            return rotation * Vector3.right;
        }
    }

    public bool CheckStonesInFront(Vector3 movementDirection)
    {
        bool stoneFound = false;
        Vector3 worldRayDirection = GetWorldRayDirection();

        // Используем движение или направление лучей в зависимости от настройки
        Vector3 checkDirection = useLocalDirection ? worldRayDirection : movementDirection;

        // Создаем сетку лучей
        for (int z = 0; z < horizontalRays; z++)
        {
            for (int y = 0; y < verticalRays; y++)
            {
                Vector3 rayOrigin = CalculateRayOrigin(z, y);

                if (CastRay(rayOrigin, checkDirection, out RaycastHit hit))
                {
                    PushStone(hit.rigidbody, checkDirection);
                    stoneFound = true;
                }
            }
        }

        return stoneFound;
    }

    /// <summary>
    /// Рассчитывает позицию начала луча
    /// </summary>
    private Vector3 CalculateRayOrigin(int zIndex, int yIndex)
    {
        float zOffset = CalculateOffset(zIndex, horizontalRays, horizontalOffset);
        float yOffset = CalculateOffset(yIndex, verticalRays, verticalOffset);

        Vector3 localOffset = Vector3.zero;

        if (useLocalDirection)
        {
            // При локальном направлении используем локальные оси
            localOffset = transform.right * zOffset + transform.up * yOffset;
        }
        else
        {
            // При глобальном направлении используем глобальные оси с учетом поворота лучей
            Quaternion rayRotation = Quaternion.Euler(0f, rotateFront, 0f);
            localOffset = rayRotation * (Vector3.forward * zOffset + Vector3.up * yOffset);
        }

        return transform.position + localOffset;
    }

    /// <summary>
    /// Рассчитывает смещение для луча
    /// </summary>
    private float CalculateOffset(int index, int totalRays, float maxOffset)
    {
        if (totalRays <= 1) return 0f;
        return Mathf.Lerp(-maxOffset, maxOffset, (float)index / (totalRays - 1));
    }

    /// <summary>
    /// Выпускает луч и проверяет столкновение
    /// </summary>
    private bool CastRay(Vector3 origin, Vector3 direction, out RaycastHit hit)
    {
        Ray ray = new Ray(origin, direction);

        // Визуализация луча в редакторе
        Debug.DrawRay(origin, direction * rayDistance, Color.red);

        if (Physics.Raycast(ray, out hit, rayDistance, stoneLayerMask))
        {
            return hit.distance <= pushDistance;
        }

        hit = new RaycastHit();
        return false;
    }

    void PushStone(Rigidbody stoneRb, Vector3 direction)
    {
        if (stoneRb != null)
        {
            // Применяем силу к камню в направлении от стены
            stoneRb.AddForce(direction * pushForce, ForceMode.Impulse);
        }
    }

    // Визуализация в инспекторе (только когда объект выбран)
    void OnDrawGizmosSelected()
    {
        UpdateRayDirection();
        Vector3 worldRayDirection = GetWorldRayDirection();

        // Рисуем основное направление лучей
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, worldRayDirection * rayDistance);

        // Рисуем зону обнаружения
        Gizmos.color = Color.yellow;
        DrawDetectionZone(worldRayDirection);

        // Рисуем точки, откуда выпускаются лучи
        Gizmos.color = Color.green;
        DrawRayPoints(worldRayDirection);
    }

    /// <summary>
    /// Рисует зону обнаружения
    /// </summary>
    private void DrawDetectionZone(Vector3 direction)
    {
        Vector3 zoneCenter = transform.position + direction * (rayDistance * 0.5f);
        Vector3 zoneSize = new Vector3(
            useLocalDirection ? horizontalOffset * 2 : rayDistance,
            verticalOffset * 2,
            useLocalDirection ? rayDistance : horizontalOffset * 2
        );

        // Поворачиваем Gizmos чтобы соответствовать направлению лучей
        Matrix4x4 originalMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(zoneCenter, Quaternion.LookRotation(direction), Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, zoneSize);
        Gizmos.matrix = originalMatrix;
    }

    /// <summary>
    /// Рисует точки начала лучей
    /// </summary>
    private void DrawRayPoints(Vector3 direction)
    {
        for (int z = 0; z < horizontalRays; z++)
        {
            for (int y = 0; y < verticalRays; y++)
            {
                Vector3 rayOrigin = CalculateRayOrigin(z, y);

                // Рисуем сферы в точках начала лучей
                Gizmos.DrawSphere(rayOrigin, 0.05f);

                // Рисуем линии лучей
                Gizmos.DrawLine(rayOrigin, rayOrigin + direction * pushDistance);
            }
        }
    }

    // Методы для настройки параметров лучей из других скриптов
    public void SetRayParameters(float distance, float pushDist, float force)
    {
        rayDistance = distance;
        pushDistance = pushDist;
        pushForce = force;
    }

    public void SetRayOffsets(float horizontal, float vertical)
    {
        horizontalOffset = horizontal;
        verticalOffset = vertical;
    }

    public void SetRayCount(int horizontal, int vertical)
    {
        horizontalRays = Mathf.Max(1, horizontal);
        verticalRays = Mathf.Max(1, vertical);
    }

    public void SetRayRotation(float rotation)
    {
        rotateFront = rotation;
        UpdateRayDirection();
    }

    public void SetUseLocalDirection(bool useLocal)
    {
        useLocalDirection = useLocal;
        UpdateRayDirection();
    }
}