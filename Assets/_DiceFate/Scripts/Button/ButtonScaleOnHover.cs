using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DiceFate.Buttons
{
    public class ButtonScaleOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {

        [Header("Настройки масштаба")]
        [SerializeField] private float hoverScale = 1.1f;
        [SerializeField] private float clickScale = 0.9f;
        [SerializeField] private float scaleSpeed = 5f;


        [Header("Настройки Цвета")]
        [SerializeField] private Color colorFerst;
        [SerializeField] private Color colorSecond;
        [SerializeField] private bool isChangeFerstColor;
        [SerializeField] private GameObject oldGreed;
        [SerializeField] private bool isChangeOldGreed;
        [Space]
        [SerializeField] private Image imageBorder;
        [SerializeField] private Image imageVizual;
        [SerializeField] private Image imageEmaki;
        [SerializeField] private Image imageBorderText;
        [SerializeField] private Image imageMove;
        [SerializeField] private Image imageAttack;
        [SerializeField] private Image imageShild;
        [SerializeField] private Image imageConterAttac;
        [Space]
        [SerializeField] private TextMeshProUGUI textName;
        [SerializeField] private TextMeshProUGUI textLevel;
        [SerializeField] private TextMeshProUGUI textDescription;
        [SerializeField] private TextMeshProUGUI textMove;
        [SerializeField] private TextMeshProUGUI textAttack;
        [SerializeField] private TextMeshProUGUI textShild;
        [SerializeField] private TextMeshProUGUI textConterAttac;


        private Vector3 originalScale;
        private Vector3 targetScale;
        private Color originalColor;

        private void Awake()
        {
            originalScale = transform.localScale;
            targetScale = originalScale;

            if (!isChangeFerstColor)
            {
                originalColor = imageBorder.color;
            }
            else
            {
                SetColorToImageAndText(colorFerst);
            }


        }

        private void Start()
        {
            ValidateScripts();
        }

        private void OnEnable()
        {
            originalScale = transform.localScale;
            targetScale = originalScale;
        }

        private void Update()
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            targetScale = originalScale * hoverScale;
            SetColorToImageAndText(colorSecond);

            if (isChangeOldGreed) oldGreed.SetActive(false);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            targetScale = originalScale;
            if (isChangeOldGreed) oldGreed.SetActive(true);

            if (!isChangeFerstColor)
            {
                originalColor = imageBorder.color;
            }
            else
            {
                SetColorToImageAndText(colorFerst);
            }
        }

        public void OnPointerDown(PointerEventData eventData) => targetScale = originalScale * clickScale;
        public void OnPointerUp(PointerEventData eventData) => targetScale = originalScale;

        private void SetColorToImageAndText(Color color)
        {
            imageMove.color = color;
            imageShild.color = color;
            imageEmaki.color = color;
            imageVizual.color = color;
            imageBorder.color = color;
            imageAttack.color = color;
            imageBorderText.color = color;
            imageConterAttac.color = color;

            textName.color = color;
            textMove.color = color;
            textShild.color = color;
            textLevel.color = color;
            textAttack.color = color;
            textConterAttac.color = color;
            textDescription.color = color;

        }

        private void ValidateScripts()
        {
            if (imageMove == null) Debug.LogError($" для {this.name} Не установлена ссылка на imageMove!");
            if (imageShild == null) Debug.LogError($" для {this.name} Не установлена ссылка на imageShild!");
            if (imageEmaki == null) Debug.LogError($" для {this.name} Не установлена ссылка на imageEmaki!");
            if (imageBorder == null) Debug.LogError($" для {this.name} Не установлена ссылка на imageBorder!");
            if (imageVizual == null) Debug.LogError($" для {this.name} Не установлена ссылка на imageVizual!");
            if (imageAttack == null) Debug.LogError($" для {this.name} Не установлена ссылка на imageAttack!");
            if (imageBorderText == null) Debug.LogError($" для {this.name} Не установлена ссылка на imageBorderText!");
            if (imageConterAttac == null) Debug.LogError($" для {this.name} Не установлена ссылка на imageConterAttac!");

            if (textName == null) Debug.LogError($" для {this.name} Не установлена ссылка на textName!");
            if (textMove == null) Debug.LogError($" для {this.name} Не установлена ссылка на textMove!");
            if (textAttack == null) Debug.LogError($" для {this.name} Не установлена ссылка на textAttack!");
            if (textShild == null) Debug.LogError($" для {this.name} Не установлена ссылка на textShild!");
            if (textConterAttac == null) Debug.LogError($" для {this.name} Не установлена ссылка на textConterAttac!");
            if (textDescription == null) Debug.LogError($" для {this.name} Не установлена ссылка на textDescription!");

            if (oldGreed == null) Debug.LogError($" для {this.name} Не установлена ссылка на oldGreed!");
        }

    }
}
