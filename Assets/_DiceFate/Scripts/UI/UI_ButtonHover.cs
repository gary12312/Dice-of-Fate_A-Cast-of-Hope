using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


namespace DiceFate.UI
{
    public class UI_ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TextMeshProUGUI buttonText; // Ссылка на компонент текста
        [SerializeField] private Color normalColor = Color.white; // Обычный цвет текста
        [SerializeField] private Color hoverColor = Color.red; // Цвет текста при наведении
               
        private void Start()
        {
            ChangedColorTextButton(normalColor);
            
        }

        // Вызывается, когда курсор наводится на кнопку
        public void OnPointerEnter(PointerEventData eventData)
        {
            ChangedColorTextButton(hoverColor);
            
        }

        // Вызывается, когда курсор уходит с кнопки
        public void OnPointerExit(PointerEventData eventData)
        {
            ChangedColorTextButton(normalColor);

        }

        // Дополнительный метод: изменение цвета при нажатии
        public void OnButtonPressed()
        {
           
        }

        // Метод для возврата цвета после нажатия
        public void OnButtonReleased()
        {

        }

        private void ChangedColorTextButton(Color color)
        {
            if (buttonText != null) buttonText.color = color;
        }



    }
}
