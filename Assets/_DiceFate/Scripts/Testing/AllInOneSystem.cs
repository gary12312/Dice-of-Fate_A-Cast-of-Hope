using UnityEngine;
using System.Collections.Generic;

public class AllInOneSystem : MonoBehaviour
{
    [Header("Настройки")]
    public float cylinderRadius = 3f;
    public float cylinderHeight = 4f;
    public float moveSpeed = 10f;

    [Header("Цвета")]
    public Color normalColor = Color.white;
    public Color activeColor = Color.yellow;
    public Color cylinderColor = new Color(0, 1, 0, 0.3f);

    private GameObject cylinder;
    private List<Transform> cubes = new List<Transform>();
    private List<Material> cubeMaterials = new List<Material>();
    private bool isDragging = false;

    void Start()
    {
        // Находим все кубы
        GameObject[] cubeObjects = GameObject.FindGameObjectsWithTag("Cube");
        foreach (GameObject cube in cubeObjects)
        {
            cubes.Add(cube.transform);

            // Сохраняем материалы для смены цвета
            Renderer renderer = cube.GetComponent<Renderer>();
            if (renderer != null)
            {
                cubeMaterials.Add(renderer.material);
            }
            else
            {
                cubeMaterials.Add(null);
            }
        }

        Debug.Log("Найдено кубов: " + cubes.Count);
    }

    void Update()
    {
        // Нажатие ЛКМ
        if (Input.GetMouseButtonDown(0))
        {
            StartDragging();
        }

        // Перетаскивание
        if (isDragging && cylinder != null)
        {
            UpdateCylinderPosition();
            UpdateCubesPosition();
        }

        // Отпускание
        if (Input.GetMouseButtonUp(0))
        {
            StopDragging();
        }
    }

    void StartDragging()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Создаем цилиндр
            cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.GetComponent<Renderer>().material.color = cylinderColor;
            Destroy(cylinder.GetComponent<Collider>());

            // Позиционируем
            cylinder.transform.position = hit.point + Vector3.up * cylinderHeight / 2;
            cylinder.transform.localScale = new Vector3(cylinderRadius * 2, cylinderHeight / 2, cylinderRadius * 2);

            // Меняем цвет кубов
            for (int i = 0; i < cubeMaterials.Count; i++)
            {
                if (cubeMaterials[i] != null)
                {
                    cubeMaterials[i].color = activeColor;
                }
            }

            // Сразу перемещаем кубы к цилиндру
            PositionCubesAroundCylinder();

            isDragging = true;
        }
    }

    void PositionCubesAroundCylinder()
    {
        Vector3 center = cylinder.transform.position;

        for (int i = 0; i < cubes.Count; i++)
        {
            if (cubes[i] == null) continue;

            float angle = i * (360f / cubes.Count) * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(
                center.x + Mathf.Cos(angle) * cylinderRadius * 0.8f,
                cubes[i].position.y,
                center.z + Mathf.Sin(angle) * cylinderRadius * 0.8f
            );

            cubes[i].position = pos;
        }
    }

    void UpdateCylinderPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 targetPos = hit.point + Vector3.up * cylinderHeight / 2;
            cylinder.transform.position = Vector3.Lerp(cylinder.transform.position, targetPos, Time.deltaTime * 10f);
        }
    }

    void UpdateCubesPosition()
    {
        Vector3 center = cylinder.transform.position;

        for (int i = 0; i < cubes.Count; i++)
        {
            if (cubes[i] == null) continue;

            Vector3 toCube = cubes[i].position - center;
            Vector3 toCubeFlat = new Vector3(toCube.x, 0, toCube.z);
            float distance = toCubeFlat.magnitude;

            if (distance > cylinderRadius)
            {
                Vector3 targetPos = center + toCubeFlat.normalized * cylinderRadius;
                targetPos.y = cubes[i].position.y;

                cubes[i].position = Vector3.Lerp(cubes[i].position, targetPos, Time.deltaTime * moveSpeed);
            }
            else
            {
                // Двигаем вместе с цилиндром
                cubes[i].position += cylinder.transform.position - (center - Vector3.up * cylinderHeight / 2);
            }
        }
    }

    void StopDragging()
    {
        if (cylinder != null)
        {
            Destroy(cylinder);

            // Возвращаем цвет
            for (int i = 0; i < cubeMaterials.Count; i++)
            {
                if (cubeMaterials[i] != null)
                {
                    cubeMaterials[i].color = normalColor;
                }
            }
        }

        isDragging = false;
    }
}