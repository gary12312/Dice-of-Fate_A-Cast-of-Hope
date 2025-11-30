using UnityEngine;

public class MouseTargerOutliner : MonoBehaviour
{
    [SerializeField] private float updateInterval = 0.05f;

    private ObjectOutline objectOutline;
    private Camera mainCamera;
    private float accumulatedTime;

    void Start()
    {
        objectOutline = GetComponent<ObjectOutline>();
        mainCamera = Camera.main;
        objectOutline?.DisableOutline();
    }

    void Update()
    {
        accumulatedTime += Time.deltaTime;

        if (accumulatedTime >= updateInterval)
        {
            UpdateFPS();
            accumulatedTime = 0;
        }
    }

    private void UpdateFPS()
    {
        if (!IsMouseOverThisObject())
        {
            objectOutline?.DisableOutline();
            return;
        }

        objectOutline?.EnableOutline();
    }

    
    private bool IsMouseOverThisObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out RaycastHit hit, 100) && hit.collider.gameObject == gameObject;
    }
}