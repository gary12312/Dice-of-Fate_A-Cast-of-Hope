using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using DiceFate.Maine;

namespace DiceFate.MouseW
{
    public class CursorTutorial : MonoBehaviour
    {
        [SerializeField] private Image cursor;
        [SerializeField] private GameObject pointStart;
        [SerializeField] private GameObject pointOne;
        [SerializeField] private GameObject pointTwo;
        [SerializeField] private GameObject pointThree;
        [SerializeField] private GameObject pointFour;
        [SerializeField] private GameObject pointFive;
        [SerializeField] private float moveDuration = 1f;
        [SerializeField] private float rotationDuration = 0.5f;
        [SerializeField] private float pauseBetweenAnimations = 0.1f;
        [SerializeField]
        public AnimationCurve moveCurve = new AnimationCurve(
                new Keyframe(0f, 0f),
                new Keyframe(1f, 0f)); // Кривая для плавности прыжка

        [Space]
        [SerializeField] private PrologScenario prologScenario;


        private Sequence _animationSequence;
        private Sequence _animationSequenceSix;
        private Vector3 _initialRotation;


        private void Start()
        {
            if (cursor == null || pointStart == null || pointOne == null)
            {
                Debug.LogError($"CursorTutorial: Не все ссылки установлены! для {this.name}");
                return;
            }

            InitializeCursor();
        }

        private void InitializeCursor()
        {
            // Сохраняем начальное вращение
            _initialRotation = cursor.transform.eulerAngles;

            // Устанавливаем курсор в начальную позицию
            cursor.transform.position = pointStart.transform.position;
            cursor.transform.eulerAngles = _initialRotation;

            // Делаем курсор видимым
            cursor.gameObject.SetActive(false);
        }

        public void AnimationCursorForPrologFour()
        {
            StopAnimation();
            cursor.gameObject.SetActive(true);

            // Создаем новую последовательность
            _animationSequence = DOTween.Sequence();

            _animationSequence.AppendInterval(0.1f);

            // 1. Движение из точки A в точку B
            _animationSequence
                .Append(cursor.transform.DOMove(pointOne.transform.position, moveDuration).SetEase(moveCurve));

            // 2. Пауза
            _animationSequence.AppendInterval(pauseBetweenAnimations);

            // 3. Поворот на 90 градусов по Z
            _animationSequence.Append(
                cursor.transform.DORotate(_initialRotation + new Vector3(0, 0, 45), rotationDuration)
                    .SetEase(moveCurve));

            // 4. Пауза
            // _animationSequence.AppendInterval(pauseBetweenAnimations);

            // 5. Поворот обратно
            _animationSequence.Append(
                cursor.transform.DORotate(_initialRotation, rotationDuration)
                    .SetEase(moveCurve));

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

        public void AnimationCursorForPrologFive()
        {
            StopAnimation();
            cursor.gameObject.SetActive(true);
            cursor.transform.position = pointOne.transform.position;
            cursor.transform.eulerAngles = _initialRotation;

            _animationSequence = DOTween.Sequence();
            // _animationSequence.AppendInterval(0.1f);
            //_animationSequence.Append(pointTwo.transform.DOMove(pointThree.transform.position, moveDuration).SetEase(moveCurve)
            //    .Play());
            _animationSequence
               .AppendInterval(0.1f)
               .Append(cursor.transform.DOMove(pointTwo.transform.position, moveDuration).SetEase(moveCurve))
               .AppendInterval(pauseBetweenAnimations)
               .Append(cursor.transform.DORotate(_initialRotation + new Vector3(0, 0, 45), rotationDuration).SetEase(moveCurve))
               .Append(cursor.transform.DORotate(_initialRotation, rotationDuration).SetEase(moveCurve))
               .AppendInterval(pauseBetweenAnimations)
               .AppendCallback(() => cursor.gameObject.SetActive(false))
               .Play();
        }

        public void AnimationCursorForPrologSix()
        {
            StopAnimation();
            cursor.gameObject.SetActive(true);
            cursor.transform.position = pointThree.transform.position;
            cursor.transform.eulerAngles = _initialRotation;

            _animationSequence = DOTween.Sequence();
            _animationSequence
               .AppendInterval(0.1f)
               .Append(cursor.transform.DORotate(_initialRotation + new Vector3(0, 0, 45), rotationDuration).SetEase(moveCurve))
               .Append(cursor.transform.DOMove(pointFour.transform.position, moveDuration).SetEase(moveCurve))
               .AppendInterval(pauseBetweenAnimations)
               .Append(cursor.transform.DORotate(_initialRotation, rotationDuration).SetEase(moveCurve))
               .AppendInterval(pauseBetweenAnimations)
               .Append(cursor.transform.DOMove(pointFive.transform.position, moveDuration).SetEase(moveCurve))
               .AppendInterval(pauseBetweenAnimations)
               .Append(cursor.transform.DORotate(_initialRotation + new Vector3(0, 0, 45), rotationDuration).SetEase(moveCurve))
               .Append(cursor.transform.DORotate(_initialRotation, rotationDuration).SetEase(moveCurve))
               .AppendInterval(pauseBetweenAnimations)
               .AppendCallback(() => cursor.gameObject.SetActive(false))
               .Play();
        }

        public void AnimationCursorForPrologSixPartOne()
        {
            StopAnimation();
            cursor.gameObject.SetActive(true);
            cursor.transform.position = pointThree.transform.position;
            cursor.transform.eulerAngles = _initialRotation;

            _animationSequence = DOTween.Sequence();
            _animationSequence
               .AppendInterval(0.1f)
               .Append(cursor.transform.DORotate(_initialRotation + new Vector3(0, 0, 45), rotationDuration).SetEase(moveCurve))
               .Append(cursor.transform.DOMove(pointFour.transform.position, moveDuration).SetEase(moveCurve))
               .AppendInterval(pauseBetweenAnimations)
               .Append(cursor.transform.DORotate(_initialRotation, rotationDuration).SetEase(moveCurve))
               .AppendInterval(pauseBetweenAnimations)
               .Append(cursor.transform.DOMove(pointFive.transform.position, moveDuration).SetEase(moveCurve))
               .AppendInterval(pauseBetweenAnimations)
               .Play();
        }
        public void AnimationCursorForPrologSixPartTwo()
        {
            StopAnimation();
            cursor.gameObject.SetActive(true);
            cursor.transform.position = pointThree.transform.position;
            cursor.transform.eulerAngles = _initialRotation;

            _animationSequence = DOTween.Sequence();
            _animationSequence
            .Append(cursor.transform.DORotate(_initialRotation + new Vector3(0, 0, 45), rotationDuration).SetEase(moveCurve))
            .Append(cursor.transform.DORotate(_initialRotation, rotationDuration).SetEase(moveCurve))
            .AppendInterval(pauseBetweenAnimations)
            .AppendCallback(() => cursor.gameObject.SetActive(false))
            .Play();
        }



        public void StopAnimation()
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
            AnimationCursorForPrologFour();
        }

        // Метод для изменения точек движения
        public void SetPoints(GameObject start, GameObject finish)
        {
            pointStart = start;
            pointOne = finish;
            RestartAnimation();
        }
    }
}