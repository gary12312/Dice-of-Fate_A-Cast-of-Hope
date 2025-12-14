using UnityEngine;

public class T_MoveCube : MonoBehaviour
{
    [Header("Настройки движения")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float stopDistance = 0.1f;
    [SerializeField] private float pushForce = 5f; // Сила толкания от цилиндра

    [Header("Визуализация")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color insideCylinderColor = Color.yellow;

    // Компоненты
    private Renderer cubeRenderer;
    private Material cubeMaterial;

    // Состояния
    private bool shouldMove = false;
    private bool isInsideCylinder = false;
    private CylinderCollider currentCylinder;
    private Vector3 targetPosition;

    void Start()
    {
        targetPosition = transform.position;

        // Настройка материалов для смены цвета
        cubeRenderer = GetComponent<Renderer>();
        if (cubeRenderer != null)
        {
            cubeMaterial = cubeRenderer.material;
            cubeMaterial.color = normalColor;
        }
    }

    void Update()
    {
        // Если куб должен двигаться к целевой точке (старая логика)
        if (shouldMove && !isInsideCylinder)
        {
            MoveTowardsTarget();
        }
    }

    private void MoveTowardsTarget()
    {
        Vector3 direction = targetPosition - transform.position;

        if (direction.magnitude > stopDistance)
        {
            Vector3 moveStep = direction.normalized * moveSpeed * Time.deltaTime;

            if (moveStep.magnitude > direction.magnitude)
            {
                transform.position = targetPosition;
                shouldMove = false;
            }
            else
            {
                transform.position += moveStep;
            }
        }
        else
        {
            transform.position = targetPosition;
            shouldMove = false;
        }
    }

    // Метод для перемещения куба цилиндром
    public void MoveWithCylinder(Vector3 cylinderTargetPosition)
    {
        // Вычисляем направление движения
        Vector3 direction = cylinderTargetPosition - transform.position;

        // Двигаем куб к границе цилиндра
        if (direction.magnitude > stopDistance)
        {
            Vector3 moveStep = direction.normalized * pushForce * Time.deltaTime;

            // Ограничиваем максимальный шаг
            if (moveStep.magnitude > direction.magnitude)
            {
                transform.position = cylinderTargetPosition;
            }
            else
            {
                transform.position += moveStep;
            }
        }
    }

    // Установка состояния "внутри цилиндра"
    public void SetInsideCylinder(bool inside, CylinderCollider cylinder)
    {
        isInsideCylinder = inside;
        currentCylinder = cylinder;

        // Меняем цвет куба для визуальной обратной связи
        if (cubeMaterial != null)
        {
            cubeMaterial.color = inside ? insideCylinderColor : normalColor;
        }

        // Логирование для отладки
        Debug.Log(gameObject.name + (inside ? " вошел в цилиндр" : " вышел из цилиндра"));
    }

    // Получение текущего состояния
    public bool IsInsideCylinder()
    {
        return isInsideCylinder;
    }

    // Старый метод для движения к точке (если нужно сохранить старую функциональность)
    public void MoveToPoint(Vector3 point)
    {
        targetPosition = point;
        targetPosition.y = transform.position.y;
        shouldMove = true;
        isInsideCylinder = false; // Выходим из режима цилиндра
    }

    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying && shouldMove && !isInsideCylinder)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetPosition, 0.2f);
            Gizmos.DrawLine(transform.position, targetPosition);
        }
    }
}