using DiceFate.Player;
using UnityEngine;


namespace DiceFate.Player
{
    public class CameraBoundaryController : MonoBehaviour
    {
        [SerializeField] private DF_PlayerInput playerInput;
        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void OnCollisionStay(Collision collision)
        {
            // Проверяем, что это граничный коллайдер
            if (collision.gameObject.CompareTag("CameraBoundary"))
            {
                // Отталкиваем от границы
                Vector3 pushDirection = (transform.position - collision.GetContact(0).point).normalized;
                pushDirection.y = 0;

                // Уменьшаем скорость при приближении к границе
                if (rb != null && playerInput != null)
                {
                    // Останавливаем движение в направлении границы
                    Vector3 currentVelocity = rb.linearVelocity;
                    Vector3 normal = collision.GetContact(0).normal;

                    // Проекция скорости на нормаль коллизии
                    float velocityTowardsBoundary = Vector3.Dot(currentVelocity, normal);

                    // Если двигаемся к границе, уменьшаем скорость
                    if (velocityTowardsBoundary < 0)
                    {
                        // Умножаем компоненту скорости, направленную к границе
                        Vector3 velocityAwayFromBoundary = currentVelocity - (normal * velocityTowardsBoundary);
                        rb.linearVelocity = velocityAwayFromBoundary;
                    }

                    // Добавляем небольшое отталкивание
                    rb.AddForce(pushDirection * 5f, ForceMode.Impulse);
                }
            }
        }
    }
}
