using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace DiceFate.Buttons
{
    public class SliderValue : MonoBehaviour
    {
        public TextMeshProUGUI valueText;
        public Slider slider;

        private void Awake()
        {            
            //Debug
            if (slider == null)
            {
                Debug.LogWarning("Компонент slider не назначен для " + this.gameObject.name);
                return;
            }

            // Подписываемся на событие изменения значения
            slider.onValueChanged.AddListener(UpdateValueText);
        }

        void Start()
        {
            // Инициализируем текст текущим значением слайдера
            UpdateValueText(slider.value);
        }

        private void UpdateValueText(float value)
        {
            // Обновляем текст с текущим значением слайдера
            valueText.text = value.ToString("F1"); // F2 - формат с 2 знаками после запятой
        }
    }
}