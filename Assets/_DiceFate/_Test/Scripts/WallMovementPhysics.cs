using UnityEngine;

public class WallMovementPhysics : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;

    void Start()
    {
        //  эшируем ссылку на Rigidbody дл€ производительности
        rb = GetComponent<Rigidbody>();
    }

    // FixedUpdate вызываетс€ с фиксированным интервалом и используетс€ дл€ работы с физикой
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.D))
        {
            // –ассчитываем новую позицию на основе движени€
            Vector3 newPosition = rb.position + Vector3.right * moveSpeed * Time.fixedDeltaTime;
            // ѕеремещаем Rigidbody на новую позицию
            rb.MovePosition(newPosition);
        }
        // ќбрабатываем нажатие клавиши D
        if (Input.GetKey(KeyCode.A))
        {
            // –ассчитываем новую позицию на основе движени€
            Vector3 newPosition = rb.position + Vector3.left * moveSpeed * Time.fixedDeltaTime;
            // ѕеремещаем Rigidbody на новую позицию
            rb.MovePosition(newPosition);
        }
    }
}