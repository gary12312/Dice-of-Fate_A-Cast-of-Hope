using UnityEngine;

public class MiniMovementOnClick : MonoBehaviour
{
    [Header("Настройки перемещения")]
    [SerializeField] private float movementSpeed = 5f; // Скорость перемещения персонажа
    [SerializeField] private float rotationSpeed = 10f; // Скорость поворота к цели

    // Ссылка на компонент камеры, через которую будем делать рейкаст
    private Camera mainCamera;
    // Компонент персонажа для перемещения
    private CharacterController characterController;
    // Текущая целевая точка перемещения
    private Vector3 targetPosition;
    // Флаг, указывающий, что мы перемещаем персонажа
    private bool isMoving = false;

    void Start()
    {
        // Автоматически находим основную камеру на сцене
        mainCamera = Camera.main;

        // Пытаемся получить компонент CharacterController у этого объекта
        characterController = GetComponent<CharacterController>();

        // Изначально целевая позиция - текущая позиция персонажа
        targetPosition = transform.position;
    }

    void Update()
    {
        // Обрабатываем нажатие правой кнопки мыши
        HandleRightClick();

        // Перемещаем персонажа, если это необходимо
        MoveCharacter();
    }

    /// <summary>
    /// Обрабатывает нажатие правой кнопки мыши
    /// </summary>
    private void HandleRightClick()
    {
        // Если нажата правая кнопка мыши
        if (Input.GetMouseButtonDown(1)) // 1 - это правая кнопка мыши
        {
            // Создаем луч из камеры через позицию курсора мыши
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Если луч пересекает какой-либо коллайдер
            if (Physics.Raycast(ray, out hit))
            {
                // Проверяем, попал ли луч именно в этого персонажа
                if (hit.collider.gameObject == gameObject)
                {
                    // Начинаем перемещение
                    isMoving = true;
                    Debug.Log("Начало перемещения персонажа");
                }
            }
        }

        // Если отпустили правую кнопку мыши
        if (Input.GetMouseButtonUp(1))
        {
            // Останавливаем перемещение
            isMoving = false;
            Debug.Log("Остановка перемещения персонажа");
        }

        // Если удерживаем ПКМ и перемещение активно
        if (isMoving && Input.GetMouseButton(1))
        {
            // Получаем точку на плоскости под курсором мыши
            UpdateTargetPosition();
        }
    }

    /// <summary>
    /// Обновляет целевую позицию на основе позиции курсора мыши
    /// </summary>
    private void UpdateTargetPosition()
    {
        // Создаем луч из камеры через позицию курсора мыши
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // Плоскость на уровне Y=0
        float rayDistance;

        // Если луч пересекает плоскость земли
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            // Получаем точку пересечения луча с плоскостью
            Vector3 point = ray.GetPoint(rayDistance);

            // Устанавливаем целевую позицию (сохраняем текущую Y-координату персонажа)
            targetPosition = new Vector3(point.x, transform.position.y, point.z);
        }
    }

    /// <summary>
    /// Перемещает персонажа к целевой позиции
    /// </summary>
    private void MoveCharacter()
    {
        // Если персонаж уже находится в целевой позиции, ничего не делаем
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            return;

        // Вычисляем направление движения
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        // Плавно поворачиваем персонажа в направлении движения
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Перемещаем персонажа
        if (characterController != null && characterController.enabled)
        {
            // Используем CharacterController для плавного перемещения
            Vector3 movement = moveDirection * movementSpeed * Time.deltaTime;
            characterController.Move(movement);
        }
        else
        {
            // Если CharacterController отсутствует, используем простое перемещение
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Визуализация целевой позиции в редакторе (только для отладки)
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Рисуем сферу в целевой позиции (видно только в сцене в редакторе)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(targetPosition, 0.5f);

        // Рисуем линию от персонажа к целевой позиции
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, targetPosition);
    }
}