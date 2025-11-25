using UnityEngine;

public class CreateDice : MonoBehaviour
{
    [Header("Настройки спавна")]
    [SerializeField] private Transform spawnPoint;    // Точка появления префаба (перетащите в инспекторе)
    [SerializeField] private GameObject prefabToSpawn; // Префаб для создания (перетащите в инспекторе)
    [SerializeField] private int prefabCount = 3;     // Количество префабов по умолчанию 3
    [SerializeField] private float prefabScale = 0.2f; // Масштаб префаба по умолчанию 0.2
    [SerializeField] private float prefabSpacing = 0.5f; // Расстояние между префабами

    private GameObject[] spawnedPrefabs; // Массив созданных объектов
    private bool arePrefabsSpawned = false; // Флаг, указывающий созданы ли префабы

    void Start()
    {        
        spawnedPrefabs = new GameObject[prefabCount];  // Инициализируем массив для хранения префабов

        if (spawnPoint == null) Debug.LogError("SpawnPoint не назначен! ");       // Проверяем что spawnPoint назначен
        if (prefabToSpawn == null) Debug.LogError("PrefabToSpawn не назначен!");  // Проверяем что префаб назначен
    }


    void Update()
    {
        CreateDiceOnMouseClick();
        DestroyPrefabsOnQPress();
    }
   
    private void CreateDiceOnMouseClick()
    {
        // Проверяем нажатие ЛКМ
        if (Input.GetMouseButtonDown(0))
        {    
            if (!arePrefabsSpawned) SpawnPrefabs(); // Создаем префабы при нажатии    
        }
    } 
    private void DestroyPrefabsOnQPress()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DestroyPrefabs();
        }
    }

    // Логика -----------------------------------------------------------------------------------------
    // Создает несколько префабов в точке спавна с учетом расстояния между ними
    private void SpawnPrefabs()
    {
        if (prefabToSpawn != null && spawnPoint != null)
        {            
            Vector3 startPosition = spawnPoint.position;           // Вычисляем начальную позицию для первого префаба          
            Vector3 spacingDirection = Vector3.right;              // Определяем направление расстановки (например, по оси X)            
            float totalLength = (prefabCount - 1) * prefabSpacing; // Вычисляем общую длину ряда префабов
           
            Vector3 currentPosition = startPosition - spacingDirection * (totalLength / 2);  // Начальная позиция с центрированием

            for (int i = 0; i < prefabCount; i++)
            {               
                spawnedPrefabs[i] = Instantiate(prefabToSpawn, currentPosition, spawnPoint.rotation);  // Создаем префаб в текущей позиции               
                spawnedPrefabs[i].transform.localScale = Vector3.one * prefabScale;                    // Устанавливаем масштаб префаба               
                currentPosition += spacingDirection * prefabSpacing;                                   // Перемещаем позицию для следующего префаба
            }

            arePrefabsSpawned = true;
            Debug.Log($"Создано {prefabCount} префабов! Масштаб: {prefabScale}, Расстояние: {prefabSpacing}");
        }
    }


    // Удаляет все созданные префабы
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



}
