using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    [SerializeField] private GameObject settings;
    [SerializeField] private CanvasGroup settingsCanvasGroup;

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
    private Sequence sequenceAnimationSettings;

    private bool isReloadEnabled = true;
    private bool canReloadScene = false;
    private Vector3 settingsInitialPosition;
    private bool isSettingsActive = false;

    private void Start()
    {
        ValidateScriptsAndObject();

        // Сохраняем начальную позицию настроек
        settingsInitialPosition = settings.transform.position;

        // Скрываем все элементы UI в начале
        IsHideButtonOnStart();
        canvasGroupFirst.alpha = 0;

        // Настройка настроек
        settingsCanvasGroup.alpha = 0;
        settings.SetActive(false);

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

    private void IsHideButtonOnStart() => IsActiveObject(false, false, false, false);
    private void IsActiveSettings() => IsActiveObject(false, false, false, true);
    private void IsActiveManeMenuButton() => IsActiveObject(true, true, true, false);

    private void IsActiveObject(bool isNewGame, bool isContinueGame, bool isSettingsGame, bool issettings)
    {
        newGame.gameObject.SetActive(isNewGame);
        continueGame.gameObject.SetActive(isContinueGame);
        settingsGame.gameObject.SetActive(isSettingsGame);
        //back.gameObject.SetActive(isback);
        settings.SetActive(issettings);
    }

    // -------------------------  Обработчики нажатий кнопок ----------------------------------
    private void NewGame()
    {
        StartCoroutine(ButtonAnimationRun("Out", () =>
        {
            Debug.Log("Начата новая игра!");           
        }));
    }

    private void ContinueGame()
    {
        StartCoroutine(ButtonAnimationRun("Out", () =>
        {
            Debug.Log("Продолжение игры!");           
        }));
    }

    private void SettingsGame()
    {
        StartCoroutine(ButtonAnimationRun("Out", () => ShowSettingsPanel()));
    }

    private void Back()
    {
        if (isSettingsActive)
        {
            HideSettingsPanel();
        }
        else
        {
            // Если нужно вернуться к меню
            StartCoroutine(AnimateButtonsIn());
            IsActiveManeMenuButton();
        }
    }

    // ----------------------------- Анимации -----------------------------
    private void ShowSettingsPanel()
    {
        isSettingsActive = true;

        IsActiveSettings();     

        settingsCanvasGroup.alpha = 0;

        // Начальная позиция за пределами экрана (слева)
        Vector3 startPos = settings.transform.position;
        Vector3 targetPos = startPos;
        startPos.x -= 300; // Смещение справа

        settings.transform.position = startPos;

        // Анимация
        Sequence settingsSequence = DOTween.Sequence();
        settingsSequence
            .Append(settings.transform.DOMoveX(targetPos.x, 0.3f))
            .Join(settingsCanvasGroup.DOFade(1, 0.2f))
            .OnComplete(() =>
            {
                unClicker.SetActive(false); // Разблокируем клики
            })
            .Play();
    }

    private void HideSettingsPanel()
    {
        unClicker.SetActive(true); // Блокируем клики во время анимации

        // Анимация скрытия панели настроек
        Vector3 startPos = settings.transform.position;
        Vector3 targetPos = startPos;
        targetPos.x += 300; // Смещение вправо для скрытия

        Sequence settingsSequence = DOTween.Sequence();
        settingsSequence
            .Append(settings.transform.DOMoveX(targetPos.x, 0.3f)) 
            .Join(settingsCanvasGroup.DOFade(0, 0.2f))
            .OnComplete(() =>
            {
                // Скрываем панель настроек
                settings.SetActive(false);
                settings.transform.position = settingsInitialPosition; // Возвращаем на начальную позицию

                // Показываем кнопки меню с анимацией
                ShowMenuButtons();
                isSettingsActive = false;
            })
            .Play();
    }

    private void ShowMenuButtons()
    {  
        IsActiveManeMenuButton();       
        StartCoroutine(AnimateButtonsIn());

        unClicker.SetActive(false);
    }    

    private IEnumerator ButtonAnimationRun(string direction, System.Action onComplete = null)
    {
        unClicker.SetActive(true);

        ButtonAnimationInOut(tNewGame, ref sequenceAnimationNewGame, direction);
        yield return new WaitForSeconds(dilayAnimButton);

        ButtonAnimationInOut(tСontinueGame, ref sequenceAnimationСontinueGame, direction);
        yield return new WaitForSeconds(dilayAnimButton);

        ButtonAnimationInOut(tSettingsGame, ref sequenceAnimationSettingsGame, direction);
        yield return new WaitForSeconds(0.2f);

        onComplete?.Invoke();

        // Разблокируем только если не открыты настройки
        if (!isSettingsActive)
        {
            unClicker.SetActive(false);
        }
    }

    private void ButtonAnimationInOut(TextMeshProUGUI textUI, ref Sequence sequenceAnimation, string direction)
    {
        sequenceAnimation?.Kill();
        sequenceAnimation = DOTween.Sequence();

        textUI.alpha = (direction == "Out") ? 1f : 0f;
        var startPos = textUI.transform.position;

        switch (direction)
        {
            case "Out":
                sequenceAnimation
                    .Append(textUI.DOFade(0, 0.2f))
                    .Join(textUI.transform.DOMoveX(startPos.x + 300, 0.3f))
                    .AppendCallback(() =>
                    {
                        textUI.transform.position = new Vector3(startPos.x - 300, startPos.y, startPos.z);
                        textUI.gameObject.SetActive(false); // Скрываем кнопки после анимации
                    })
                    .Append(textUI.transform.DOMoveX(startPos.x, 0.3f))
                    .SetAutoKill(true);
                break;

            case "In":
                textUI.gameObject.SetActive(true); // Активируем кнопку
                textUI.transform.position = new Vector3(startPos.x - 300, startPos.y, startPos.z);
                sequenceAnimation
                    .Append(textUI.transform.DOMoveX(startPos.x, 0.3f))
                    .Join(textUI.DOFade(1, 0.2f))
                    .SetAutoKill(true);
                break;
        }

        sequenceAnimation.Play();
    }

        // Перезагружает текущую сцену
    private void ReloadCurrentScene()
    {
        Debug.Log("Тестирование Перезагрузка сцены по нажатию R ");

        isReloadEnabled = false;

        StartCoroutine(ButtonAnimationRun("Out", () =>
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }));
    }    
    public void SetReloadEnabled(bool enabled) => isReloadEnabled = enabled;  // Включает или отключает возможность перезагрузки сцены   
    public void EnableReload() => isReloadEnabled = true;                     // Включает  возможность перезагрузки сцены
    public void DisableReload() => isReloadEnabled = false;                   // Отключает возможность перезагрузки сцены

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
        if (settings == null)
            Debug.LogError($"Для {this.name} не установлена ссылка на settings!");
        if (settingsCanvasGroup == null && settings.GetComponent<CanvasGroup>() == null)
            Debug.LogWarning($"Для {this.name} не установлена ссылка на settingsCanvasGroup! Пытаемся получить компонент автоматически.");  
    }

    private void OnDestroy()
    {
        sequenceAnimationNewGame?.Kill();
        sequenceAnimationСontinueGame?.Kill();
        sequenceAnimationSettingsGame?.Kill();
        sequenceAnimationSettings?.Kill();

        newGame.onClick.RemoveListener(NewGame);
        continueGame.onClick.RemoveListener(ContinueGame);
        settingsGame.onClick.RemoveListener(SettingsGame);
        back.onClick.RemoveListener(Back);
    }
} 