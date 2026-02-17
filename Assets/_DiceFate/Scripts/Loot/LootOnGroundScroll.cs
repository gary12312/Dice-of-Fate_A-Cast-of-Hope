using UnityEngine;
using DiceFate.Units;
using DiceFate.Maine;
using DG.Tweening;


namespace DiceFate.Loot
{
    public class LootOnGroundScroll : MonoBehaviour, ILooter, IHover
    {
        //  [field: SerializeField] public bool IsSelected { get; private set; }   // ISelectable
        [field: SerializeField] public bool IsHover { get; private set; }   // IHover

        [Header("Настройки лута")]
        [SerializeField] private ObjectOutline Outline; //обводка
        [SerializeField] private PrologScenario prologScenario;
        [Space]
        [SerializeField] private ParticleSystem particleSystemSparkles;
        [SerializeField] private ParticleSystem particleSystemHit;


        [Header("Настройки обнаружения")]
        [SerializeField] private string targetTag = "Player";   // Тег объекта для обнаружения
        [SerializeField] private float pickupRadius = 2f;       // Радиус обнаружения
        [SerializeField] private LayerMask detectionLayer = ~0; // Слой для поиска (по умолчанию все)
        [SerializeField] private bool showGizmos = true;        // Показывать радиус в редакторе

        [Header("Настройки анимации отказа")]
        [SerializeField] private float bounceHeight = 0.5f; // Высота подпрыгивания
        [SerializeField] private float bounceDuration = 0.5f; // Длительность анимации
        [SerializeField] private int bounceCount = 2; // Количество подпрыгиваний
        [SerializeField]
        public AnimationCurve bounceCurve =
            new AnimationCurve(
                new Keyframe(0f, 0f),
                new Keyframe(1f, 0f)); // Кривая для плавности
        [SerializeField]
        public AnimationCurve selectedCurve =
            new AnimationCurve(
                new Keyframe(0f, 0f),
                new Keyframe(1f, 0f)); // Кривая для плавности при выделении


        private bool _isCanTake = false;
        private Tween _currentFeedbackTween;
        private Vector3 startPosition;


        private void Start()
        {
            ValidateScriptsAndObject();
            InitializationStart();
        }

        private void InitializationStart()
        {
            //prologScenario = GetComponent<PrologScenario>();
            Outline?.DisableOutline();
            _isCanTake = false; // Сбрасываем состояние при старте
            startPosition = transform.position;
        }

        //-------------- IHover реализация --------------
        public void OnEnterHover()
        {
            //if (IsSelected == false)
            //{
            //}
            Outline?.EnableOutline();
            IsHover = true;
        }

        public void OnExitHover()
        {
            //if (IsSelected == false)
            //{
            //}
            Outline?.DisableOutline();
            IsHover = false;
        }

        //-------------- Управление обводкой юнита --------------
        public void OutlineOnSelected() => Outline?.EnableOutline();
        public void OutlineOffSelected() => Outline?.DisableOutline();

        //-------------- LootSelection реализация --------------
        public void LootSelection()
        {
            Debug.Log("Запуск0");
            //TakeScroll();
            // Проверяем есть ли игрок в радиусе
            if (FindUnitsAround())
            {
                Debug.Log("Запуск1");
                prologScenario.StartScenarioNine();

                _isCanTake = true;
                TakeScroll();
            }
            else
            {
                Debug.LogWarning($"Нельзя взять {gameObject.name} - игрок слишком далеко (требуется в радиусе {pickupRadius}м)");
                // Можно добавить визуальную обратную связь (например, мигание красным)
                ShowCannotPickupFeedback();
            }
        }

        //-------------- Метод обнаружения объектов --------------
        private bool FindUnitsAround()
        {
            if (string.IsNullOrEmpty(targetTag))
            {
                Debug.LogError($"Не установлен targetTag для {gameObject.name}");
                return false;
            }

            // Находим все объекты с нужным тегом в радиусе
            GameObject[] targetObjects = GameObject.FindGameObjectsWithTag(targetTag);

            foreach (GameObject target in targetObjects)
            {
                // Проверяем слой, если не все слои выбраны
                if (((1 << target.layer) & detectionLayer.value) == 0)
                    continue;

                float distance = Vector3.Distance(transform.position, target.transform.position);

                if (distance <= pickupRadius)
                {
                    Debug.Log($"Обнаружен {target.name} на расстоянии {distance:F2}м - true");
                    return true;
                }
            }

            // Альтернативный метод через Physics.OverlapSphere (если нужно учитывать коллайдеры)
            // Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRadius, detectionLayer);
            // foreach (Collider collider in colliders)
            // {
            //     if (collider.CompareTag(targetTag))
            //     {
            //         return true;
            //     }
            // }

            return false;
        }

