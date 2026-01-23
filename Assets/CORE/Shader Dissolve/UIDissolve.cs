using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class UIDissolve : MonoBehaviour
{
    [Header("Dissolve Settings")]
    public Material dissolveMaterial; // Материал с эффектом dissolve
    [Range(0, 1)] public float edge = 0.34f; // Граница dissolve эффекта
    [Range(0, 0.5f)] public float edgeWidth = 0.05f; // Ширина границы эффекта
    [ColorUsage(true, true)] public Color edgeColor = new Color(0.09f, 4.3f, 0f); // Цвет границы
    public float scale = 20f; // Масштаб эффекта
    [SerializeField] private bool isAutoDissolve = true;

    [Header("Animation Settings")]
    public float animationSpeed = 0.25f; // Скорость анимации
    [Tooltip("Scale value when burning out")]
    public float dissolveScaleOut = 50f; // Масштаб при исчезновении
    [Tooltip("Scale value when burning in")]
    public float dissolveScaleIn = 30f; // Масштаб при появлении
    public float delayAnimationOut = 1f; // Задержка перед исчезновением

    [Header("Keyboard Controls")]
    public bool enableKeyboardControls = true; // Включить управление с клавиатуры
    public KeyCode dissolveInKey = KeyCode.Q; // Клавиша для появления
    public KeyCode dissolveOutKey = KeyCode.W; // Клавиша для исчезновения

    private Image _targetImage; // Компонент Image, к которому применяется эффект
    private Material _materialInstance; // Инстанс материала

    // Идентификаторы свойств шейдера
    private static readonly int Edge = Shader.PropertyToID("_Edge");
    private static readonly int EdgeWidth = Shader.PropertyToID("_EdgeWidth");
    private static readonly int EdgeColor = Shader.PropertyToID("_EdgeColor");
    private static readonly int Scale = Shader.PropertyToID("_Scale");


    ////Звуки
    //AudioManagerLoadScene audioManager;

    private void Awake()
    {
        //audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManagerLoadScene>();

        _targetImage = GetComponent<Image>();
        edge = 0;
        InitializeMaterial();
    }


    private void OnEnable()
    {
        if (_targetImage == null)
            _targetImage = GetComponent<Image>();

        ApplyMaterial();
        UpdateShaderProperties();

        // Запускаем автоматическую анимацию при включении
        if (isAutoDissolve)
        {            
            StartDissolve();
        }
    }

    private void OnDisable()
    {
        RestoreOriginalMaterial();
    }

    private void OnDestroy()
    {
        if (_materialInstance != null)
        {
            if (Application.isPlaying)
                Destroy(_materialInstance);
            else
                DestroyImmediate(_materialInstance);
        }
    }

    private void Update()
    {
        if (enableKeyboardControls)
        {
            if (Input.GetKeyDown(dissolveInKey)) DissolveIn();
            if (Input.GetKeyDown(dissolveOutKey)) DissolveOut();
        }

#if UNITY_EDITOR
        // Автоматическое применение материала в редакторе
        if (!Application.isPlaying && _targetImage.material != dissolveMaterial)
        {
            ApplyMaterial();
        }
#endif
    }


    private void OnValidate()
    {
        UpdateShaderProperties();
    }

    private void InitializeMaterial()
    {
        if (dissolveMaterial == null) return;

        // Создаем инстанс материала, чтобы не менять оригинальный asset
        _materialInstance = new Material(dissolveMaterial)
        {
            name = $"{name}_DissolveMaterial_Instance",
            hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor
        };
    }

    private void ApplyMaterial()
    {
        if (_targetImage == null) return;

        if (_materialInstance != null)
        {
            _targetImage.material = _materialInstance;
        }
        else if (dissolveMaterial != null)
        {
            _targetImage.material = dissolveMaterial;
        }
    }

    private void RestoreOriginalMaterial()
    {
        if (_targetImage != null)
        {
            _targetImage.material = null; // Восстанавливаем стандартный материал UI
        }
    }

    private void UpdateShaderProperties()
    {
        Material currentMaterial = _materialInstance != null ? _materialInstance : dissolveMaterial;

        if (currentMaterial == null || _targetImage == null) return;

        currentMaterial.SetFloat(Edge, edge);
        currentMaterial.SetFloat(EdgeWidth, edgeWidth);
        currentMaterial.SetColor(EdgeColor, edgeColor);
        currentMaterial.SetFloat(Scale, scale);
    }

    // Автоматическая анимация dissolve
    private void StartDissolve()
    {
        DissolveIn();
        StartCoroutine(AutoDissolveSequence());
    }

    private IEnumerator AutoDissolveSequence()
    {
        // Ждем указанную задержку
        yield return new WaitForSeconds(delayAnimationOut);

        DissolveOut();
    }

    public void DissolveIn()
    {
        StopAllCoroutines();
        scale = dissolveScaleIn; // Устанавливаем масштаб для появления
        StartCoroutine(AnimateDissolve(false));
        // audioManager.PlaySFX(audioManager.burn);
    }

    public void DissolveOut()
    {
        StopAllCoroutines();
        scale = dissolveScaleOut; // Устанавливаем масштаб для исчезновения
        StartCoroutine(AnimateDissolve(true));
        // audioManager.PlaySFX(audioManager.burn);
    }

    private IEnumerator AnimateDissolve(bool dissolveIn)
    {
        float targetValue = dissolveIn ? 0f : 1f;
        float currentSpeed = animationSpeed * (dissolveIn ? -1f : 1f);

        while ((dissolveIn && edge > 0) || (!dissolveIn && edge < 1))
        {
            edge = Mathf.Clamp(edge + Time.unscaledDeltaTime * currentSpeed, 0f, 1f);
            UpdateShaderProperties();
            yield return null;
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Create New Dissolve Material")]
    private void CreateNewMaterial()
    {
        var newMaterial = new Material(Shader.Find("Shader Graphs/UI Dissolve 2"));
        string path = UnityEditor.AssetDatabase.GenerateUniqueAssetPath($"Assets/UI_Dissolve_Material_{System.Guid.NewGuid().ToString("N").Substring(0, 8)}.mat");
        UnityEditor.AssetDatabase.CreateAsset(newMaterial, path);
        UnityEditor.AssetDatabase.SaveAssets();
        dissolveMaterial = newMaterial;
    }
#endif
}