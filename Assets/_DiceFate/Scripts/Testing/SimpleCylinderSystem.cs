using UnityEngine;
using System.Collections.Generic;

public class SimpleCylinderSystem : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject cylinderPrefab;

    [Header("Cylinder Parameters")]
    public float cylinderRadius = 3f;
    public float cylinderHeight = 4f;
    [Range(0f, 1f)]
    public float inwardOffset = 0.2f; // Смещение от края внутрь (в долях от радиуса)

    [Header("Movement Speeds")]
    public float cylinderFollowSpeed = 10f; // Скорость следования цилиндра за мышью
    public float cubesAttractionSpeed = 5f; // Скорость притягивания кубов к границе

    [Header("Detection Settings")]
    public string cubeTag = "Cube";
    public float detectionMultiplier = 1.5f; // Множитель для зоны обнаружения

    private GameObject currentCylinder;
    private List<GameObject> cubesInside = new List<GameObject>();

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CreateCylinder();
        }

        if (Input.GetMouseButton(0) && currentCylinder != null)
        {
            MoveCylinder();
            MoveCubesInside();
        }

        if (Input.GetMouseButtonUp(0))
        {
            DestroyCylinder();
        }
    }

    void CreateCylinder()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            currentCylinder = Instantiate(cylinderPrefab);
            currentCylinder.transform.position = hit.point + Vector3.up * cylinderHeight / 2;
            currentCylinder.transform.localScale = new Vector3(cylinderRadius * 2, cylinderHeight / 2, cylinderRadius * 2);

            // Собираем все кубы поблизости
            GameObject[] allCubes = GameObject.FindGameObjectsWithTag(cubeTag);
            cubesInside.Clear();

            foreach (GameObject cube in allCubes)
            {
                float distance = Vector3.Distance(
                    new Vector3(cube.transform.position.x, 0, cube.transform.position.z),
                    new Vector3(currentCylinder.transform.position.x, 0, currentCylinder.transform.position.z)
                );

                if (distance < cylinderRadius * detectionMultiplier)
                {
                    cubesInside.Add(cube);
                }
            }

            Debug.Log($"Cylinder created. Found {cubesInside.Count} cubes inside detection zone.");
        }
    }

    void MoveCylinder()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 targetPos = hit.point + Vector3.up * cylinderHeight / 2;
            currentCylinder.transform.position = Vector3.Lerp(
                currentCylinder.transform.position,
                targetPos,
                Time.deltaTime * cylinderFollowSpeed
            );
        }
    }

    void MoveCubesInside()
    {
        Vector3 cylinderCenter = currentCylinder.transform.position;
        float effectiveRadius = cylinderRadius * (1f - inwardOffset); // Учитываем смещение внутрь

        foreach (GameObject cube in cubesInside)
        {
            if (cube == null) continue;

            Vector3 toCube = cube.transform.position - cylinderCenter;
            Vector3 toCubeFlat = new Vector3(toCube.x, 0, toCube.z);
            float flatDistance = toCubeFlat.magnitude;

            // Кубы перемещаются ТОЛЬКО если они за пределами эффективного радиуса
            if (flatDistance > effectiveRadius)
            {
                // Плавно притягиваем куб к эффективной границе
                Vector3 targetPos = cylinderCenter + toCubeFlat.normalized * effectiveRadius;
                targetPos.y = cube.transform.position.y;

                cube.transform.position = Vector3.Lerp(
                    cube.transform.position,
                    targetPos,
                    Time.deltaTime * cubesAttractionSpeed
                );
            }
            // Кубы внутри эффективного радиуса НЕ перемещаются
        }
    }

    void DestroyCylinder()
    {
        if (currentCylinder != null)
        {
            Destroy(currentCylinder);
            cubesInside.Clear();
            Debug.Log("Cylinder destroyed.");
        }
    }

    // Метод для визуализации зон в редакторе (только в Play Mode)
    void OnDrawGizmosSelected()
    {
        if (currentCylinder != null && Application.isPlaying)
        {
            // Визуализация зон цилиндра
            Vector3 center = currentCylinder.transform.position;
            float effectiveRadius = cylinderRadius * (1f - inwardOffset);

            // Полный радиус цилиндра (желтый)
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(new Vector3(center.x, center.y - cylinderHeight / 4, center.z), cylinderRadius);
            Gizmos.DrawWireSphere(new Vector3(center.x, center.y + cylinderHeight / 4, center.z), cylinderRadius);

            // Эффективный радиус с учетом смещения (зеленый)
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(center, effectiveRadius);

            // Зона обнаружения (красный, полупрозрачный)
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawWireSphere(center, cylinderRadius * detectionMultiplier);
        }
    }
}