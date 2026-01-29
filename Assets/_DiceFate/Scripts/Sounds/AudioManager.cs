using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace LL.Sounds
{
    public class AudioManager : MonoBehaviour
    {
        [Header("________________Audio Sourse_______________________________")]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioSource masterSource;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;


        [Header("________________Audio Settings_____________________________")]
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Toggle musicToggle;
        [Space]
        [SerializeField] private bool musicEnabled = true; // Флаг включения музыки
        [SerializeField] private bool sfxEnabled = true;   // Флаг включения звуковых эффектов



        [Header("________________Audio Clip_________________________________")]
        [SerializeField] public AudioClip music1; // Ссылка на аудиофайл
        [SerializeField] public AudioClip burnClip; // Ссылка на аудиофайл


        //private void Awake()
        //{
        //    audioMixer =GameObject.FindGameObjectsWithTag("Audio").GetComponent<AudioManager>();
        //}


        private void Start()
        {

            PlayMusic(music1);


            //if (PlayerPrefs.HasKey("V_Music"))
            //{
            //    loadVolume();
            //}
            //else
            //{
            //    SetMusicVolume();

            //}
            SetMusicVolume();
            SetSFXVolume();

        }

        public void PlayMusic(AudioClip clip)
        {
            // Проверяем, назначен ли аудиоклип
            if (clip != null & musicEnabled)
            {
                musicSource.PlayOneShot(clip); // Проигрываем звук            
            }
        }


        //Установка громкости
        public void SetMasterVolume()
        {
            float vMaster = masterSlider.value;
            audioMixer.SetFloat("V_Master", Mathf.Log10(vMaster) * 20);
        }

        public void SetMusicVolume()
        {
            float vMusic = musicSlider.value;
            audioMixer.SetFloat("V_Music", Mathf.Log10(vMusic) * 20);

            // PlayerPrefs.SetFloat("V_Music", vMusic);
        }

        public void SetSFXVolume()
        {
            float vSFX = sfxSlider.value;
            audioMixer.SetFloat("V_SFX", Mathf.Log10(vSFX) * 20);
        }

        // Проигрывание 
        public void PlaySFX(AudioClip clip)
        {
            // Проверяем, назначен ли аудиоклип
            if (clip != null & sfxEnabled)
            {
                sfxSource.PlayOneShot(clip); // Проигрываем звук            
            }
        }


        //public void SetMusicToggle()
        //{
        //    musicToggle.inOn
        //}      


        //Сохранение
        //private void loadVolume()
        //{
        //    musicSlider.value = PlayerPrefs.GetFloat("V_Music");

        //    SetMusicVolume();
        //}

    }
}
