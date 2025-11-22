using UnityEngine;

public class WallMovementA : MonoBehaviour
{
    [Header("Настройки движения")]
    public float moveSpeed = 5f; // Скорость движения стены

    private Rigidbody rb;
    private Vector3 movement;
    private WallRayController rayController;

    void Start()
    {
        // Кэшируем ссылку на Rigidbody
        rb = GetComponent<Rigidbody>();

        // Получаем ссылку на контроллер лучей
        rayController = GetComponent<WallRayController>();
    }

    void Update()
    {
        // Сбрасываем движение каждый кадр
        movement = Vector3.zero;

        // Обрабатываем ввод
        if (Input.GetKey(KeyCode.D))
        {
            movement = Vector3.right * moveSpeed;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            movement = Vector3.left * moveSpeed;
        }
    }

    void FixedUpdate()
    {
        if (movement != Vector3.zero && rayController != null)
        {
            // Проверяем столкновение лучом ПЕРЕД движением
            if (!rayController.CheckStonesInFront(movement.normalized))
            {
                // Если камней нет на пути - двигаем стену
                Vector3 newPosition = rb.position + movement * Time.fixedDeltaTime;
                rb.MovePosition(newPosition);
            }
        }
    }
}