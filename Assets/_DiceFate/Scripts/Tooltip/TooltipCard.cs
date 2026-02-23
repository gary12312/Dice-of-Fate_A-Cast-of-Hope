using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceFate.Tooltip
{
    public class TooltipCard : MonoBehaviour
    {
        [Header("Настройки анимации")]
        [SerializeField] private float duration = 1.5f;
        [SerializeField] private Vector3 targetPosition;
        [SerializeField]
        public AnimationCurve moveCurve = new AnimationCurve(
           new Keyframe(0f, 0f),
           new Keyframe(1f, 0f)); // Кривая для плавности 
        [SerializeField]
        public AnimationCurve moveCurveHide = new AnimationCurve(
           new Keyframe(0f, 0f),
           new Keyframe(1f, 0f)); // Кривая для плавности 

        [Header("Настройки Цвета")]
        [SerializeField] private Color color;     
        [SerializeField] private Image[] _images;
        [SerializeField] private TextMeshProUGUI[] _textMeshProUGUIs;

        private Sequence _animationSequence;
        private Vector3 _startPosition;


        private void Awake()
        {
            SetColorToImageAndText(color);
            _startPosition = transform.position;

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z)) MoveTooltipOnDisable();           
        }


        private void OnEnable() => MoveTooltipOnEnable();       

        public void MoveTooltipOnEnable()
        {
            StopAnimation();
            transform.position = _startPosition;

            _animationSequence = DOTween.Sequence();
            _animationSequence
               .Append(transform.DOMove(transform.position + targetPosition, duration).SetEase(moveCurve))
               .Play();
        }
        public void MoveTooltipOnDisable()
        {
            StopAnimation();   

            _animationSequence = DOTween.Sequence();
            _animationSequence
               .Append(transform.DOMove(_startPosition, duration).SetEase(moveCurveHide))
               .AppendCallback(() => gameObject.SetActive(false))
               .Play();
        }


        public void StopAnimation()
        {
            if (_animationSequence != null)
            {
                _animationSequence.Kill();
                _animationSequence = null;
            }
        }


        private void SetColorToImageAndText(Color color)
        {

            if (_images != null)
            {
                foreach (Image img in _images)
                {
                    img.color = color;
                }
            }

            if (_textMeshProUGUIs != null)
            {
                foreach (TextMeshProUGUI text in _textMeshProUGUIs)
                {
                    text.color = color;
                }
            }
        }

       

#if UNITY_EDITOR
        private void OnValidate()
        {
            SetColorToImageAndText(color);   // Вызывается при изменении значения в инспекторе
        }
#endif
    }
}