using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    [Header("Настройки движения")]
    public float moveSpeed = 5f; // Скорость движения платформы

    [Header("Способ управления")]
    public bool isMouse = false; // Если true - управление мышью, если false - клавиатурой

    [Header("Настройки мыши")]
    public float mouseSensitivity = 2f; // Чувствительность мыши

    [Header("Ограничения движения")]
    [SerializeField] private float minX = -5f;   // Минимальная координата X
    [SerializeField] private float maxX = 5f;    // Максимальная координата X
    [SerializeField] private float minZ = -5f;   // Минимальная координата Z (вперед)
    [SerializeField] private float maxZ = 5f;    // Максимальная координата Z (вперед)

    private bool isDragging = false; // Флаг, указывающий, что мы тащим платформу
    private Vector3 initialMousePosition; // Начальная позиция мыши при нажатии
    private Vector3 initialPlatformPosition; // Начальная позиция платформы при нажатии
    private Rigidbody rb; // Ссылка на компонент Rigidbody

    void Start()
    {
        // Получаем компонент Rigidbody у этого объекта
        rb = GetComponent<Rigidbody>();

        // Проверяем, есть ли Rigidbody на объекте
        if (rb == null)
        {
            Debug.LogError("Rigidbody не найден на объекте " + gameObject.name);
        }
    }

    void FixedUpdate()
    {
        // Вызываем функцию движения в FixedUpdate для работы с физикой
        if (isMouse)
        {
            MovePlatformWithMouse(); // Управление мышью
        }
        else
        {
            MovePlatformWithInput(); // Управление клавиатурой
        }
    }

    void MovePlatformWithInput()
    {
        // Получаем ввод от игрока по осям
        float horizontalInput = Input.GetAxis("Horizontal"); // A/D или стрелки влево/вправо
        float verticalInput = Input.GetAxis("Vertical");     // W/S или стрелки вверх/вниз

        // Создаем вектор направления движения
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);

        // Нормализуем вектор, чтобы диагональное движение не было быстрее
        movement = movement.normalized;

        // Умножаем на скорость и время между кадрами для плавного движения
        movement *= moveSpeed * Time.fixedDeltaTime;

        // Вычисляем новую позицию
        Vector3 newPosition = transform.position + movement;

        // Применяем ограничения к новой позиции
        newPosition = ApplyMovementConstraints(newPosition);

        // Перемещаем платформу используя Rigidbody
        rb.MovePosition(newPosition);
    }

    void MovePlatformWithMouse()
    {
        // Обработка начала перетаскивания
        if (Input.GetMouseButtonDown(0))
        {
            StartDragging();
        }

        // Обработка процесса перетаскивания
        if (isDragging && Input.GetMouseButton(0))
        {
            ContinueDragging();
        }

        // Обработка окончания перетаскивания
        if (Input.GetMouseButtonUp(0))
        {
            StopDragging();
        }
    }

    void StartDragging()
    {
        isDragging = true;
        initialMousePosition = Input.mousePosition;
        initialPlatformPosition = transform.position;

        Debug.Log("Начало перетаскивания платформы");
    }

    void ContinueDragging()
    {
        // Получаем текущую позицию мыши
        Vector3 currentMousePosition = Input.mousePosition;

        // Вычисляем разницу в движении мыши
        Vector3 mouseDelta = currentMousePosition - initialMousePosition;

        // Создаем вектор движения на основе движения мыши
        Vector3 movement = new Vector3(mouseDelta.x, 0f, mouseDelta.y);

        // Умножаем на чувствительность (делим на 100 для более удобных значений)
        movement *= mouseSensitivity / 100f;

        // Вычисляем новую позицию относительно начальной
        Vector3 newPosition = initialPlatformPosition + movement;

        // Применяем ограничения к новой позиции
        newPosition = ApplyMovementConstraints(newPosition);

        // Перемещаем платформу используя Rigidbody
        rb.MovePosition(newPosition);
    }

    void StopDragging()
    {
        isDragging = false;
        Debug.Log("Окончание перетаскивания платформы");
    }

    Vector3 ApplyMovementConstraints(Vector3 position)
    {
        // Ограничиваем позицию по осям X и Z
        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.z = Mathf.Clamp(position.z, minZ, maxZ);

        // Сохраняем исходную позицию Y (не изменяем высоту)
        position.y = transform.position.y;

        return position;
    }

    void Update()
    {
        // Визуальная обратная связь и отладка
        if (isMouse && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Управление мышью активировано");
        }
    }

    // Метод для отображения зоны ограничения в редакторе (только в Unity Editor)
    void OnDrawGizmosSelected()
    {
        // Рисуем wire cube для визуализации зоны движения
        Gizmos.color = Color.green;
        Vector3 center = new Vector3((minX + maxX) / 2, transform.position.y, (minZ + maxZ) / 2);
        Vector3 size = new Vector3(maxX - minX, 0.1f, maxZ - minZ);
        Gizmos.DrawWireCube(center, size);
    }
}