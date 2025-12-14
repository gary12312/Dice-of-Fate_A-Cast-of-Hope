using UnityEngine;

public class T_MouseManager : MonoBehaviour
{
    [Header("Настройки мыши")]
    [SerializeField] private LayerMask groundLayer; // Слой, по которому можно кликать
    [SerializeField] private float raycastDistance = 100f; // Дальность луча
    [SerializeField] private GameObject cylinderPrefab; // Префаб цилиндра

    [Header("Настройки цилиндра")]
    [SerializeField] private float cylinderHeight = 3f; // Высота цилиндра
    [SerializeField] private float cylinderRadius = 2f; // Радиус цилиндра

    // Ссылка на созданный цилиндр
    private GameObject currentCylinder;
    private bool isLeftMouseButtonDown = false;

    void Update()
    {
        HandleMouseInput();

        // Если цилиндр существует и ЛКМ нажата - обновляем его позицию
        if (isLeftMouseButtonDown && currentCylinder != null)
        {
            UpdateCylinderPosition();
        }
    }

    private void HandleMouseInput()
    {
        // Нажатие ЛКМ - создаем или активируем цилиндр
        if (Input.GetMouseButtonDown(0))
        {
            isLeftMouseButtonDown = true;
            CreateOrUpdateCylinder();
        }

        // Отпускание ЛКМ - удаляем цилиндр
        if (Input.GetMouseButtonUp(0))
        {
            isLeftMouseButtonDown = false;
            DestroyCylinder();
        }
    }

    private void CreateOrUpdateCylinder()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance, groundLayer))
        {
            // Если цилиндр уже существует - просто обновляем позицию
            if (currentCylinder != null)
            {
                currentCylinder.transform.position = hit.point + Vector3.up * cylinderHeight / 2;
            }
            else // Иначе создаем новый
            {
                CreateCylinder(hit.point);
            }
        }
    }

    private void CreateCylinder(Vector3 position)
    {
        if (cylinderPrefab != null)
        {
            // Создаем цилиндр из префаба
            currentCylinder = Instantiate(cylinderPrefab);
        }
        else
        {
            // Если нет префаба - создаем примитив
            currentCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            currentCylinder.name = "MouseCylinder";

            // Настраиваем материал (полупрозрачный для визуализации)
            ConfigureCylinderMaterial();

            // Удаляем стандартный коллайдер
            Destroy(currentCylinder.GetComponent<Collider>());
        }

        // Настраиваем размеры и позицию цилиндра
        currentCylinder.transform.position = position + Vector3.up * cylinderHeight / 2;
        currentCylinder.transform.localScale = new Vector3(cylinderRadius * 2, cylinderHeight / 2, cylinderRadius * 2);

        // Добавляем наш специальный коллайдер
        CylinderCollider cylinderCollider = currentCylinder.AddComponent<CylinderCollider>();
        cylinderCollider.SetRadius(cylinderRadius);
        cylinderCollider.SetHeight(cylinderHeight);
    }

    private void ConfigureCylinderMaterial()
    {
        Renderer renderer = currentCylinder.GetComponent<Renderer>();
        if (renderer != null)
        {
            // Создаем новый материал с прозрачностью
            Material material = new Material(Shader.Find("Standard"));
            material.color = new Color(0, 1, 0, 0.3f); // Зеленый полупрозрачный

            // Настраиваем прозрачность
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;

            renderer.material = material;
        }
    }

    private void UpdateCylinderPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance, groundLayer))
        {
            if (currentCylinder != null)
            {
                // Плавное перемещение цилиндра
                Vector3 newPosition = hit.point + Vector3.up * cylinderHeight / 2;
                currentCylinder.transform.position = Vector3.Lerp(
                    currentCylinder.transform.position,
                    newPosition,
                    Time.deltaTime * 10f
                );
            }
        }
    }

    private void DestroyCylinder()
    {
        if (currentCylinder != null)
        {
            Destroy(currentCylinder);
            currentCylinder = null;
        }
    }

    // Получение текущего цилиндра (для кубов)
    public GameObject GetCurrentCylinder()
    {
        return currentCylinder;
    }

    // Получение параметров цилиндра
    public float GetCylinderRadius()
    {
        return cylinderRadius;
    }

    public float GetCylinderHeight()
    {
        return cylinderHeight;
    }

    // При уничтожении объекта
    void OnDestroy()
    {
        DestroyCylinder();
    }
}