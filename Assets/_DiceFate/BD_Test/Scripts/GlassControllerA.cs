using UnityEngine;

public class GlassControllerA : MonoBehaviour
{
    [Header("Настройки скорости")]
    [SerializeField] private float horizontalSpeed = 5f; // Скорость движения влево-вправо
    [SerializeField] private float verticalSpeed = 5f;   // Скорость движения вперед-назад

    [Header("Ограничения движения")]
    [SerializeField] private float minX = -5f;   // Минимальная координата X
    [SerializeField] private float maxX = 5f;    // Максимальная координата X
    [SerializeField] private float minZ = -5f;   // Минимальная координата Z (вперед)
    [SerializeField] private float maxZ = 5f;    // Максимальная координата Z (вперед)

    private bool isDragging = false; // Флаг, указывающий, что мы тащим стакан
    private Vector3 initialMousePosition; // Начальная позиция мыши при нажатии
    private Vector3 initialGlassPosition; // Начальная позиция стакана при нажатии

    [Header("Настройки спавна")]
    [SerializeField] private Transform spawnPoint;    // Точка появления префаба (перетащите в инспекторе)
    [SerializeField] private GameObject prefabToSpawn; // Префаб для создания (перетащите в инспекторе)
    [SerializeField] private int prefabCount = 3;     // Количество префабов по умолчанию 3
    [SerializeField] private float prefabScale = 0.2f; // Масштаб префаба по умолчанию 0.2
    [SerializeField] private float prefabSpacing = 0.5f; // Расстояние между префабами

    private GameObject[] spawnedPrefabs; // Массив созданных объектов
    private bool arePrefabsSpawned = false; // Флаг, указывающий созданы ли префабы

    // Кэш компонента Transform для оптимизации
    private Transform myTransform;

    void Start()
    {
        // Кэшируем Transform при старте для большей производительности
        myTransform = transform;

        // Инициализируем массив для хранения префабов
        spawnedPrefabs = new GameObject[prefabCount];

        // Проверяем что spawnPoint назначен
        if (spawnPoint == null)
        {
            Debug.LogError("SpawnPoint не назначен! Перетащите объект SpawnPoint в инспекторе.");
        }

        // Проверяем что префаб назначен
        if (prefabToSpawn == null)
        {
            Debug.LogError("PrefabToSpawn не назначен! Перетащите префаб в инспекторе.");
        }
    }

    void Update()
    {
        // Обрабатываем управление стаканом
        HandleGlassControl();
    }

    /// <summary>
    /// Основной метод обработки управления стаканом
    /// </summary>
    private void HandleGlassControl()
    {
        // Проверяем нажатие ЛКМ
        if (Input.GetMouseButtonDown(0))
        {
            StartDragging();
            if (!arePrefabsSpawned)
            {
                SpawnPrefabs(); // Создаем префабы при нажатии
            }
        }

        // Проверяем отпускание ЛКМ
        if (Input.GetMouseButtonUp(0))
        {
            StopDragging();
            DestroyPrefabs(); // Удаляем префабы при отпускании
        }

        // Если стакан перетаскивается, обновляем его позицию
        if (isDragging)
        {
            UpdateGlassPosition();
        }
    }

    /// <summary>
    /// Начинаем перетаскивание стакана
    /// </summary>
    private void StartDragging()
    {
        isDragging = true;

        // Запоминаем начальные позиции
        initialMousePosition = Input.mousePosition;
        initialGlassPosition = myTransform.position;
    }

    /// <summary>
    /// Прекращаем перетаскивание стакана
    /// </summary>
    private void StopDragging()
    {
        isDragging = false;
    }

