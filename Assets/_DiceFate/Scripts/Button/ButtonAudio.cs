using UnityEngine;
using UnityEngine.EventSystems;

namespace DiceFate.Buttons
{
    public class ButtonAudio : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        [Header("Audio Settings")]
        public AudioSource audioSource; // Компонент AudioSource для воспроизведения звуков
        public AudioClip hoverSound;    // Звук при наведении на кнопку
        public AudioClip clickSound;   // Звук при нажатии на кнопку

        private void Awake()
        {
            if (audioSource == null)
            {
                Debug.LogWarning("Компонент AudioSource не назначен для " + this.gameObject.name);
                return;
            }

            // Настраиваем AudioSource
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }


        // Воспроизводит звук при наведении курсора на кнопку
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (hoverSound != null) audioSource.PlayOneShot(hoverSound);
        }

        // Воспроизводит звук при клике на кнопку
        public void OnPointerClick(PointerEventData eventData)
        {
            if (clickSound != null) audioSource.PlayOneShot(clickSound);
        }
    }
}
