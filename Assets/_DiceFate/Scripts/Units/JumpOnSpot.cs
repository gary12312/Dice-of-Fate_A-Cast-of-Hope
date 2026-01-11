using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace DiceFate.Units
{
    public class JumpOnSpot : MonoBehaviour
    {
        [Header("Настройки прыжка")]
        public float jumpHeight = 2f; // Максимальная высота прыжка
        public float jumpDuration = 1f; // Продолжительность прыжка
        public AnimationCurve jumpCurve = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.5f, 1f),
            new Keyframe(1f, 0f)
        ); // Кривая для плавности прыжка

        private NavMeshAgent agent;
        private Vector3 originalPosition; // Исходная позиция фигуры
        private bool isJumping = false; // Выполняется ли прыжок в данный момент
        private Coroutine jumpCoroutine;

        void Start()
        {
            // Получаем компонент NavMeshAgent
            agent = GetComponent<NavMeshAgent>();
        }

        public void StartJump()
        {
            if (isJumping)
            {
                Debug.LogWarning("Прыжок уже выполняется!");
                return;
            }

            if (jumpCoroutine != null)
            {
                StopCoroutine(jumpCoroutine);
            }

            jumpCoroutine = StartCoroutine(JumpToHeight());
        }

        // Корутин для прыжка в указанную позицию
        IEnumerator JumpToHeight()
        {
            isJumping = true;

            // Сохраняем начальную позицию
            Vector3 startPosition = transform.position;

            // Сохраняем состояние агента
            bool wasStopped = agent.isStopped;
            float originalSpeed = agent.speed;

            // Блокируем автоматическое перемещение NavMeshAgent
            agent.isStopped = true;
            agent.velocity = Vector3.zero; // Сбрасываем скорость

            float elapsedTime = 0f;

            while (elapsedTime < jumpDuration)
            {
                // Вычисляем прогресс от 0 до 1
                float progress = elapsedTime / jumpDuration;

                // Вычисляем высоту прыжка по кривой
                float currentHeight = jumpCurve.Evaluate(progress) * jumpHeight;

                // Устанавливаем позицию с учетом высоты прыжка
                Vector3 newPosition = startPosition + Vector3.up * currentHeight;
                transform.position = newPosition;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Убеждаемся, что фигура точно в исходной позиции
            transform.position = startPosition;

            // Восстанавливаем состояние агента
            agent.isStopped = wasStopped;
            agent.speed = originalSpeed;

            // Если агент был активен, обновляем его позицию
            if (!wasStopped)
            {
                agent.Warp(startPosition);
                agent.SetDestination(startPosition);
            }

            isJumping = false;
            jumpCoroutine = null;
        }

        // Метод для проверки, выполняется ли прыжок
        public bool IsJumping()
        {
            return isJumping;
        }

        // Метод для принудительной остановки прыжка
        public void CancelJump()
        {
            if (jumpCoroutine != null)
            {
                StopCoroutine(jumpCoroutine);
                jumpCoroutine = null;
            }

            if (isJumping)
            {
                // Возвращаем на исходную позицию
                if (agent != null)
                {
                    agent.isStopped = false;
                    agent.Warp(transform.position);
                }
                isJumping = false;
            }
        }

        private void OnDestroy()
        {
            // Очистка корутины при уничтожении объекта
            if (jumpCoroutine != null)
            {
                StopCoroutine(jumpCoroutine);
            }
        }
    }
}