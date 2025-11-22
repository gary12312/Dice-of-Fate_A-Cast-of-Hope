using UnityEngine;

public class StoneCollisionPrevention : MonoBehaviour
{
    [Header("Настройки лучей")]
    public float rayDistance = 5f; // Длина лучей
    public float pushDistance = 0.3f; // Дистанция срабатывания
    public float pushForce = 15f; // Сила отталкивания

    [Header("Настройки слоев")]
    public string wallLayerName = "Wall"; // Название слоя стены

    private Rigidbody rb;
    private Vector3[] rayDirections; // Массив направлений лучей
    private int wallLayerMask; // Маска слоя стены

    void Start()
    {
        // Кэшируем ссылку на Rigidbody
        rb = GetComponent<Rigidbody>();

        // Создаем маску слоя для стены
        wallLayerMask = 1 << LayerMask.NameToLayer(wallLayerName);

        // Определяем 6 направлений для лучей (все стороны)
        rayDirections = new Vector3[]
        {
            Vector3.right,    // Вправо
            Vector3.left,     // Влево
            Vector3.up,       // Вверх
            Vector3.down,     // Вниз
            Vector3.forward,  // Вперед
            Vector3.back      // Назад
        };

        // Проверяем, что слой существует
        if (LayerMask.NameToLayer(wallLayerName) == -1)
        {
            Debug.LogError($"Слой '{wallLayerName}' не найден! Проверьте настройки слоев в Project Settings -> Tags and Layers");
        }
    }

    void FixedUpdate()
    {
        // Проверяем все направления на наличие препятствий
        CheckAllDirections();
    }

    void CheckAllDirections()
    {
        // Проходим по всем 6 направлениям
        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 direction = rayDirections[i];

            // Выпускаем луч из центра камня
            Ray ray = new Ray(transform.position, direction);
            RaycastHit hit;

            // Визуализация луча в редакторе
            Color rayColor = GetRayColor(direction);
            Debug.DrawRay(transform.position, direction * rayDistance, rayColor);

            // Проверяем столкновение луча только с объектами на слое Wall
            if (Physics.Raycast(ray, out hit, rayDistance, wallLayerMask))
            {
                // Проверяем, что это не сам камень и расстояние критическое
                if (hit.rigidbody != rb && hit.distance <= pushDistance)
                {
                    // Отталкиваем камень от стены
                    PushStoneAwayFrom(direction, hit.rigidbody);

                    // Дополнительная визуализация при срабатывании
                    Debug.DrawRay(transform.position, direction * hit.distance, Color.white, 0.1f);
                }
            }
        }
    }

    void PushStoneAwayFrom(Vector3 obstacleDirection, Rigidbody obstacleRb)
    {
        if (rb != null)
        {
            // Вычисляем направление отталкивания (противоположное направлению к стене)
            Vector3 pushDirection = -obstacleDirection;

            // Применяем силу к камню
            rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);

            // Можно раскомментировать для дополнительного эффекта:
            // Добавляем небольшое случайное смещение чтобы избежать залипания
            // rb.AddForce(Random.insideUnitSphere * 0.3f, ForceMode.Impulse);
        }
    }

    // Вспомогательная функция для цветов лучей (визуализация)
    Color GetRayColor(Vector3 direction)
    {
        if (direction == Vector3.right) return Color.red;      // Красный - право
        if (direction == Vector3.left) return Color.blue;      // Синий - лево
        if (direction == Vector3.up) return Color.green;       // Зеленый - верх
        if (direction == Vector3.down) return Color.yellow;    // Желтый - низ
        if (direction == Vector3.forward) return Color.cyan;   // Бирюзовый - вперед
        if (direction == Vector3.back) return Color.magenta;   // Пурпурный - назад

        return Color.white;
    }

    // Визуализация в инспекторе
    void OnDrawGizmosSelected()
    {
        if (rayDirections == null || rayDirections.Length == 0)
        {
            rayDirections = new Vector3[]
            {
                Vector3.right, Vector3.left, Vector3.up,
                Vector3.down, Vector3.forward, Vector3.back
            };
        }

        // Рисуем сферу зоны обнаружения
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, pushDistance);

        // Рисуем линии зоны обнаружения для каждого направления
        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 direction = rayDirections[i];
            Color rayColor = GetRayColor(direction);

            Gizmos.color = rayColor;
            Vector3 rayEnd = transform.position + direction * pushDistance;
            Gizmos.DrawLine(transform.position, rayEnd);

            // Рисуем небольшие сферы на концах лучей для лучшей видимости
            Gizmos.DrawSphere(rayEnd, 0.03f);
        }

        // Отображаем информацию о слое
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, $"Слой: {wallLayerName}", style);
#endif
    }
}