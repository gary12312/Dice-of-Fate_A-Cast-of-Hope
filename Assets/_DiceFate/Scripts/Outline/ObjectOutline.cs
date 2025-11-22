using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class ObjectOutline : MonoBehaviour
{
    private static HashSet<Mesh> registeredMeshes = new HashSet<Mesh>();

    public enum Mode
    {
        OutlineAll,
        OutlineVisible,
        OutlineHidden,
        OutlineAndSilhouette,
        SilhouetteOnly
    }

    public Mode OutlineMode
    {
        get { return outlineMode; }
        set
        {
            outlineMode = value;
            needsUpdate = true;
        }
    }

    public Color OutlineColor
    {
        get { return outlineColor; }
        set
        {
            outlineColor = value;
            needsUpdate = true;
        }
    }

    public float OutlineWidth
    {
        get { return outlineWidth; }
        set
        {
            outlineWidth = value;
            needsUpdate = true;
        }
    }

    [Serializable]
    private class ListVector3
    {
        public List<Vector3> data;
    }

    [SerializeField]
    private Mode outlineMode;

    [SerializeField]
    private Color outlineColor = Color.white;

    [SerializeField, Range(0f, 10f)]
    private float outlineWidth = 2f;

    [Header("Настройки обводки")]
    [SerializeField, Tooltip("Если true, то обводка включена по умолчанию при старте")]
    private bool isOutline = true;

    [SerializeField, Tooltip("Предварительные вычисления включены: Вычисления на вершинах выполняются в редакторе и сериализуются с объектом. "
    + "Предварительные вычисления отключены: Вычисления на вершинах выполняются во время выполнения в Awake(). Это может вызвать паузу для больших мешей.")]
    private bool precomputeOutline;

    [SerializeField, HideInInspector]
    private List<Mesh> bakeKeys = new List<Mesh>();

    [SerializeField, HideInInspector]
    private List<ListVector3> bakeValues = new List<ListVector3>();

    private Renderer[] renderers;
    private Material outlineMaskMaterial;
    private Material outlineFillMaterial;

    private bool needsUpdate;
    private bool isOutlineEnabled;
    private Color defaultOutlineColor;

    void Awake()
    {
        // Сохраняем начальный цвет обводки
        defaultOutlineColor = outlineColor;

        // Устанавливаем начальное состояние обводки
        isOutlineEnabled = isOutline;

        // Кэшируем рендереры
        renderers = GetComponentsInChildren<Renderer>();

        // Проверяем наличие рендереров
        if (renderers == null || renderers.Length == 0)
        {
            Debug.LogWarning("Не найдено рендереров для компонента ObjectOutline.", this);
            enabled = false;
            return;
        }

        // Загружаем материалы из Resources
        var maskMaterial = Resources.Load<Material>(@"Materials/OutlineMask");
        var fillMaterial = Resources.Load<Material>(@"Materials/OutlineFill");

        // Проверяем успешность загрузки материалов
        if (maskMaterial == null || fillMaterial == null)
        {
            Debug.LogError("Материалы обводки не найдены в папке Resources! Путь: Materials/OutlineMask и Materials/OutlineFill", this);
            enabled = false;
            return;
        }

        // Создаем экземпляры материалов обводки
        outlineMaskMaterial = Instantiate(maskMaterial);
        outlineFillMaterial = Instantiate(fillMaterial);

        outlineMaskMaterial.name = "OutlineMask (Instance)";
        outlineFillMaterial.name = "OutlineFill (Instance)";

        // Получаем или генерируем сглаженные нормали
        LoadSmoothNormals();

        // Применяем свойства материалов сразу
        needsUpdate = true;

        // Устанавливаем начальное состояние компонента
        enabled = isOutlineEnabled;
    }

    void Start()
    {
        // Принудительно обновляем состояние обводки при старте
        if (isOutlineEnabled)
        {
            OnEnable();
        }
        else
        {
            OnDisable();
        }
        needsUpdate = true;
    }

    void OnEnable()
    {
        if (!isOutlineEnabled) return;
        if (renderers == null) return;

        foreach (var renderer in renderers)
        {
            if (renderer == null) continue;

            // Добавляем шейдеры обводки
            var materials = renderer.sharedMaterials.ToList();

            // Проверяем, не добавлены ли уже материалы, чтобы избежать дублирования
            if (!materials.Contains(outlineMaskMaterial))
            {
                materials.Add(outlineMaskMaterial);
            }
            if (!materials.Contains(outlineFillMaterial))
            {
                materials.Add(outlineFillMaterial);
            }

            renderer.materials = materials.ToArray();
        }
    }

    void OnValidate()
    {
        // Обновляем свойства материалов
        needsUpdate = true;

        // Очищаем кэш когда предварительные вычисления отключены или кэш поврежден
        if (!precomputeOutline && bakeKeys.Count != 0 || bakeKeys.Count != bakeValues.Count)
        {
            bakeKeys.Clear();
            bakeValues.Clear();
        }

        // Генерируем сглаженные нормали когда предварительные вычисления включены
        if (precomputeOutline && bakeKeys.Count == 0)
        {
            Bake();
        }
    }

    void Update()
    {
        if (needsUpdate)
        {
            needsUpdate = false;
            UpdateMaterialProperties();
        }
    }

    void OnDisable()
    {
        if (renderers == null) return;

        foreach (var renderer in renderers)
        {
            if (renderer == null) continue;

            // Удаляем шейдеры обводки
            var materials = renderer.sharedMaterials.ToList();

            materials.Remove(outlineMaskMaterial);
            materials.Remove(outlineFillMaterial);

            renderer.materials = materials.ToArray();
        }
    }

    void OnDestroy()
    {
        // Уничтожаем экземпляры материалов
        if (outlineMaskMaterial != null)
        {
            Destroy(outlineMaskMaterial);
        }
        if (outlineFillMaterial != null)
        {
            Destroy(outlineFillMaterial);
        }
    }

    /// <summary>
    /// Включает обводку объекта
    /// </summary>
    public void EnableOutline()
    {
        if (isOutlineEnabled) return;

        isOutlineEnabled = true;
        enabled = true;
        OnEnable();
        needsUpdate = true;
    }

    /// <summary>
    /// Выключает обводку объекта
    /// </summary>
    public void DisableOutline()
    {
        if (!isOutlineEnabled) return;

        isOutlineEnabled = false;
        OnDisable();
        enabled = false;
    }

    /// <summary>
    /// Изменяет цвет обводки
    /// </summary>
    /// <param name="newColor">Новый цвет обводки</param>
    public void ChangeColorOutline(Color newColor)
    {
        outlineColor = newColor;
        needsUpdate = true;
    }

    /// <summary>
    /// Возвращает начальный цвет обводки, установленный в инспекторе
    /// </summary>
    /// <returns>Начальный цвет обводки</returns>
    public Color DefaultColorOutline()
    {
        return defaultOutlineColor;
    }

    /// <summary>
    /// Восстанавливает начальный цвет обводки
    /// </summary>
    public void ResetColorOutline()
    {
        outlineColor = defaultOutlineColor;
        needsUpdate = true;
    }

    void Bake()
    {
        // Генерируем сглаженные нормали для каждого меша
        var bakedMeshes = new HashSet<Mesh>();

        foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
        {
            if (meshFilter == null || meshFilter.sharedMesh == null) continue;

            // Пропускаем дубликаты
            if (!bakedMeshes.Add(meshFilter.sharedMesh))
            {
                continue;
            }

            // Сериализуем сглаженные нормали
            var smoothNormals = SmoothNormals(meshFilter.sharedMesh);

            bakeKeys.Add(meshFilter.sharedMesh);
            bakeValues.Add(new ListVector3() { data = smoothNormals });
        }
    }

    void LoadSmoothNormals()
    {
        // Получаем или генерируем сглаженные нормали
        foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
        {
            if (meshFilter == null || meshFilter.sharedMesh == null) continue;

            // Пропускаем если сглаженные нормали уже были применены
            if (!registeredMeshes.Add(meshFilter.sharedMesh))
            {
                continue;
            }

            // Получаем или генерируем сглаженные нормали
            var index = bakeKeys.IndexOf(meshFilter.sharedMesh);
            var smoothNormals = (index >= 0) ? bakeValues[index].data : SmoothNormals(meshFilter.sharedMesh);

            // Сохраняем сглаженные нормали в UV3
            meshFilter.sharedMesh.SetUVs(3, smoothNormals);

            // Объединяем подмеши
            var renderer = meshFilter.GetComponent<Renderer>();

            if (renderer != null)
            {
                CombineSubmeshes(meshFilter.sharedMesh, renderer.sharedMaterials);
            }
        }

        // Очищаем UV3 на SkinnedMeshRenderer
        foreach (var skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (skinnedMeshRenderer == null || skinnedMeshRenderer.sharedMesh == null) continue;

            // Пропускаем если UV3 уже были сброшены
            if (!registeredMeshes.Add(skinnedMeshRenderer.sharedMesh))
            {
                continue;
            }

            // Очищаем UV3
            skinnedMeshRenderer.sharedMesh.uv4 = new Vector2[skinnedMeshRenderer.sharedMesh.vertexCount];

            // Объединяем подмеши
            CombineSubmeshes(skinnedMeshRenderer.sharedMesh, skinnedMeshRenderer.sharedMaterials);
        }
    }

    List<Vector3> SmoothNormals(Mesh mesh)
    {
        if (mesh == null) return new List<Vector3>();

        // Группируем вершины по местоположению
        var groups = mesh.vertices.Select((vertex, index) => new KeyValuePair<Vector3, int>(vertex, index)).GroupBy(pair => pair.Key);

        // Копируем нормали в новый список
        var smoothNormals = new List<Vector3>(mesh.normals);

        // Усредняем нормали для сгруппированных вершин
        foreach (var group in groups)
        {
            // Пропускаем одиночные вершины
            if (group.Count() == 1)
            {
                continue;
            }

            // Вычисляем усредненную нормаль
            var smoothNormal = Vector3.zero;

            foreach (var pair in group)
            {
                smoothNormal += smoothNormals[pair.Value];
            }

            smoothNormal.Normalize();

            // Назначаем сглаженную нормаль каждой вершине
            foreach (var pair in group)
            {
                smoothNormals[pair.Value] = smoothNormal;
            }
        }

        return smoothNormals;
    }

    void CombineSubmeshes(Mesh mesh, Material[] materials)
    {
        if (mesh == null) return;

        // Пропускаем меши с одним подмешем
        if (mesh.subMeshCount == 1)
        {
            return;
        }

        // Пропускаем если количество подмешей превышает количество материалов
        if (mesh.subMeshCount > materials.Length)
        {
            return;
        }

        // Добавляем объединенный подмеш
        mesh.subMeshCount++;
        mesh.SetTriangles(mesh.triangles, mesh.subMeshCount - 1);
    }

    void UpdateMaterialProperties()
    {
        if (outlineFillMaterial == null || outlineMaskMaterial == null) return;

        // Применяем свойства в соответствии с режимом
        outlineFillMaterial.SetColor("_OutlineColor", outlineColor);

        switch (outlineMode)
        {
            case Mode.OutlineAll:
                outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
                break;

            case Mode.OutlineVisible:
                outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
                break;

            case Mode.OutlineHidden:
                outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
                outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
                break;

            case Mode.OutlineAndSilhouette:
                outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
                break;

            case Mode.SilhouetteOnly:
                outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
                outlineFillMaterial.SetFloat("_OutlineWidth", 0f);
                break;
        }
    }
}