using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

namespace DG_Tweening_Exemple
{
    public class DT_UI_testing : MonoBehaviour
    {

        public Button buttonW;
        public RectTransform point;
        public Transform pointPS;
        public ParticleSystem particleSystem;
        public float duration = 1f;
        public float durationSecond = 0.6f;

        //public GameObject targetObjectPS;

        [SerializeField] private UIParticalArrey UI_Particals;

        private Tween _tweenAnimation;

        public AnimationCurve animationCurve = new AnimationCurve(
          new Keyframe(0f, 0f),
          new Keyframe(1f, 1f)); // Кривая для плавности прыжка

        public AnimationCurve animationCurveSecond = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(1f, 1f)); // Кривая для плавности прыжка


        private Sequence _sequenceAnimation;
        private Vector2 _targetBodyPosition;
        private Vector2 _startShift;

        private Vector3 _startPoint;
        private Vector3 _scale;

        private void Awake()
        {
            ActiveParticleSystem(false, point);

            if (particleSystem != null)
            {
                particleSystem.Stop();
            }
        }


        void Start()
        {
            buttonW.onClick.AddListener(OnButtonClick);

            _targetBodyPosition = point.anchoredPosition;
            _startShift = new Vector2(buttonW.transform.position.x, buttonW.transform.position.y);
            _startPoint = buttonW.transform.position;
            _scale = buttonW.transform.localScale;
        }

        void OnButtonClick()
        {

            UI_Particals.ActivePartical(1, point);

            RectTransform btn = buttonW.GetComponent<RectTransform>();

            ActiveParticleSystem(true, btn);

            //btn.DOAnchorPos(_targetBodyPosition, duration).SetEase(animationCurve).Play();

            _sequenceAnimation = DOTween.Sequence();
            _sequenceAnimation
                .Append(btn.DOAnchorPos(point.anchoredPosition, duration).SetEase(animationCurve))
                .Join(btn.transform.DOScale(0, durationSecond).SetEase(animationCurveSecond))
                .OnComplete(() => ActiveParticleSystem(true, point))
                .Play();



            StartCoroutine(Wait(duration + 0.3f));

        }


        IEnumerator Wait(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            buttonW.transform.position = _startPoint;
            buttonW.transform.localScale = _scale;
        }

        private void ActiveParticleSystem(bool isActive, RectTransform rectTransform)
        {
            //targetObjectPS.transform.position = rectTransform.position;
            //targetObjectPS.SetActive(isActive);

        }

        // Метод для проигрывания партикла
        private void PlayParticleSystemQ()
        {
            if (particleSystem != null)
            {
                // Установка позиции партикла в позиции кнопки
                particleSystem.transform.position = new Vector3(0, 0, 0);

                // Воспроизведение партикла
                particleSystem.Play();

            }
            else
            {
                Debug.LogWarning("Particle System не назначен в инспекторе!");
            }
        }

        private void PlayParticleSystem()
        {
            if (particleSystem != null)
            {

                if (particleSystem != null)
                {
                    particleSystem.transform.position = point.position;

                    particleSystem.Play();
                }
            }
        }


    }
}
