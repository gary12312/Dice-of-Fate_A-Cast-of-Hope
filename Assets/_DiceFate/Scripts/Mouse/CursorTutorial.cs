using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace DiceFate.MouseW
{
    public class CursorTutorial : MonoBehaviour
    {
        [SerializeField] private Image cursor;
        [SerializeField] private GameObject pointStart;
        [SerializeField] private GameObject pointFinish;
        [SerializeField] private float moveDuration = 1f;
        [SerializeField] private float rotationDuration = 0.5f;
        [SerializeField] private float pauseBetweenAnimations = 0.1f;
        [SerializeField]
        public AnimationCurve moveCurve = new AnimationCurve(
                new Keyframe(0f, 0f),
                new Keyframe(1f, 0f)); // Кривая для плавности прыжка

        private Sequence _animationSequence;
        private Vector3 _initialRotation;

        private void OnEnable()
        {
            if (cursor == null || pointStart == null || pointFinish == null)
            {
                Debug.LogError("CursorTutorial: Не все ссылки установлены!");
                return;
            }

            InitializeCursor();
            StartAnimation();
        }

        private void OnDisable()
        {
            StopAnimation();
        }

        private void InitializeCursor()
        {
            // Сохраняем начальное вращение
            _initialRotation = cursor.transform.eulerAngles;

            // Устанавливаем курсор в начальную позицию
            cursor.transform.position = pointStart.transform.position;
            cursor.transform.eulerAngles = _initialRotation;

            // Делаем курсор видимым
            cursor.gameObject.SetActive(true);
        }

        private void StartAnimation()
        {
            // Создаем новую последовательность
            _animationSequence = DOTween.Sequence();



            //.Append(transform.DOMove(transform.position + Vector3.up * 3, 0.5f))
            //   .Join(transform.DORotate(Vector3.up * 180, 0.5f))
            //   .Append(transform.DOScale(0, 0.25f).SetEase(Ease.InQuint))
            //   .Join(transform.DORotate(Vector3.up * 720, 0.25f, RotateMode.FastBeyond360))
            //   .Play()
            //   .WaitForCompletion();



            _animationSequence.AppendInterval(0.1f);

            // 1. Движение из точки A в точку B
            _animationSequence
                .Append(cursor.transform.DOMove(pointFinish.transform.position, moveDuration).SetEase(moveCurve)
            );

            // 2. Пауза
            _animationSequence.AppendInterval(pauseBetweenAnimations);

            // 3. Поворот на 90 градусов по Z
            _animationSequence.Append(
                cursor.transform.DORotate(_initialRotation + new Vector3(0, 0, 45), rotationDuration)
                    .SetEase(moveCurve)
            );

            // 4. Пауза
           // _animationSequence.AppendInterval(pauseBetweenAnimations);

            // 5. Поворот обратно
            _animationSequence.Append(
                cursor.transform.DORotate(_initialRotation, rotationDuration)
                    .SetEase(moveCurve)
            );

            // 6. Пауза
            _animationSequence.AppendInterval(pauseBetweenAnimations);


            // 9. Скрываем курсор
            _animationSequence.AppendCallback(() => cursor.gameObject.SetActive(false));


            //// 11. Повторяем анимацию с начала
            //_animationSequence.AppendCallback(() =>
            //{
            //    if (cursor != null && cursor.gameObject.activeInHierarchy)
            //    {
            //        InitializeCursor();
            //    }
            //});

            //// Устанавливаем бесконечное повторение
            //_animationSequence.SetLoops(-1);

            // Запускаем анимацию
            _animationSequence.Play();
        }

        private void StopAnimation()
        {
            if (_animationSequence != null)
            {
                _animationSequence.Kill();
                _animationSequence = null;
            }

            // Скрываем курсор при отключении
            if (cursor != null)
            {
                cursor.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            StopAnimation();
        }

        // Метод для ручного запуска/остановки анимации
        public void RestartAnimation()
        {
            StopAnimation();
            InitializeCursor();
            StartAnimation();
        }

        // Метод для изменения точек движения
        public void SetPoints(GameObject start, GameObject finish)
        {
            pointStart = start;
            pointFinish = finish;
            RestartAnimation();
        }
    }
}