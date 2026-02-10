using DG.Tweening;
using System.Collections;
using UnityEngine;


namespace DiceFate.Animation
{
    public class DT_AnimationRocks : MonoBehaviour
    {
        [SerializeField] private GameObject[] rocks;
        [Space]
        [SerializeField] private Transform[] points;
        [Space]
        [SerializeField] private float moveDuration = 2f;
        [SerializeField] private float pauseBetweenAnimations = 0.1f;
        [SerializeField]
        public AnimationCurve moveCurve = new AnimationCurve(
                new Keyframe(0f, 0f),
                new Keyframe(1f, 0f)); // Кривая для плавности прыжка
            

        private Vector3[] _initialPosition;

        private void Start()
        {
            ValidateScripts();
            InitializeRocks();
        }

        private void InitializeRocks()
        {
            _initialPosition = new Vector3[rocks.Length]; // Инициализируем массив с нужным размером
            SaveStartPosition();
            SetRocksToBeginPoint();
            SetActiveAllRocks(false);
        }

        public void StartAnimationRocks() => StartCoroutine(AnimationRocks());  
    

        private IEnumerator AnimationRocks()
        {        
            yield return new WaitForSeconds(pauseBetweenAnimations);
            AnimationRock(0);

            yield return new WaitForSeconds(pauseBetweenAnimations);
            AnimationRock(2);

            yield return new WaitForSeconds(pauseBetweenAnimations);
            AnimationRock(1);

            yield return new WaitForSeconds(pauseBetweenAnimations);
            AnimationRock(3);

            yield return new WaitForSeconds(pauseBetweenAnimations);
            AnimationRock(5);

            yield return new WaitForSeconds(pauseBetweenAnimations);
            AnimationRock(4);
        }

        public void AnimationRock(int i)
        {
            if (i < 0 || i >= rocks.Length) { return; }

            //  StopAnimation();
            SetActiveRock(i, true);

            Sequence _animationSequence;
            _animationSequence = DOTween.Sequence();
            _animationSequence
                .AppendInterval(0.1f)
                .Append(rocks[i].transform.DOMove(_initialPosition[i], moveDuration).SetEase(moveCurve))
                .Play();
        }

        private void SaveStartPosition()
        {
            for (int i = 0; i < rocks.Length; i++)
            {
                _initialPosition[i] = rocks[i].transform.position;
            }
        }

        private void SetRocksToBeginPoint()
        {
            for (int i = 0; i < rocks.Length; i++)
            {
                rocks[i].transform.position = points[i].position;
            }
        }

        private void SetActiveAllRocks(bool isActive)
        {
            foreach (var item in rocks)
            {
                item.SetActive(isActive);
            }
            ;
        }

        private void SetActiveRock(int i, bool isActive) => rocks[i].SetActive(isActive);

        private void ValidateScripts()
        {
            if (rocks == null)
                Debug.LogError($" для {this.name} Не установленs объекты на rocks!");

            if (points == null)
                Debug.LogError($" для {this.name} Не установленs объекты на points!");

            if (rocks.Length != points.Length)
                Debug.LogError($" для {this.name} Количество объект��в на rocks и points должно быть одинаковым!");
        }
    }
}
