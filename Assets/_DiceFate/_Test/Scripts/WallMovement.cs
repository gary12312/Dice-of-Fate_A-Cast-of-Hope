using UnityEngine;

public class WallMovement : MonoBehaviour
{
    [Header("Настройки движения")]
    public float moveSpeed = 5f; // Скорость движения стены

    private Rigidbody rb;
    private Vector3 movement;

    void Start()
    {
        // Кэшируем ссылку на Rigidbody
        rb = GetComponent<Rigidbody>();
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
        if (movement != Vector3.zero)
        {
            // Просто двигаем стену - обнаружением столкновений теперь занимается камень
            Vector3 newPosition = rb.position + movement * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
        }
    }
}