using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteAlways]
public class UIDissolveBurne : MonoBehaviour
{
    [Header("Base Settings")]
    public Material burnMaterial;
    [ColorUsage(true, true)]
    public Color baseColor = Color.white;
    [Range(0, 1)]
    public float alphaClip = 0.1f;

    [Header("Dissolve Settings")]
    [Range(0, 1)]
    public float dissolveAmount = 0.5f;
    public float dissolveWidth = 0.5f;
    [Tooltip("Current dissolve scale (automatically set during animation)")]
    public float dissolveScale = 50f;
    [ColorUsage(true, true)]
    public Color dissolveColor = new Color(4f, 1.78f, 0.77f, 1f);

    [Header("Animation Settings")]
    public float animationSpeed = 1f;



    [Tooltip("Scale value when burning out")]
    public float dissolveScaleOut = 50f;
    [Tooltip("Scale value when burning in")]
    public float dissolveScaleIn = 30f;
    public bool animateOnStart = false;
    public bool destroyOnComplete = false;

    [Header("Keyboard Controls")]
    public bool enableKeyboardControls = true;
    public KeyCode burnOutKey = KeyCode.Q;
    public KeyCode burnInKey = KeyCode.W;

    private Material _materialInstance;
    private Image _targetImage;

    // Property IDs
    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
    private static readonly int AlphaClipID = Shader.PropertyToID("_AlpfaClip");
    private static readonly int DissolveAmountID = Shader.PropertyToID("_DissolveAmount");
    private static readonly int DissolveWidthID = Shader.PropertyToID("_DissolveWidgh");
    private static readonly int DissolveScaleID = Shader.PropertyToID("_DissolveScale");
    private static readonly int DissolveColorID = Shader.PropertyToID("_DissolveColor");

    private void Awake()
    {
        _targetImage = GetComponent<Image>();
        InitializeMaterial();

        if (animateOnStart)
        {
            BurnOut();
        }
    }

    private void OnEnable()
    {
        InitializeMaterial();
        UpdateMaterialProperties();
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
            if (Input.GetKeyDown(burnOutKey))
            {
                BurnOut();
            }
            else if (Input.GetKeyDown(burnInKey))
            {
                BurnIn();
            }
        }
    }

    private void InitializeMaterial()
    {
        if (_targetImage == null) return;

        if (burnMaterial == null)
        {
            Debug.LogWarning("Burn material is not assigned!", this);
            return;
        }

        if (_materialInstance == null)
        {
            _materialInstance = new Material(burnMaterial)
            {
                name = $"{name}_BurnMaterial_Instance",
                hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor
            };
        }

        _targetImage.material = _materialInstance;
    }

    private void RestoreOriginalMaterial()
    {
        if (_targetImage != null)
        {
            _targetImage.material = null;
        }
    }

    private void UpdateMaterialProperties()
    {
        if (_materialInstance == null) return;

        _materialInstance.SetColor(BaseColorID, baseColor);
        _materialInstance.SetFloat(AlphaClipID, alphaClip);
        _materialInstance.SetFloat(DissolveAmountID, dissolveAmount);
        _materialInstance.SetFloat(DissolveWidthID, dissolveWidth);
        _materialInstance.SetFloat(DissolveScaleID, dissolveScale);
        _materialInstance.SetColor(DissolveColorID, dissolveColor);
    }

    private void OnValidate()
    {
        UpdateMaterialProperties();
    }

    public void BurnOut()
    {
        StopAllCoroutines();
        dissolveScale = dissolveScaleOut; // Устанавливаем масштаб для BurnOut
        UpdateMaterialProperties();
        StartCoroutine(AnimateBurn(1f, 0f));
    }

    public void BurnIn()
    {
        StopAllCoroutines();
        dissolveScale = dissolveScaleIn; // Устанавливаем масштаб для BurnIn
        UpdateMaterialProperties();
        StartCoroutine(AnimateBurn(0f, 1f));
    }

    private IEnumerator AnimateBurn(float startValue, float endValue)
    {
        dissolveAmount = startValue;
        UpdateMaterialProperties();

        float duration = Mathf.Abs(endValue - startValue) / animationSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            dissolveAmount = Mathf.Lerp(startValue, endValue, elapsed / duration);
            UpdateMaterialProperties();
            yield return null;
        }

        dissolveAmount = endValue;
        UpdateMaterialProperties();

        if (destroyOnComplete && endValue >= 1f)
        {
            if (Application.isPlaying)
                Destroy(gameObject);
            else
                DestroyImmediate(gameObject);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Create Burn Material")]
    private void CreateBurnMaterial()
    {
        var shader = Shader.Find("Shader Graphs/BurnePaper");
        if (shader == null)
        {
            Debug.LogError("BurnePaper shader not found!");
            return;
        }

        var material = new Material(shader);
        string path = UnityEditor.AssetDatabase.GenerateUniqueAssetPath("Assets/BurnMaterial.mat");
        UnityEditor.AssetDatabase.CreateAsset(material, path);
        UnityEditor.AssetDatabase.SaveAssets();
        burnMaterial = material;
    }
#endif
}