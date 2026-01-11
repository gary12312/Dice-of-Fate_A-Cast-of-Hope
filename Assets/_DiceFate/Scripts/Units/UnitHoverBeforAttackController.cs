using DiceFate.EventBus;
using DiceFate.Events;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace DiceFate.Units
{
    // Контроллер для управления зависанием юнита с отдельными кривыми для подъема и опускания
    public class UnitHoverBeforAttackController : MonoBehaviour
    {
        [Header("Основные настройки зависания")]
        [SerializeField] private float hoverHeight = 2f; // Высота зависания
        [SerializeField] private float riseDuration = 0.5f; // Продолжительность подъема
        [SerializeField] private float descendDuration = 0.3f; // Продолжительность опускания

        [Header("Кривые анимации")]
        [SerializeField] //для Прыжка
        private AnimationCurve riseCurve = new AnimationCurve(
            new Keyframe(0f, 0f, 0f, 2f), // Медленно начинаем
            new Keyframe(0.7f, 1.1f, 0f, 0f), // Слегка перелетаем
            new Keyframe(1f, 1f, -2f, 0f) // Плавно останавливаемся
        );

        [SerializeField]
        private AnimationCurve descendCurve = new AnimationCurve(
            new Keyframe(0f, 0f, 2f, 0f), // Быстрый старт
            new Keyframe(0.3f, 0.8f, 0f, 0f), // Замедление
            new Keyframe(1f, 1f, 0f, -2f) // Мягкая посадка
        );

        [Header("Эффекты")]
        [SerializeField] private ParticleSystem descendParticles; // Эффекты при спуске
        [SerializeField] private AudioClip descendSound;
        [SerializeField] private AudioClip riseSound;

        private Vector3 groundPosition;
        private Coroutine jumpCoroutine;
        private AudioSource audioSource;
        private NavMeshAgent agent;
        private bool isJumping = false;
        private bool isFalling = false;


        [Header("Настройки прыжка")]
        public bool isJump = false; // Если true - фигура прыгает вместо плавного перемещения
        public float jumpHeight = 2f; // Максимальная высота прыжка
        public float jumpDuration = 1f; // Продолжительность прыжка
        public AnimationCurve jumpCurve = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.5f, 1f),
            new Keyframe(1f, 0f)
        ); // Кривая для плавности прыжка



        private void Awake()
        {
            InitializeComponents();
        }

        private void Start()
        {
            // Получаем компонент NavMeshAgent
            agent = GetComponent<NavMeshAgent>();
        }

        private void InitializeComponents()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.spatialBlend = 1f;
                audioSource.volume = 0.7f;
            }
        }

        // ------------------------------ Логика -----------------------------

        // Прыжок
        public void JumpingBeforAttack()
        {
            if (isJumping) return;

            if (jumpCoroutine != null)
            {
                StopCoroutine(jumpCoroutine);
            }
            jumpCoroutine = StartCoroutine(JumpSequence());
        }

        private IEnumerator JumpSequence()
        {
            isJumping = true;

            // Сохраняем начальную позицию
            groundPosition = transform.position;

            // Прыжок вверх
            yield return StartCoroutine(RiseCoroutine());

            // Небольшая пауза в верхней точке
            yield return new WaitForSeconds(0.2f);

            // Падение вниз
            yield return StartCoroutine(DescendCoroutine());

            isJumping = false;
        }

        private IEnumerator RiseCoroutine()
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = groundPosition + Vector3.up * hoverHeight;

            // Воспроизводим звук подъема
            if (riseSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(riseSound);
            }

            float elapsedTime = 0f;

            while (elapsedTime < riseDuration)
            {
                float progress = elapsedTime / riseDuration;
                float curveValue = riseCurve.Evaluate(progress);

                // Используем Lerp для плавного перемещения
                transform.position = Vector3.Lerp(startPosition, targetPosition, curveValue);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Гарантируем точное попадание в целевую позицию
            transform.position = targetPosition;
        }

        private IEnumerator DescendCoroutine()
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = groundPosition;

            // Воспроизводим эффекты спуска
            PlayDescendEffects();

            float elapsedTime = 0f;

            while (elapsedTime < descendDuration)
            {
                float progress = elapsedTime / descendDuration;
                float curveValue = descendCurve.Evaluate(progress);

                transform.position = Vector3.Lerp(startPosition, targetPosition, curveValue);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Гарантируем точное попадание в исходную позицию
            transform.position = targetPosition;

            // Разблокируем NavMeshAgent после завершения прыжка
            if (agent != null)
            {
                agent.isStopped = false;
            }
        }

        private void PlayDescendEffects()
        {
            // Воспроизводим звук спуска
            if (descendSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(descendSound);
            }

            // Воспроизводим частицы спуска
            if (descendParticles != null)
            {
                descendParticles.Play();
            }
        }

        // Метод для принудительной остановки прыжка
        public void CancelJump()
        {
            if (jumpCoroutine != null)
            {
                StopCoroutine(jumpCoroutine);
                jumpCoroutine = null;
            }

            // Возвращаем на землю
            if (isJumping || isFalling)
            {
                transform.position = groundPosition;
            }

            // Разблокируем агента
            if (agent != null)
            {
                agent.isStopped = false;
            }

            isJumping = false;
            isFalling = false;
        }

        // Проверка, выполняется ли прыжок
        public bool IsJumping()
        {
            return isJumping || isFalling;
        }

        private void OnDestroy()
        {
            if (jumpCoroutine != null)
            {
                StopCoroutine(jumpCoroutine);
            }
        }
    }
}