using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Добавляем для работы со сценами

public class UI_Menu : MonoBehaviour
{
    [Header("Кнопки")]
    [SerializeField] private Button newGame;
    [SerializeField] private Button continueGame;
    [SerializeField] private Button settingsGame;
    [SerializeField] private Button back;
    [SerializeField] private TextMeshProUGUI tNewGame;
    [SerializeField] private TextMeshProUGUI tСontinueGame;
    [SerializeField] private TextMeshProUGUI tSettingsGame;

    [Header("Временные задержки")]
    [SerializeField] private float dilayOpenButton = 1f;
    [SerializeField] private float dilayAnimButton = 0.1f;

    [Space]
    [Header("Первый текст")]
    [SerializeField] private GameObject textFirst;
    [SerializeField] private CanvasGroup canvasGroupFirst;
    [SerializeField] private float durationFade = 1f;

    [Space]
    [Header("Диссолв эффект")]
    [SerializeField] private UIDissolve uIDissolve;

    [Space]
    [Header("Второй текст")]
    [SerializeField] private GameObject textSecond;

    [Space]
    [SerializeField] private GameObject unClicker;

    private Sequence sequenceAnimationNewGame;
    private Sequence sequenceAnimationСontinueGame;
    private Sequence sequenceAnimationSettingsGame;

    private bool isReloadEnabled = true; // Флаг для включения/отключения перезагрузки
    private bool canReloadScene = false; // Флаг, разрешающий перезагрузку

   
    private void Start()
    {
        ValidateScriptsAndObject();

        //tNewGame.alpha = 0f;
        //tСontinueGame.alpha = 0f;
        //tSettingsGame.alpha = 0f; 

        IsActiveButton(false); // Сначала скрываем кнопки

        canvasGroupFirst.alpha = 0;

        // Подписываемся на события кнопок
        newGame.onClick.AddListener(NewGame);
        continueGame.onClick.AddListener(ContinueGame);
        settingsGame.onClick.AddListener(SettingsGame);
        back.onClick.AddListener(Back);

        unClicker.SetActive(false);

        // Запускаем активацию UI элементов
        StartCoroutine(ActivationUIElements());
    }

    private void Update()
    {
        // Проверяем нажатие клавиши R для перезагрузки сцены
        if (canReloadScene && isReloadEnabled && Input.GetKeyDown(KeyCode.R))
        {
            ReloadCurrentScene();
        }
    }

    private IEnumerator ActivationUIElements()
    {
        // 1. Появление первого текста
        yield return StartCoroutine(FadeCanvasGroup(canvasGroupFirst, 0f, 1f, durationFade));

        // 2. Появление второго текста с эффектом диссолва
        if (uIDissolve != null)
        {
            uIDissolve.DissolveIn();
            yield return new WaitForSeconds(0.5f);
        }

        // 3. Появление кнопок меню
        yield return new WaitForSeconds(dilayOpenButton);
       // IsActiveButton(true);

        // Разрешаем перезагрузку сцены после появления меню
        canReloadScene = true;

        // Анимация появления кнопок
        yield return StartCoroutine(AnimateButtonsIn());
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }

    private IEnumerator AnimateButtonsIn()
    {
        newGame.gameObject.SetActive(true);
        tNewGame.alpha = 0f;
        ButtonAnimationInOut(tNewGame, ref sequenceAnimationNewGame, "In");
        yield return new WaitForSeconds(dilayAnimButton);

        continueGame.gameObject.SetActive(true);
        tСontinueGame.alpha = 0f;
        ButtonAnimationInOut(tСontinueGame, ref sequenceAnimationСontinueGame, "In");
        yield return new WaitForSeconds(dilayAnimButton);

        settingsGame.gameObject.SetActive(true);
        tSettingsGame.alpha = 0f;
        ButtonAnimationInOut(tSettingsGame, ref sequenceAnimationSettingsGame, "In");
    }



    private void IsActiveButton(bool isActive)
    {
        newGame.gameObject.SetActive(isActive);
        continueGame.gameObject.SetActive(isActive);
        settingsGame.gameObject.SetActive(isActive);
        back.gameObject.SetActive(false); // Кнопка "Назад" всегда скрыта в меню     
    }

    // Обработчики нажатий кнопок
    private void NewGame()
    {
        StartCoroutine(ButtonAnimationRun("Out", () =>
        {
            Debug.Log("Начата новая игра!");
            // Здесь добавьте логику запуска новой игры
        }));
    }

    private void ContinueGame()
    {
        StartCoroutine(ButtonAnimationRun("Out", () =>
        {
            Debug.Log("Продолжение игры!");
            // Здесь добавьте логику продолжения игры
        }));
    }

    private void SettingsGame()
    {
        StartCoroutine(ButtonAnimationRun("Out", () =>
        {
            Debug.Log("Открыты настройки!");
            // Здесь добавьте логику открытия настроек
            ShowBackButton();
        }));
    }

    private void Back()
    {
        // Анимация возврата кнопок
        StartCoroutine(AnimateButtonsIn());
        HideBackButton();
    }