        //-------------- Метод для проверки расстояния до конкретной цели --------------
        public bool IsTargetInRange(GameObject target)
        {
            if (target == null) return false;

            if (!target.CompareTag(targetTag)) return false;

            float distance = Vector3.Distance(transform.position, target.transform.position);
            return distance <= pickupRadius;
        }

        //-------------- Метод для получения ближайшего подходящего объекта --------------
        public GameObject GetNearestTarget()
        {
            GameObject[] targetObjects = GameObject.FindGameObjectsWithTag(targetTag);
            GameObject nearestTarget = null;
            float minDistance = float.MaxValue;

            foreach (GameObject target in targetObjects)
            {
                if (((1 << target.layer) & detectionLayer.value) == 0)
                    continue;

                float distance = Vector3.Distance(transform.position, target.transform.position);

                if (distance <= pickupRadius && distance < minDistance)
                {
                    minDistance = distance;
                    nearestTarget = target;
                }
            }

            return nearestTarget;
        }

        //-------------- Анимация ----------------------------
        private void TakeScroll()
        {
            if (!_isCanTake)
            {
                Debug.LogWarning($"Нельзя взять {gameObject.name} - не пройдена проверка доступности");
                return;
            }

            // Получаем ближайшую цель для дополнительной информации
            GameObject nearestTarget = GetNearestTarget();
            if (nearestTarget != null)
            {
                Debug.Log($"Взятие {gameObject.name} игроком {nearestTarget.name}");
            }

            // Отключаем возможность повторного взятия
            _isCanTake = false;

            // Анимация исчезновения
            //transform
            //    .DOScale(0, 0.5f)
            //    .SetEase(Ease.InBack)
            //    .OnComplete(() =>
            //    {
            //        OnPickupComplete(nearestTarget);
            //    })
            //    .Play();

            Vector3 unitTarget = new Vector3(nearestTarget.transform.position.x,
                                             nearestTarget.transform.position.y + 1,
                                             nearestTarget.transform.position.z);

            DOTween.Sequence()
                .Append(transform.DOMoveY(1f, 0.5f)).SetEase(selectedCurve)
               // .AppendInterval(0.1f)
                .Append(transform.DOMove(unitTarget, 0.5f))
                .Join(transform.DOScale(0, 0.5f))
                .OnComplete(() => OnPickupComplete(nearestTarget))
                .Play();






            //transform
            //    .DOMoveY(2f, 0.5f)          
            //    .OnComplete(() =>
            //    {
            //        OnPickupComplete(nearestTarget);
            //    })
            //    .Play();


            Debug.Log($"Свиток {gameObject.name} взят");

            // Останавливаем частицы
            if (particleSystemSparkles != null)
                particleSystemSparkles.Stop();

            //if (particleSystemHit != null)
            //    particleSystemHit.gameObject.SetActive(true); // Пока отключить

            // Убираем обводку
            Outline?.DisableOutline();
        }

        private void OnPickupComplete(GameObject picker)
        {
            // Здесь можно добавить логику начисления предмета игроку
            // Например: picker.GetComponent<Inventory>()?.AddItem(this);

            // Отключаем или уничтожаем объект
            gameObject.SetActive(false); // или Destroy(gameObject);

            // Можно вызвать событие
            // OnLootPickedUp?.Invoke(this, picker);
        }

