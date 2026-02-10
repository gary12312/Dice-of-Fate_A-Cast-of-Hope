using UnityEngine;
using UnityEngine.SceneManagement;


namespace DiceFate
{
    public class SceneLoader : MonoBehaviour
    {

        // Статическая ссылка для доступа из любых скриптов
        public static SceneLoader Instance;

        // Названия сцен (задаются в инспекторе)
        [SerializeField] private string mainMenuScene = "MainMenu";
        [SerializeField] private string gameScene = "GameScene";
        [SerializeField] private string settingsScene = "Settings";

        [SerializeField] private GameObject ImageScreenLoad;
        [SerializeField] private float durationScreenloder = 0.5f;

        private void Awake()
        {
            // Реализуем паттерн Singleton
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Объект не уничтожается при загрузке новой сцены
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Методы для загрузки конкретных сцен
        public void LoadMainMenu()
        {
            SceneManager.LoadScene(mainMenuScene);
        }

        public void StartGame()
        {
            SceneManager.LoadScene(gameScene);
        }

        public void OpenSettings()
        {
            ImageLoder();
            SceneManager.LoadScene(settingsScene);
        }

        // Метод с анимацией перехода (если нужно)
        public void LoadSceneWithDelay(string sceneName, float delay = 0.5f)
        {
            StartCoroutine(LoadSceneDelayed(sceneName, delay));
        }

        private System.Collections.IEnumerator LoadSceneDelayed(string sceneName, float delay)
        {
            // Можно добавить анимацию затемнения

            yield return new WaitForSeconds(delay);
            SceneManager.LoadScene(sceneName);
        }

        //--------------------------------
        private void ImageLoder()
        {
            UIImageLoading uiImage = ImageScreenLoad.GetComponent<UIImageLoading>();

            uiImage.DOFateImageLoadToOne(durationScreenloder);
        }

    }
}