    /// <summary>
    /// Перезагружает текущую сцену
    /// </summary>
    private void ReloadCurrentScene()
    {
        Debug.Log("Перезагрузка сцены по нажатию R...");

        // Временно блокируем перезагрузку
        isReloadEnabled = false;

        // Анимация перед перезагрузкой (опционально)
        StartCoroutine(ButtonAnimationRun("Out", () =>
        {
            // Получаем индекс текущей сцены
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            // Загружаем текущую сцену заново
            SceneManager.LoadScene(currentSceneIndex);
        }));
    }

    /// <summary>
    /// Включает или отключает возможность перезагрузки сцены
    /// </summary>
    public void SetReloadEnabled(bool enabled)
    {
        isReloadEnabled = enabled;
    }

    /// <summary>
    /// Включает возможность перезагрузки сцены
    /// </summary>
    public void EnableReload()
    {
        isReloadEnabled = true;
    }

    /// <summary>
    /// Отключает возможность перезагрузки сцены
    /// </summary>
    public void DisableReload()
    {
        isReloadEnabled = false;
    }

    private void ShowBackButton()
    {
        back.gameObject.SetActive(true);
        newGame.gameObject.SetActive(false);
        continueGame.gameObject.SetActive(false);
        settingsGame.gameObject.SetActive(false);
    }

    private void HideBackButton()
    {
        back.gameObject.SetActive(false);
        newGame.gameObject.SetActive(true);
        continueGame.gameObject.SetActive(true);
        settingsGame.gameObject.SetActive(true);
    }

    private IEnumerator ButtonAnimationRun(string direction, System.Action onComplete = null)
    {
        // Блокируем кнопки во время анимации
        unClicker.SetActive(true); // Включаем блокировку кликов

        ButtonAnimationInOut(tNewGame, ref sequenceAnimationNewGame, direction);
        yield return new WaitForSeconds(dilayAnimButton);

        ButtonAnimationInOut(tСontinueGame, ref sequenceAnimationСontinueGame, direction);
        yield return new WaitForSeconds(dilayAnimButton);

        ButtonAnimationInOut(tSettingsGame, ref sequenceAnimationSettingsGame, direction);
        yield return new WaitForSeconds(0.5f); // Ждем завершения анимации

        // Вызываем колбэк после завершения анимации
        onComplete?.Invoke();

        // Разблокируем кнопки (если не переходим на другую сцену)
        unClicker.SetActive(false);
    }

    private void ButtonAnimationInOut(TextMeshProUGUI textUI, ref Sequence sequenceAnimation, string direction)
    {
        // Останавливаем предыдущую анимацию
        sequenceAnimation?.Kill();

        // Создаем новую последовательность
        sequenceAnimation = DOTween.Sequence();

        // Сбрасываем начальное состояние
        textUI.alpha = (direction == "Out") ? 1f : 0f;
        var startPos = textUI.transform.position;

        switch (direction)
        {
            case "Out":
                // Анимация исчезновения
                sequenceAnimation
                    .Append(textUI.DOFade(0, 0.2f))
                    .Join(textUI.transform.DOMoveX(startPos.x + 300, 0.3f))
                    .AppendCallback(() =>
                    {
                        // Возвращаем на начальную позицию (невидимо)
                        textUI.transform.position = new Vector3(startPos.x - 300, startPos.y, startPos.z);
                    })
                    .Append(textUI.transform.DOMoveX(startPos.x, 0.3f))
                    .SetAutoKill(true);
                break;

            case "In":
                // Анимация появления
                textUI.transform.position = new Vector3(startPos.x - 300, startPos.y, startPos.z);
                sequenceAnimation
                    .Append(textUI.transform.DOMoveX(startPos.x, 0.3f))
                    .Join(textUI.DOFade(1, 0.2f))
                    .SetAutoKill(true);
                break;
        }

        sequenceAnimation.Play();
    }

    private void SetButtonsInteractable(bool isInteractable)
    {
        newGame.interactable = isInteractable;
        continueGame.interactable = isInteractable;
        settingsGame.interactable = isInteractable;
        back.interactable = isInteractable;
    }

    private void ValidateScriptsAndObject()
    {
        if (newGame == null)
            Debug.LogError($"Для {this.name} не установлена ссылка на newGame!");
        if (continueGame == null)
            Debug.LogError($"Для {this.name} не установлена ссылка на continueGame!");
        if (settingsGame == null)
            Debug.LogError($"Для {this.name} не установлена ссылка на settingsGame!");
        if (textFirst == null)
            Debug.LogError($"Для {this.name} не установлена ссылка на textFirst!");
        if (textSecond == null)
            Debug.LogError($"Для {this.name} не установлена ссылка на textSecond!");
        if (canvasGroupFirst == null)
            Debug.LogError($"Для {this.name} не установлена ссылка на canvasGroupFirst!");
    }

    private void OnDestroy()
    {
        // Очищаем анимации при уничтожении объекта
        sequenceAnimationNewGame?.Kill();
        sequenceAnimationСontinueGame?.Kill();
        sequenceAnimationSettingsGame?.Kill();

        // Отписываемся от событий
        newGame.onClick.RemoveListener(NewGame);
        continueGame.onClick.RemoveListener(ContinueGame);
        settingsGame.onClick.RemoveListener(SettingsGame);
        back.onClick.RemoveListener(Back);
    }
}