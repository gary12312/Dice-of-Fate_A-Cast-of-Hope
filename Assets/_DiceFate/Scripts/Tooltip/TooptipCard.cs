using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceFate.Tooltip
{
    public class TooptipCard : MonoBehaviour
    {
        [Header("Настройки анимации")]
        [SerializeField] private float duration = 1.5f;
        [SerializeField] private Vector3 targetPosition;
        [SerializeField]
        public AnimationCurve moveCurve = new AnimationCurve(
           new Keyframe(0f, 0f),
           new Keyframe(1f, 0f)); // Кривая для плавности 

        [Header("Настройки Цвета")]
        [SerializeField] private Color color;
        [Space]
        [SerializeField] private Image[] _images;
        [SerializeField] private TextMeshProUGUI[] _textMeshProUGUIs;
        [SerializeField] private Image imageCard;
        [SerializeField] private Image imageVizual;
        [SerializeField] private Image imageBorderText;
        [SerializeField] private Image imageMove;
        [SerializeField] private Image imageAttack;
        [SerializeField] private Image imageShild;
        [SerializeField] private Image imageConterAttac;
        [Space]
        [SerializeField] private Image imageOldGreed;
        [Space]
        [SerializeField] private TextMeshProUGUI textName;
        [SerializeField] private TextMeshProUGUI textLevel;
        [SerializeField] private TextMeshProUGUI textDescription;
        [SerializeField] private TextMeshProUGUI textMove;
        [SerializeField] private TextMeshProUGUI textAttack;
        [SerializeField] private TextMeshProUGUI textShild;
        [SerializeField] private TextMeshProUGUI textConterAttac;

        private Sequence _animationSequence;
        private Sequence _animationSequenceFade;
        private Vector3 _startPosition;
        private Color _defultColor;
        private Color _defultColorOldGreed;

        //private Image[] _images;
        //private TextMeshProUGUI[] _textMeshProUGUIs;

        private void Awake()
        {
            SetColorToImageAndText(color);
            _startPosition = transform.position;

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Debug.Log("fdfg");
                MoveTooltipOnDisable();
            }

        }

        private void Start()
        {
            ValidateScripts();
        }

        private void OnEnable()
        {
            MoveTooltipOnEnable();
        }

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
            //transform.position = _startPosition;

            _animationSequence = DOTween.Sequence();
            _animationSequence
               .Append(transform.DOMove(_startPosition, duration).SetEase(moveCurve))
               .AppendCallback(() => gameObject.SetActive(false))
               .Play();
        }



        private void DoFadeImageAndText()
        {
            _animationSequenceFade = DOTween.Sequence();
            _animationSequenceFade
               .Append(transform.DOMove(_startPosition, duration).SetEase(moveCurve))
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


        private void DefoultColorSave()
        {

        }


        private void SetColorToImageAndText(Color color)
        {

            if (_images.Length >= 0)
            {
                foreach (Image i in _images)
                {
                    i.color = color;
                }

            }


            foreach (TextMeshProUGUI t in _textMeshProUGUIs)
            {
                t.color = color;
            }

            //if (imageMove != null) imageMove.color = color;
            //if (imageShild != null) imageShild.color = color;
            //if (imageVizual != null) imageVizual.color = color;
            //if (imageCard != null) imageCard.color = color;
            //if (imageAttack != null) imageAttack.color = color;
            //if (imageBorderText != null) imageBorderText.color = color;
            //if (imageConterAttac != null) imageConterAttac.color = color;

            //if (textName != null) textName.color = color;
            //if (textMove != null) textMove.color = color;
            //if (textShild != null) textShild.color = color;
            //if (textLevel != null) textLevel.color = color;
            //if (textAttack != null) textAttack.color = color;
            //if (textConterAttac != null) textConterAttac.color = color;
            //if (textDescription != null) textDescription.color = color;
        }

        private void ValidateScripts()
        {
            if (imageMove == null) Debug.LogError($" для {this.name} Не установлена ссылка на imageMove!");
            if (imageShild == null) Debug.LogError($" для {this.name} Не установлена ссылка на imageShild!");
            if (imageCard == null) Debug.LogError($" для {this.name} Не установлена ссылка на imageBorder!");
            if (imageVizual == null) Debug.LogError($" для {this.name} Не установлена ссылка на imageVizual!");
            if (imageAttack == null) Debug.LogError($" для {this.name} Не установлена ссылка на imageAttack!");
            if (imageBorderText == null) Debug.LogError($" для {this.name} Не установлена ссылка на imageBorderText!");
            if (imageConterAttac == null) Debug.LogError($" для {this.name} Не установлена ссылка на imageConterAttac!");

            if (imageOldGreed == null) Debug.LogError($" для {this.name} Не установлена ссылка на imageOldGreed!");

            if (textName == null) Debug.LogError($" для {this.name} Не установлена ссылка на textName!");
            if (textMove == null) Debug.LogError($" для {this.name} Не установлена ссылка на textMove!");
            if (textAttack == null) Debug.LogError($" для {this.name} Не установлена ссылка на textAttack!");
            if (textShild == null) Debug.LogError($" для {this.name} Не установлена ссылка на textShild!");
            if (textConterAttac == null) Debug.LogError($" для {this.name} Не установлена ссылка на textConterAttac!");
            if (textDescription == null) Debug.LogError($" для {this.name} Не установлена ссылка на textDescription!");
        }







#if UNITY_EDITOR
        private void OnValidate()
        {
            SetColorToImageAndText(color);   // Вызывается при изменении значения в инспекторе
        }
#endif
    }
}