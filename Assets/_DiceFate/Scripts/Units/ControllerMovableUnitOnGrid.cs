using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using DiceFate.Events;
using DiceFate.EventBus;

namespace DiceFate.Units
{
    public class ControllerMovableUnitOnGrid : MonoBehaviour
    {
        [Header("Настройки перемещения")]
        public float moveDistance = 20f; // Максимальная дистанция перемещения
        public float liftHeight = 0.5f; // Высота подъема при движении

        [Header("Настройки прыжка")]
        public bool isJump = false; // Если true - фигура прыгает вместо плавного перемещения
        public float jumpHeight = 2f; // Максимальная высота прыжка
        public float jumpDuration = 1f; // Продолжительность прыжка
        public AnimationCurve jumpCurve = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.5f, 1f),
            new Keyframe(1f, 0f)
        ); // Кривая для плавности прыжка

        [Header("Настройки поворота")]
        public bool enableRotation = true; // Включить поворот в сторону движения
        public float rotationSpeed = 5f; // Скорость поворота
        public float rotationThreshold = 0.1f; // Минимальная дистанция для начала поворота
        public bool keepFinalRotation = true; // Сохранять финальный поворот после движения

        private DF_MiniFigureIsSelectable selectionController; // Контроллер выделения
        public GridMovableForUnit gridSystem; // Система сетки

        private NavMeshAgent agent;
        private Vector3 originalPosition; // Исходная позиция фигуры
        private bool isMoving = false; // Выполняется ли перемещение в данный момент
        private Quaternion targetRotation; // Целевой поворот

        private ISelectable selectedUnit; // Текущий выделенный юнит который имеет интерфейс ISelectable


        public bool IsSelected => throw new System.NotImplementedException();

        private void Awake()
        {
            Bus<UnitSelectedEvent>.OnEvent += HandelUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent += HandeleUnitDeselect;
        }
        private void OnDestroy()
        {
            Bus<UnitSelectedEvent>.OnEvent -= HandelUnitSelected;
            Bus<UnitDeselectedEvent>.OnEvent -= HandeleUnitDeselect;
        }

        void Start()
        {
            // Получаем компонент NavMeshAgent
            agent = GetComponent<NavMeshAgent>();

            // Получаем компонент выделения
            selectionController = GetComponent<DF_MiniFigureIsSelectable>();
            if (selectionController == null)
            {
                Debug.LogError("DF_MiniFigureIsSelectable не найден на объекте!");
                return;
            }

            // Подписываемся на события выделения

            //Bus<UnitSelectedEvent>.OnEvent += HandelUnitSelected;
            //Bus<UnitDeselectedEvent>.OnEvent += HandeleUnitDeselect;

            // Сохраняем исходную позицию для ограничений перемещения
            originalPosition = transform.position;

            // Инициализируем систему сетки
            gridSystem = GetComponent<GridMovableForUnit>();

            // Инициализируем целевой поворот
            targetRotation = transform.rotation;
        }

        void Update()
        {
            //if (selectedUnit != null)
            //{ SelectFigure(); }
            //else
            //{ DeselectFigure(); }

            //// Если фигура выделена и был клик по земле - перемещаем
            //if (selectionController.IsSelected() && Input.GetMouseButtonDown(0) && !isMoving)
            //{
            //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //    RaycastHit hit;

            //    if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Ground"))
            //    {
            //        // Используем позицию из системы круговой сетки
            //        Vector3 targetPosition = gridSystem.GetCircularGridPosition(hit.point);

            //        // Проверяем, что позиция валидная (не нулевая)
            //        if (targetPosition != Vector3.zero)
            //        {
            //            MoveToPosition(targetPosition);
            //        }
            //        else
            //        {
            //            Debug.Log("Не удалось определить позицию на сетке");
            //        }
            //    }
            //}

            //// Плавный поворот к целевой ориентации
            //if (enableRotation && transform.rotation != targetRotation)
            //{
            //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            //}
        }


        //---------------------------------- запуск событий    ----------------------------------
        private void HandelUnitSelected(UnitSelectedEvent evt)
        {
            // selectedUnit = evt.Unit; // Обновить текущий выделенный юнит
            //  EnaibleGrid();
                        
            if (gridSystem != null) gridSystem.EnablePreview(transform.position); // Включаем систему предпросмотра круговой сетки с центром в позиции фигуры
        }

        private void HandeleUnitDeselect(UnitDeselectedEvent evt)
        {
            // selectedUnit = null;  // Сбросить текущий выделенный юнит          
            // DisableGrid();
                        
            if (gridSystem != null) gridSystem.DisablePreview(); // Выключаем систему предпросмотра сетки
        }



        // Метод выделения Сетки
        public void EnaibleGrid()
        {
            // Включаем систему предпросмотра круговой сетки с центром в позиции фигуры
            if (gridSystem != null) gridSystem.EnablePreview(transform.position);
        }

        // Метод отмены сетки
        public void DisableGrid()
        {
            // Выключаем систему предпросмотра сетки
            if (gridSystem != null) gridSystem.DisablePreview();

        }





        // Проверка возможности перемещения в указанную позицию
        bool CanMoveToPosition(Vector3 targetPosition)
        {
            // Проверяем дистанцию от исходной позиции
            float distance = Vector3.Distance(originalPosition, targetPosition);

            if (distance > moveDistance)
            {
                Debug.Log("Слишком далеко! Дистанция: " + distance + ", максимум: " + moveDistance);
                return false;
            }

            // Дополнительная проверка на NavMesh
            NavMeshHit navHit;
            if (!NavMesh.SamplePosition(targetPosition, out navHit, 1.0f, NavMesh.AllAreas))
            {
                Debug.Log("Целевая позиция недоступна на NavMesh");
                return false;
            }

            return true;
        }

        // Перемещение фигуры в указанную позицию
        public void MoveToPosition(Vector3 targetPosition)
        {
            // Проверяем возможность перемещения
            if (!CanMoveToPosition(targetPosition))
            {
                Debug.Log("Невозможно переместиться в указанную позицию");
                return;
            }

            Debug.Log("Начинаем перемещение в позицию: " + targetPosition);

            // Вычисляем направление для поворота (только горизонтальное)
            Vector3 direction = GetHorizontalDirection(targetPosition);
            if (direction.magnitude > rotationThreshold && enableRotation)
            {
                // Вычисляем поворот только вокруг оси Y
                targetRotation = GetYRotationOnly(direction);
            }

            // Запускаем соответствующую корутину в зависимости от режима
            if (isJump)
            {
                StartCoroutine(JumpToPosition(targetPosition));
            }
            else
            {
                StartCoroutine(MoveWithLift(targetPosition));
            }

            // Снимаем выделение после начала перемещения
            selectionController.DeselectFigure();
        }

        // Получение горизонтального направления (игнорирует высоту)
        Vector3 GetHorizontalDirection(Vector3 targetPosition)
        {
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0; // Игнорируем вертикальную составляющую
            return direction.normalized;
        }

        // Получение поворота только вокруг оси Y (сохраняет X и Z)
        Quaternion GetYRotationOnly(Vector3 direction)
        {
            if (direction.magnitude < rotationThreshold)
                return transform.rotation;

            // Создаем поворот только вокруг оси Y
            float targetYAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            // Возвращаем чистый поворот по Y, без влияния на X и Z
            return Quaternion.Euler(0f, targetYAngle, 0f);
        }

        // Корутин для плавного перемещения с подъемом (оригинальный метод)
        IEnumerator MoveWithLift(Vector3 targetPosition)
        {
            isMoving = true;
            Debug.Log("Запуск корутина плавного перемещения");

            // Блокируем автоматическое перемещение NavMeshAgent
            agent.isStopped = true;

            Vector3 startPosition = transform.position;
            Vector3 raisedPosition = startPosition + Vector3.up * liftHeight;
            Vector3 targetRaisedPosition = targetPosition + Vector3.up * liftHeight;

            Debug.Log("Начальная позиция: " + startPosition);
            Debug.Log("Целевая позиция: " + targetPosition);

            // Поворачиваем в сторону движения в начале перемещения
            if (enableRotation)
            {
                Vector3 initialDirection = GetHorizontalDirection(targetPosition);
                if (initialDirection.magnitude > rotationThreshold)
                {
                    targetRotation = GetYRotationOnly(initialDirection);
                    // Ждем немного чтобы поворот начался
                    yield return new WaitForSeconds(0.1f);
                }
            }

            // Поднимаем фигуру
            float liftDuration = 0.3f;
            float elapsedTime = 0f;

            while (elapsedTime < liftDuration)
            {
                transform.position = Vector3.Lerp(startPosition, raisedPosition, elapsedTime / liftDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Debug.Log("Фаза подъема завершена");

            // Перемещаем по горизонтали
            float moveDuration = Vector3.Distance(raisedPosition, targetRaisedPosition) / agent.speed;
            elapsedTime = 0f;

            while (elapsedTime < moveDuration)
            {
                transform.position = Vector3.Lerp(raisedPosition, targetRaisedPosition, elapsedTime / moveDuration);

                // Плавно поворачиваем во время движения (только по Y)
                if (enableRotation)
                {
                    Vector3 currentDirection = GetHorizontalDirection(targetPosition);
                    if (currentDirection.magnitude > rotationThreshold)
                    {
                        targetRotation = GetYRotationOnly(currentDirection);
                    }
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Debug.Log("Фаза горизонтального перемещения завершена");

            // Опускаем фигуру
            elapsedTime = 0f;
            while (elapsedTime < liftDuration)
            {
                transform.position = Vector3.Lerp(targetRaisedPosition, targetPosition, elapsedTime / liftDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Debug.Log("Фаза опускания завершена");

            // Устанавливаем финальную позицию
            transform.position = targetPosition;

            // Финальный поворот для точности (сохраняем направление движения)
            if (enableRotation && keepFinalRotation)
            {
                Vector3 finalDirection = GetHorizontalDirection(targetPosition);
                if (finalDirection.magnitude > rotationThreshold)
                {
                    Quaternion finalRotation = GetYRotationOnly(finalDirection);
                    transform.rotation = finalRotation;
                    targetRotation = finalRotation; // Синхронизируем
                }
            }

            // Включаем обратно NavMeshAgent
            agent.isStopped = false;
            agent.SetDestination(targetPosition);

            Debug.Log("Плавное перемещение завершено");
            isMoving = false;
        }

        // Корутин для прыжка в указанную позицию
        IEnumerator JumpToPosition(Vector3 targetPosition)
        {
            isMoving = true;
            Debug.Log("Запуск корутина прыжка");

            // Блокируем автоматическое перемещение NavMeshAgent
            agent.isStopped = true;

            Vector3 startPosition = transform.position;

            Debug.Log("Начальная позиция прыжка: " + startPosition);
            Debug.Log("Целевая позиция прыжка: " + targetPosition);

            // Поворачиваем в сторону движения перед прыжком (только по Y)
            if (enableRotation)
            {
                Vector3 jumpDirection = GetHorizontalDirection(targetPosition);
                if (jumpDirection.magnitude > rotationThreshold)
                {
                    targetRotation = GetYRotationOnly(jumpDirection);
                    // Ждем немного чтобы поворот начался перед прыжком
                    yield return new WaitForSeconds(0.1f);
                }
            }

            float elapsedTime = 0f;

            while (elapsedTime < jumpDuration)
            {
                // Вычисляем прогресс от 0 до 1
                float progress = elapsedTime / jumpDuration;

                // Плавно интерполируем горизонтальную позицию
                Vector3 horizontalPosition = Vector3.Lerp(startPosition, targetPosition, progress);

                // Вычисляем высоту прыжка по кривой
                float currentHeight = jumpCurve.Evaluate(progress) * jumpHeight;

                // Устанавливаем позицию с учетом высоты прыжка
                transform.position = horizontalPosition + Vector3.up * currentHeight;

                // Плавно поворачиваем во время прыжка (только по Y)
                if (enableRotation)
                {
                    Vector3 currentDirection = GetHorizontalDirection(targetPosition);
                    if (currentDirection.magnitude > rotationThreshold)
                    {
                        targetRotation = GetYRotationOnly(currentDirection);
                    }
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Убеждаемся, что фигура точно в целевой позиции
            transform.position = targetPosition;

            // Финальный поворот для точности (сохраняем направление движения)
            if (enableRotation && keepFinalRotation)
            {
                Vector3 finalDirection = GetHorizontalDirection(targetPosition);
                if (finalDirection.magnitude > rotationThreshold)
                {
                    Quaternion finalRotation = GetYRotationOnly(finalDirection);
                    transform.rotation = finalRotation;
                    targetRotation = finalRotation; // Синхронизируем
                }
            }

            // Включаем обратно NavMeshAgent
            agent.isStopped = false;
            agent.SetDestination(targetPosition);

            Debug.Log("Прыжок завершен");
            isMoving = false;
        }

        // Метод для принудительного поворота в указанную сторону (только Y)
        public void RotateTowards(Vector3 direction)
        {
            if (enableRotation && direction.magnitude > rotationThreshold)
            {
                Vector3 horizontalDirection = direction;
                horizontalDirection.y = 0;
                horizontalDirection = horizontalDirection.normalized;

                targetRotation = GetYRotationOnly(horizontalDirection);
            }
        }

        // Метод для принудительного поворота к позиции (только Y)
        public void RotateTowardsPosition(Vector3 targetPosition)
        {
            if (enableRotation)
            {
                Vector3 direction = GetHorizontalDirection(targetPosition);
                if (direction.magnitude > rotationThreshold)
                {
                    targetRotation = GetYRotationOnly(direction);
                }
            }
        }

        // Метод для принудительной остановки перемещения
        public void StopMovement()
        {
            StopAllCoroutines();
            isMoving = false;

            if (agent != null)
            {
                agent.isStopped = false;
            }
        }

        // Метод для проверки, выполняется ли перемещение
        public bool IsMoving()
        {
            return isMoving;
        }

        // Метод для смены режима перемещения/прыжка во время выполнения
        public void SetJumpMode(bool jumpEnabled)
        {
            isJump = jumpEnabled;
            Debug.Log("Режим прыжка " + (jumpEnabled ? "включен" : "выключен"));
        }

        // Метод для настройки параметров поворота
        public void SetRotationParameters(bool enabled, float speed = 5f, float threshold = 0.1f, bool keepRotation = true)
        {
            enableRotation = enabled;
            rotationSpeed = speed;
            rotationThreshold = threshold;
            keepFinalRotation = keepRotation;
            Debug.Log("Параметры поворота обновлены: enabled=" + enabled + ", speed=" + speed + ", threshold=" + threshold);
        }

        // Метод для настройки параметров прыжка
        public void SetJumpParameters(float height, float duration, AnimationCurve curve = null)
        {
            jumpHeight = height;
            jumpDuration = duration;

            if (curve != null)
            {
                jumpCurve = curve;
            }

            Debug.Log("Параметры прыжка обновлены: высота=" + height + ", длительность=" + duration);
        }


    }

}