    /// <summary>
    /// Создает несколько префабов в точке спавна с учетом расстояния между ними
    /// </summary>
    private void SpawnPrefabs()
    {
        if (prefabToSpawn != null && spawnPoint != null)
        {
            // Вычисляем начальную позицию для первого префаба
            Vector3 startPosition = spawnPoint.position;

            // Определяем направление расстановки (например, по оси X)
            Vector3 spacingDirection = Vector3.right;

            // Вычисляем общую длину ряда префабов
            float totalLength = (prefabCount - 1) * prefabSpacing;

            // Начальная позиция с центрированием
            Vector3 currentPosition = startPosition - spacingDirection * (totalLength / 2);

            for (int i = 0; i < prefabCount; i++)
            {
                // Создаем префаб в текущей позиции
                spawnedPrefabs[i] = Instantiate(prefabToSpawn, currentPosition, spawnPoint.rotation);

                // Устанавливаем масштаб префаба
                spawnedPrefabs[i].transform.localScale = Vector3.one * prefabScale;

                // Перемещаем позицию для следующего префаба
                currentPosition += spacingDirection * prefabSpacing;
            }

            arePrefabsSpawned = true;
            Debug.Log($"Создано {prefabCount} префабов! Масштаб: {prefabScale}, Расстояние: {prefabSpacing}");
        }
    }

    /// <summary>
    /// Удаляет все созданные префабы
    /// </summary>
    private void DestroyPrefabs()
    {
        if (spawnedPrefabs != null && arePrefabsSpawned)
        {
            for (int i = 0; i < spawnedPrefabs.Length; i++)
            {
                if (spawnedPrefabs[i] != null)
                {
                    Destroy(spawnedPrefabs[i]);
                    spawnedPrefabs[i] = null;
                }
            }
            arePrefabsSpawned = false;
            Debug.Log($"Все {prefabCount} префабов удалены!");
        }
    }

    /// <summary>
    /// Обновляем позицию стакана на основе движения мыши
    /// </summary>
    private void UpdateGlassPosition()
    {
        // Получаем разницу в позиции мыши с момента начала перетаскивания
        Vector3 mouseDelta = Input.mousePosition - initialMousePosition;

        // Вычисляем новую позицию стакана
        Vector3 newPosition = initialGlassPosition;

        // Движение мыши ВЛЕВО-ВПРАВО = движение стакана ВЛЕВО-ВПРАВО (по оси X)
        newPosition.x = initialGlassPosition.x + (mouseDelta.x / Screen.width) * horizontalSpeed;

        // Движение мыши ВВЕРХ-ВНИЗ = движение стакана ВПЕРЕД-НАЗАД (по оси Z)
        // В Unity ось Z обычно отвечает за "глубину" (вперед-назад)
        newPosition.z = initialGlassPosition.z + (mouseDelta.y / Screen.height) * verticalSpeed;

        // Ограничиваем позицию в заданных пределах
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);

        // Применяем новую позицию к стакану
        myTransform.position = newPosition;
    }

    // Метод для визуализации зоны движения в редакторе Unity (только в сцене)
    private void OnDrawGizmosSelected()
    {
        // Рисуем wireframe куб, показывающий границы движения
        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3((minX + maxX) / 2, transform.position.y, (minZ + maxZ) / 2);
        Vector3 size = new Vector3(maxX - minX, 0.1f, maxZ - minZ);
        Gizmos.DrawWireCube(center, size);
    }

    /// <summary>
    /// Метод для обновления количества префабов во время выполнения (опционально)
    /// </summary>
    public void UpdatePrefabCount(int newCount)
    {
        if (arePrefabsSpawned)
        {
            DestroyPrefabs();
        }

        prefabCount = newCount;
        spawnedPrefabs = new GameObject[prefabCount];
    }

    /// <summary>
    /// Метод для обновления расстояния между префабами во время выполнения (опционально)
    /// </summary>
    public void UpdatePrefabSpacing(float newSpacing)
    {
        prefabSpacing = newSpacing;

        // Если префабы уже созданы, пересоздаем их с новым расстоянием
        if (arePrefabsSpawned)
        {
            DestroyPrefabs();
            SpawnPrefabs();
        }
    }
}