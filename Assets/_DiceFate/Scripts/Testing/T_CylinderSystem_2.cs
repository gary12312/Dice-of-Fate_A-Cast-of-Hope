using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class T_CylinderSystem_2 : MonoBehaviour
{
    [Header("Cylinder Parameters")]
    public float cylinderRadius = 3f;
    public float cylinderHeight = 4f;
    [Range(0f, 1f)]
    public float inwardOffset = 0.2f; // Смещение от края внутрь
    public GameObject pointToCreate;

    [Header("Movement Speeds")]
    public float cylinderFollowSpeed = 10f;
    public float cubesAttractionSpeed = 5f;

    [Header("Cube Physics Settings")]
    public float minimumDistance = 1f; // Минимальное расстояние между кубами
    public float pushForce = 2f; // Сила отталкивания

    [Header("Visualization Colors")]
    public UnityEngine.Color cylinderColor = UnityEngine.Color.yellow;
    public UnityEngine.Color detectionColor = new UnityEngine.Color(1, 0, 0, 0.3f);
    public UnityEngine.Color effectiveRadiusColor = UnityEngine.Color.green;
    public UnityEngine.Color cubePersonalSpaceColor = UnityEngine.Color.cyan;

    [Header("Detection Settings")]
    public string cubeTag = "Cube";
    public float detectionMultiplier = 1.5f;

    private Vector3 cylinderCenter;
    private List<GameObject> cubesInside = new List<GameObject>();
    private Dictionary<GameObject, Vector3> cubePositions = new Dictionary<GameObject, Vector3>();
    private bool isCylinderActive = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CreateVirtualCylinder();
        }

        if (Input.GetMouseButton(0) && isCylinderActive)
        {
            MoveVirtualCylinder();   // перемещает центр цилиндра за курсором мыши
            MoveCubesInside();       // притягивает кубы к границе цилиндра (если они снаружи)
            ResolveCubeCollisions(); // предотвращает наложение кубов друг на друга
        }

        if (Input.GetMouseButtonUp(0))
        {
            DestroyVirtualCylinder();
        }
    }

    // Создает виртуальный цилиндр в точке клика
    void CreateVirtualCylinder()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {

            //cylinderCenter = hit.point + Vector3.up * cylinderHeight / 2;
            cylinderCenter = pointToCreate.transform.position;
            isCylinderActive = true;

            // Собираем все кубы поблизости
            GameObject[] allCubes = GameObject.FindGameObjectsWithTag(cubeTag);
            cubesInside.Clear();
            cubePositions.Clear();

            foreach (GameObject cube in allCubes)
            {
                float distance = Vector3.Distance(
                    new Vector3(cube.transform.position.x, 0, cube.transform.position.z),
                    new Vector3(cylinderCenter.x, 0, cylinderCenter.z)
                );

                if (distance < cylinderRadius * detectionMultiplier)
                {
                    cubesInside.Add(cube);
                    cubePositions[cube] = cube.transform.position;
                }
            }

            Debug.Log($"Cylinder created. Found {cubesInside.Count} cubes inside detection zone.");
        }
    }

    // Перемещаем центр цилиндра за курсором мыши
    void MoveVirtualCylinder()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //Vector3 targetPos = hit.point + pointToCreate.transform.position * cylinderHeight / 2;
            Vector3 targetPos = new Vector3(hit.point.x, pointToCreate.transform.position.y, hit.point.z);

            cylinderCenter = Vector3.Lerp(
                cylinderCenter,
                targetPos,
                Time.deltaTime * cylinderFollowSpeed
            );
        }

        //Vector3 transformMousePoint = new Vector3(point.Point.x, pointToCreate.transform.position.y, point.Point.z); 

        //cylinderCenter = Vector3.Lerp(cylinderCenter,
        //                                  transformMousePoint,
        //                                  cylinderFollowSpeed * Time.deltaTime);




    }

    // Притягиваем кубы к границе цилиндра(если они снаружи)
    void MoveCubesInside()
    {
        if (cubesInside.Count == 0) return;

        float effectiveRadius = cylinderRadius * (1f - inwardOffset);

        foreach (GameObject cube in cubesInside)
        {
            if (cube == null) continue;

            Vector3 toCube = cube.transform.position - cylinderCenter;
            Vector3 toCubeFlat = new Vector3(toCube.x, 0, toCube.z);
            float flatDistance = toCubeFlat.magnitude;

            if (flatDistance > effectiveRadius)
            {
                Vector3 targetPos = cylinderCenter + toCubeFlat.normalized * effectiveRadius;
                targetPos.y = cube.transform.position.y;

                cube.transform.position = Vector3.Lerp(
                    cube.transform.position,
                    targetPos,
                    Time.deltaTime * cubesAttractionSpeed
                );

                // Обновляем запись позиции
                cubePositions[cube] = cube.transform.position;
            }
        }
    }

    // предотвращает наложение кубов друг на друга
    void ResolveCubeCollisions()
    {
        if (cubesInside.Count < 2) return;

        float effectiveRadius = cylinderRadius * (1f - inwardOffset);

        for (int i = 0; i < cubesInside.Count; i++)
        {
            GameObject cubeA = cubesInside[i];
            if (cubeA == null) continue;

            for (int j = i + 1; j < cubesInside.Count; j++)
            {
                GameObject cubeB = cubesInside[j];
                if (cubeB == null) continue;

                Vector3 posA = cubeA.transform.position;
                Vector3 posB = cubeB.transform.position;

                Vector3 flatPosA = new Vector3(posA.x, 0, posA.z);
                Vector3 flatPosB = new Vector3(posB.x, 0, posB.z);

                Vector3 delta = flatPosB - flatPosA;
                float distance = delta.magnitude;

                if (distance < minimumDistance && distance > 0.01f)
                {
                    Vector3 direction = delta.normalized;
                    float overlap = minimumDistance - distance;
                    Vector3 separation = direction * (overlap * 0.5f * pushForce);

                    Vector3 newPosA = flatPosA - separation;
                    Vector3 newPosB = flatPosB + separation;

                    // Ограничиваем позиции внутри цилиндра
                    newPosA = ClampToCylinder(newPosA, effectiveRadius);
                    newPosB = ClampToCylinder(newPosB, effectiveRadius);

                    cubeA.transform.position = new Vector3(newPosA.x, posA.y, newPosA.z);
                    cubeB.transform.position = new Vector3(newPosB.x, posB.y, newPosB.z);

                    cubePositions[cubeA] = cubeA.transform.position;
                    cubePositions[cubeB] = cubeB.transform.position;
                }
            }
        }
    }

    // Ограничивает позицию внутри границ цилиндра
    Vector3 ClampToCylinder(Vector3 position, float radius)
    {
        Vector3 toPosition = position - new Vector3(cylinderCenter.x, 0, cylinderCenter.z);
        float distance = toPosition.magnitude;

        if (distance > radius)
        {
            return new Vector3(cylinderCenter.x, 0, cylinderCenter.z) + toPosition.normalized * radius;
        }

        return position;
    }

    // Очищаем состояние и уничтожает виртуальный цилиндр
    void DestroyVirtualCylinder()
    {
        isCylinderActive = false;
        cubesInside.Clear();
        cubePositions.Clear();
        Debug.Log("Cylinder destroyed.");
    }

    // визуализирует границы и зоны в редакторе Unity
    void OnDrawGizmos()
    {
        if (isCylinderActive)
        {
            // Визуализация полного радиуса цилиндра (верх и низ)
            Gizmos.color = cylinderColor;
            Vector3 topCenter = cylinderCenter + Vector3.up * cylinderHeight / 2;
            Vector3 bottomCenter = cylinderCenter - Vector3.up * cylinderHeight / 2;
            Gizmos.DrawWireSphere(topCenter, cylinderRadius);
            Gizmos.DrawWireSphere(bottomCenter, cylinderRadius);

            // Соединяем верх и низ цилиндра линиями
            int segments = 8;
            for (int i = 0; i < segments; i++)
            {
                float angle = i * (360f / segments);
                float x = Mathf.Sin(Mathf.Deg2Rad * angle) * cylinderRadius;
                float z = Mathf.Cos(Mathf.Deg2Rad * angle) * cylinderRadius;

                Vector3 topPoint = topCenter + new Vector3(x, 0, z);
                Vector3 bottomPoint = bottomCenter + new Vector3(x, 0, z);
                Gizmos.DrawLine(topPoint, bottomPoint);
            }

            // Визуализация зоны обнаружения (полупрозрачная)
            Gizmos.color = detectionColor;
            Gizmos.DrawWireSphere(cylinderCenter, cylinderRadius * detectionMultiplier);

            // Визуализация эффективного радиуса (зеленый)
            float effectiveRadius = cylinderRadius * (1f - inwardOffset);
            Gizmos.color = effectiveRadiusColor;

            // Рисуем круги на разных высотах для лучшей визуализации
            float[] heights = { -cylinderHeight / 2, 0, cylinderHeight / 2 };
            foreach (float height in heights)
            {
                Vector3 center = cylinderCenter + Vector3.up * height;
                DrawCircleGizmo(center, effectiveRadius, 36);
            }

            // Визуализация минимального расстояния между кубами
            Gizmos.color = cubePersonalSpaceColor;
            foreach (GameObject cube in cubesInside)
            {
                if (cube != null)
                {
                    Gizmos.DrawWireSphere(cube.transform.position, minimumDistance * 0.5f);
                }
            }
        }
    }

    //вспомогательный метод для рисования кругов
    void DrawCircleGizmo(Vector3 center, float radius, int segments)
    {
        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * (360f / segments);
            float angle2 = (i + 1) * (360f / segments);

            float x1 = Mathf.Sin(Mathf.Deg2Rad * angle1) * radius;
            float z1 = Mathf.Cos(Mathf.Deg2Rad * angle1) * radius;

            float x2 = Mathf.Sin(Mathf.Deg2Rad * angle2) * radius;
            float z2 = Mathf.Cos(Mathf.Deg2Rad * angle2) * radius;

            Gizmos.DrawLine(
                center + new Vector3(x1, 0, z1),
                center + new Vector3(x2, 0, z2)
            );
        }
    }
}