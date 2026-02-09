using UnityEngine;
using System.Collections;


    public class CameraShakeAdvanced : MonoBehaviour
    {
        [Header("Настройки тряски")]
        [SerializeField] private float shakeDuration = 0.5f;    // Продолжительность тряски
        [SerializeField] private float shakeMagnitude = 0.3f;   // Сила тряски
        [SerializeField]
        private AnimationCurve dampCurve =     // Кривая затухания
            AnimationCurve.Linear(0, 1, 1, 0);

        [Header("Дополнительные параметры")]
        [SerializeField] private bool shakeOnX = true;  // Тряска по оси X
        [SerializeField] private bool shakeOnY = true;  // Тряска по оси Y
        [SerializeField] private bool shakeOnZ = true;  // Тряска по оси Z

        // Внутренние переменные
        private Vector3 initialPosition;
        private float currentShakeDuration;
        private bool isShaking = false;
        private Coroutine shakeCoroutine;

        void Awake()
        {
            initialPosition = transform.localPosition;
        }


        // Запуск тряски камеры
        public void Shake()
        {
            Shake(shakeDuration, shakeMagnitude);
        }


        // Запуск тряски с кастомными параметрами
        public void Shake(float duration, float magnitude)
        {
            // Останавливаем предыдущую тряску если есть
            if (shakeCoroutine != null)
            {
                StopCoroutine(shakeCoroutine);
            }

            // Запускаем новую тряску
            shakeCoroutine = StartCoroutine(ShakeCoroutine(duration, magnitude));
        }


        // Корутина для плавной тряски
        private IEnumerator ShakeCoroutine(float duration, float magnitude)
        {
            initialPosition = transform.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                // Вычисляем множитель затухания по кривой
                float strength = dampCurve.Evaluate(elapsed / duration);

                // Генерируем смещение с учетом осей
                Vector3 shakeOffset = Vector3.zero;

                if (shakeOnX) shakeOffset.x = Random.Range(-1f, 1f);
                if (shakeOnY) shakeOffset.y = Random.Range(-1f, 1f);
                if (shakeOnZ) shakeOffset.z = Random.Range(-1f, 1f);

                // Нормализуем и применяем силу
                shakeOffset = shakeOffset.normalized * magnitude * strength;

                // Применяем смещение
                transform.localPosition = initialPosition + shakeOffset;

                yield return null;
            }

            // Возвращаем камеру на место
            transform.localPosition = initialPosition;
            shakeCoroutine = null;
        }


        // Немедленная остановка тряски
        public void StopShake()
        {
            if (shakeCoroutine != null)
            {
                StopCoroutine(shakeCoroutine);
                transform.localPosition = initialPosition;
                shakeCoroutine = null;
            }
        }
    }

