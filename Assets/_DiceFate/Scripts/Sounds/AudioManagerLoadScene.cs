using UnityEngine;

namespace LL.Sounds
{
    public class AudioManagerLoadScene : MonoBehaviour
    {
        [Header("________________Audio Manalger___________________________________________")]            
        [SerializeField] private AudioClip soundClip; // Ссылка на аудиофайл
        private AudioSource audioSource; // Компонент для воспроизведения звука

        [Header("________________Audio Play___________________________________________")]
        public float[] delayTimes = { 1f, 5f, 11f }; // Массив с временными задержками

        private float timer = 0f; // Таймер для отслеживания времени
        private int currentDelayIndex = 0; // Индекс текущей задержки

        void Start()
        {
            // Проверяем, есть ли компонент AudioSource на объекте
            audioSource = GetComponent<AudioSource>();

            // Назначаем аудиоклип
            audioSource.clip = soundClip;
        }


        void Update()
        {
            // Увеличиваем таймер на время между кадрами
            timer += Time.deltaTime;

            // Проверяем, не настало ли время проигрывать звук
            if (currentDelayIndex < delayTimes.Length && timer >= delayTimes[currentDelayIndex])
            {
                PlaySound(); // Проигрываем звук
                currentDelayIndex++; // Переходим к следующей задержке
            }
        }

        void PlaySound()
        {
            // Проверяем, назначен ли аудиоклип
            if (soundClip != null)
            {
                audioSource.Play(); // Проигрываем звук
            }
        }
    }
}