        //-------------- Визуальная обратная связь при невозможности взять --------------
        private void ShowCannotPickupFeedback()
        {
            Outline?.ChangeColorOutline(Color.red);
            gameObject.transform.position = startPosition;

            if (_currentFeedbackTween != null && _currentFeedbackTween.IsActive()) // Останавливаем предыдущую анимацию если она есть
                _currentFeedbackTween.Kill();

            StartScenarioFour();

            Sequence bounceSequence = DOTween.Sequence();
            // Подъем вверх
            bounceSequence.Append(
                transform.DOMoveY(startPosition.y + bounceHeight, bounceDuration / 2)
                    .SetEase(Ease.OutQuad)
            );
            // Падение вниз
            bounceSequence.Append(
                transform.DOMoveY(startPosition.y, bounceDuration / 2)
                    .SetEase(bounceCurve)
            );
            // Возвращаем объект в точную начальную позицию (на случай накопления ошибок)
            bounceSequence.Append(
                transform.DOMove(startPosition, 0.1f)
            );

            _currentFeedbackTween = bounceSequence;   // Сохраняем твин для возможности отмены

            _currentFeedbackTween.OnComplete(() =>    // Устанавливаем автоочистку
            {
                Outline?.ResetColorOutline();
                Outline?.DisableOutline();
                _currentFeedbackTween = null;


            });
            bounceSequence.Play();
        }

        private void StartScenarioFour()
        {
            if (prologScenario.prologNumber == 3)
            {
                prologScenario.StartScenarioFour();
            }
        }

        //-------------- Валидация --------------
        private void ValidateScriptsAndObject()
        {
            if (Outline == null)
                Debug.LogError($"Установить Outline для {gameObject.name}");

            if (string.IsNullOrEmpty(targetTag))
                Debug.LogError($"Установить targetTag для {gameObject.name}");

            if (particleSystemSparkles == null)
                Debug.LogWarning($"particleSystemSparkles не установлен для {gameObject.name}");

            if (particleSystemHit == null)
                Debug.LogWarning($"particleSystemHit не установлен для {gameObject.name}");

            if (pickupRadius <= 0)
            {
                Debug.LogWarning($"pickupRadius должен быть больше 0 для {gameObject.name}");
                pickupRadius = 2f; // Значение по умолчанию
            }


        }

        //-------------- Метод для сброса состояния --------------
        public void ResetLoot()
        {
            _isCanTake = false;
            //IsSelected = false;
            IsHover = false;
            transform.localScale = Vector3.one;
            gameObject.SetActive(true);

            // Сбрасываем частицы
            if (particleSystemSparkles != null)
            {
                particleSystemSparkles.Play();
            }

            if (particleSystemHit != null)
            {
                particleSystemHit.gameObject.SetActive(false);
            }

            Outline?.DisableOutline();
        }

        //-------------- Метод для принудительного взятия --------------
        public void ForcePickup(GameObject picker = null)
        {
            _isCanTake = true;
            TakeScroll();
        }

        //-------------- Метод для проверки доступности без взятия --------------
        public bool CheckIfCanBePicked()
        {
            return FindUnitsAround();
        }

        //-------------- Gizmos для визуализации в редакторе --------------
        private void OnDrawGizmosSelected()
        {
            if (!showGizmos) return;

            // Рисуем радиус обнаружения
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, pickupRadius);

            // Если в сцене есть объекты с нужным тегом, показываем расстояние до них
            if (!Application.isPlaying || string.IsNullOrEmpty(targetTag)) return;

            GameObject[] targetObjects = GameObject.FindGameObjectsWithTag(targetTag);
            foreach (GameObject target in targetObjects)
            {
                if (((1 << target.layer) & detectionLayer.value) == 0)
                    continue;

                float distance = Vector3.Distance(transform.position, target.transform.position);
                Gizmos.color = distance <= pickupRadius ? Color.green : Color.red;
                Gizmos.DrawLine(transform.position, target.transform.position);

                // Подпись с расстоянием
#if UNITY_EDITOR
                UnityEditor.Handles.Label(
                    Vector3.Lerp(transform.position, target.transform.position, 0.5f),
                    $"{distance:F1}m"
                );
#endif
            }
        }

        private void OnDrawGizmos()
        {
            if (!showGizmos || !Application.isPlaying) return;

            // Во время игры показываем статус доступности
            if (FindUnitsAround())
            {
                Gizmos.color = new Color(0, 1, 0, 0.3f); // Зеленый полупрозрачный
                Gizmos.DrawSphere(transform.position, pickupRadius);
            }
        }


    }
}