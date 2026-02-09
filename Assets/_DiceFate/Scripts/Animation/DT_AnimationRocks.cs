using DG.Tweening;
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
            // ValidateScripts();
            InitializeRocks();
        }

        private void InitializeRocks()
        {
            _initialPosition = new Vector3[rocks.Length]; // Инициализируем массив с нужным размером
            SaveStartPosition();
            SetRocksToBeginPoint();
            SetActiveAllRocks(false);
        }


        public void AnimationRocks(int i)
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


            //      _animationSequence
            //.AppendInterval(0.1f)
            //.Append(cursor.transform.DOMove(pointTwo.transform.position, moveDuration).SetEase(moveCurve))
            //.AppendInterval(pauseBetweenAnimations)
            //.Append(cursor.transform.DORotate(_initialRotation + new Vector3(0, 0, 45), rotationDuration).SetEase(moveCurve))
            //.Append(cursor.transform.DORotate(_initialRotation, rotationDuration).SetEase(moveCurve))
            //.AppendInterval(pauseBetweenAnimations)
            //.AppendCallback(() => cursor.gameObject.SetActive(false))
            //.Play();



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
            };
        }

        private void SetActiveRock(int i, bool isActive) => rocks[i].SetActive(isActive);
     
        private void ValidateScripts()
        {


            //if (rockOne == null)
            //    Debug.LogError($" для {this.name} Не установлена ссылка на rockOne!");
            //if (rockTwo == null)
            //    Debug.LogError($" для {this.name} Не установлена ссылка на rockTwo!");
            //if (rockThree == null)
            //    Debug.LogError($" для {this.name} Не установлена ссылка на rockThree!");
            //if (rockFour == null)
            //    Debug.LogError($" для {this.name} Не установлена ссылка на rockFour!");
            //if (rockFive == null)
            //    Debug.LogError($" для {this.name} Не установлена ссылка на rockFive!");
            //if (rockSix == null)
            //    Debug.LogError($" для {this.name} Не установлена ссылка на rockSix!");

            //if (pointOne == null)
            //    Debug.LogError($" для {this.name} Не установлена ссылка на pointOne!");
            //if (pointTwo == null)
            //    Debug.LogError($" для {this.name} Не установлена ссылка на pointTwo!");
            //if (pointThree == null)
            //    Debug.LogError($" для {this.name} Не установлена ссылка на pointThree!");
            //if (pointFour == null)
            //    Debug.LogError($" для {this.name} Не установлена ссылка на pointFour!");
            //if (pointFive == null)
            //    Debug.LogError($" для {this.name} Не установлена ссылка на pointFive!");
            //if (pointSix == null)
            //    Debug.LogError($" для {this.name} Не установлена ссылка на pointSix!");
        }
    }
}
