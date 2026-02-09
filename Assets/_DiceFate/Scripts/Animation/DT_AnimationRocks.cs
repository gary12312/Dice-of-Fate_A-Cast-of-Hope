using DG.Tweening;
using DiceFate.Maine;
using DiceFate.UI;
using UnityEditor.Overlays;
using UnityEngine;
using Архив;

namespace DiceFate.Animation
{
    public class DT_AnimationRocks : MonoBehaviour
    {


        [SerializeField] private GameObject[] rocks;
        [SerializeField] private GameObject rockOne;
        [SerializeField] private GameObject rockTwo;
        [SerializeField] private GameObject rockThree;
        [SerializeField] private GameObject rockFour;
        [SerializeField] private GameObject rockFive;
        [SerializeField] private GameObject rockSix;
        [SerializeField] private GameObject rockSeven;
        [Space]
        [SerializeField] private Transform[] points;
        [SerializeField] private Transform pointOne;
        [SerializeField] private Transform pointTwo;
        [SerializeField] private Transform pointThree;
        [SerializeField] private Transform pointFour;
        [SerializeField] private Transform pointFive;
        [SerializeField] private Transform pointSix;
        [Space]
        [SerializeField] private float moveDuration = 2f;
        //[SerializeField] private float rotationDuration = 0.5f;
        [SerializeField] private float pauseBetweenAnimations = 0.1f;
        [SerializeField]
        public AnimationCurve moveCurve = new AnimationCurve(
                new Keyframe(0f, 0f),
                new Keyframe(1f, 0f)); // Кривая для плавности прыжка

        [Space]
        //[SerializeField] private PrologScenario prologScenario;


        private Sequence _animationSequence;

        private Vector3[] _initialPosition;

        private Vector3 _initialPositionRockOne;
        private Vector3 _initialPositionRockTwo;
        private Vector3 _initialPositionRockThree;
        private Vector3 _initialPositionRockFour;
        private Vector3 _initialPositionRockFive;
        private Vector3 _initialPositionRockSix;


        private void Start()
        {
           // ValidateScripts();
            InitializeRocks();
        }

        private void InitializeRocks()
        {
            SaveStartPosition();
            SetRocksToBeginPoint();
            SetActiveRocks(false);
        }




        public void AnimationRocks()
        {
            //  StopAnimation();
            SetActiveRocks(true);

            _animationSequence = DOTween.Sequence();

            _animationSequence
                .AppendInterval(0.1f)
                .Append(rockOne.transform.DOMove(_initialPositionRockOne, moveDuration).SetEase(moveCurve))
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
            _initialPositionRockOne = rockOne.transform.position;
            //_initialPositionRockTwo = rockTwo.transform.position;
            //_initialPositionRockThree = rockThree.transform.position;
            //_initialPositionRockFour = rockFour.transform.position;
            //_initialPositionRockFive = rockFive.transform.position;
            //_initialPositionRockSix = rockSix.transform.position;
        }

        private void SetRocksToBeginPoint()
        {
            rockOne.transform.position = pointOne.position;
            //rockTwo.transform.position = pointTwo.position;
            //rockThree.transform.position = pointThree.position;
            //rockFour.transform.position = pointFour.position;
            //rockFive.transform.position = pointFive.position;
            //rockSix.transform.position = pointSix.position;
        }

        private void SetActiveRocks(bool isActive)
        {
            rockOne.gameObject.SetActive(isActive);
            //rockTwo.gameObject.SetActive(isActive);
            //rockThree.gameObject.SetActive(isActive);
            //rockFour.gameObject.SetActive(isActive);
            //rockFive.gameObject.SetActive(isActive);
            //rockSix.gameObject.SetActive(isActive);
        }

        private void ValidateScripts()
        {
            if (rockOne == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на rockOne!");
            if (rockTwo == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на rockTwo!");
            if (rockThree == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на rockThree!");
            if (rockFour == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на rockFour!");
            if (rockFive == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на rockFive!");
            if (rockSix == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на rockSix!");

            if (pointOne == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на pointOne!");
            if (pointTwo == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на pointTwo!");
            if (pointThree == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на pointThree!");
            if (pointFour == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на pointFour!");
            if (pointFive == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на pointFive!");
            if (pointSix == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на pointSix!");
        }
    }
}
