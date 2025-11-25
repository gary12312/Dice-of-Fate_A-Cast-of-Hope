using UnityEngine;
using System.Collections;

public class GlassController : MonoBehaviour
{
    [Header("Glass Settings")]
    public float movementSpeed = 2f;
    public float rotationSpeed = 100f;
    public float maxTiltAngle = 45f;

    [Header("Dice Settings")]
    public GameObject dicePrefab;
    public int diceCount = 3;
    public float diceScale = 1f;
    public Transform diceSpawnPoint;

    [Header("Throw Settings")]
    public float throwForceMultiplier = 5f;
    public float torqueForceMultiplier = 3f;

    public bool isDragging = false;
    private Vector3 lastMousePosition;
    private Vector3 currentVelocity;
    private Vector3 angularVelocity;
    private Rigidbody glassRigidbody;
    private GameObject[] diceArray;
    private Vector3[] diceLocalPositions;
    private Quaternion[] diceLocalRotations;

    void Start()
    {
        glassRigidbody = GetComponent<Rigidbody>();
        if (glassRigidbody == null)
        {
            glassRigidbody = gameObject.AddComponent<Rigidbody>();
            glassRigidbody.isKinematic = true;
        }

        InitializeDice();
    }

    void InitializeDice()
    {
        diceArray = new GameObject[diceCount];
        diceLocalPositions = new Vector3[diceCount];
        diceLocalRotations = new Quaternion[diceCount];

        for (int i = 0; i < diceCount; i++)
        {
            // Создаем кость внутри стакана
            Vector3 spawnPosition = diceSpawnPoint.position +
                new Vector3(Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f));

            diceArray[i] = Instantiate(dicePrefab, spawnPosition, Random.rotation, transform);
            diceArray[i].transform.localScale = Vector3.one * diceScale;

            // Сохраняем локальные позиции и вращения
            diceLocalPositions[i] = diceArray[i].transform.localPosition;
            diceLocalRotations[i] = diceArray[i].transform.localRotation;

            // Делаем Rigidbody кинематическими, чтобы они двигались с стаканом
            Rigidbody diceRb = diceArray[i].GetComponent<Rigidbody>();
            if (diceRb != null)
            {
                diceRb.isKinematic = true;
            }
        }
    }

    void Update()
    {
        HandleInput();

        if (isDragging)
        {
            UpdateDiceMovement();
        }
    }

    void HandleInput()
    {
        // Начало перетаскивания
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.transform.IsChildOf(transform))
            {
                isDragging = true;
                lastMousePosition = Input.mousePosition;
                currentVelocity = Vector3.zero;
                angularVelocity = Vector3.zero;
            }
        }

        // Перетаскивание
        if (isDragging && Input.GetMouseButton(0))
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 mouseDelta = currentMousePosition - lastMousePosition;

            // Движение стакана
            Vector3 movement = new Vector3(mouseDelta.x, 0, mouseDelta.y) * movementSpeed * Time.deltaTime;
            transform.Translate(movement, Space.World);

            // Вращение стакана (наклон)
            float tiltX = -mouseDelta.y * rotationSpeed * Time.deltaTime;
            float tiltZ = mouseDelta.x * rotationSpeed * Time.deltaTime;

            Vector3 currentRotation = transform.eulerAngles;
            currentRotation.x = Mathf.Clamp(currentRotation.x + tiltX, -maxTiltAngle, maxTiltAngle);
            currentRotation.z = Mathf.Clamp(currentRotation.z + tiltZ, -maxTiltAngle, maxTiltAngle);
            transform.eulerAngles = currentRotation;

            // Расчет скорости для костей
            currentVelocity = movement / Time.deltaTime;
            angularVelocity = new Vector3(tiltX, 0, tiltZ) / Time.deltaTime;

            lastMousePosition = currentMousePosition;
        }

        // Отпускание стакана
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            ThrowDice();
        }
    }

    void UpdateDiceMovement()
    {
        // Перемешиваем кости внутри стакана
        for (int i = 0; i < diceArray.Length; i++)
        {
            if (diceArray[i] != null)
            {
                // Добавляем случайное движение для эффекта перемешивания
                float randomOffset = Mathf.PerlinNoise(Time.time * 2f + i, i) - 0.5f;
                Vector3 offset = new Vector3(
                    randomOffset * 0.1f,
                    Mathf.Abs(randomOffset) * 0.05f,
                    randomOffset * 0.1f
                );

                diceArray[i].transform.localPosition = diceLocalPositions[i] + offset;
                diceArray[i].transform.Rotate(Random.onUnitSphere * angularVelocity.magnitude * 0.1f * Time.deltaTime);
            }
        }
    }

    void ThrowDice()
    {
        // Анимируем бросок стакана
        StartCoroutine(ThrowAnimation());

        // Выпускаем кости
        for (int i = 0; i < diceArray.Length; i++)
        {
            if (diceArray[i] != null)
            {
                ReleaseDice(diceArray[i], i);
            }
        }
    }

    IEnumerator ThrowAnimation()
    {
        Vector3 startRotation = transform.eulerAngles;
        Vector3 targetRotation = new Vector3(60f, startRotation.y, startRotation.z);

        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.eulerAngles = Vector3.Lerp(startRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Возвращаем стакан в исходное положение
        StartCoroutine(ResetGlass());
    }

    IEnumerator ResetGlass()
    {
        yield return new WaitForSeconds(1f);

        Vector3 startRotation = transform.eulerAngles;
        Vector3 targetRotation = Vector3.zero;

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.eulerAngles = Vector3.Lerp(startRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.eulerAngles = targetRotation;
    }

    void ReleaseDice(GameObject dice, int index)
    {
        // Активируем физику для кости
        Rigidbody diceRb = dice.GetComponent<Rigidbody>();
        if (diceRb != null)
        {
            diceRb.isKinematic = false;

            // Применяем силы на основе движения стакана
            Vector3 throwForce = (transform.forward + Vector3.up) * currentVelocity.magnitude * throwForceMultiplier;
            Vector3 torque = angularVelocity * torqueForceMultiplier;

            diceRb.AddForce(throwForce, ForceMode.Impulse);
            diceRb.AddTorque(torque, ForceMode.Impulse);
        }

        // Отключаем родителя, чтобы кость двигалась независимо
        dice.transform.SetParent(null);

        // Запускаем проверку значения кости через некоторое время
        DiceValue diceValue = dice.GetComponent<DiceValue>();
        if (diceValue != null)
        {
            StartCoroutine(StartDiceCheck(diceValue, 2f));
        }
    }

    IEnumerator StartDiceCheck(DiceValue diceValue, float delay)
    {
        yield return new WaitForSeconds(delay);
        diceValue.ForceCheck();
    }

    // Метод для сброса игры (можно вызвать для повторного броска)
    public void ResetGame()
    {
        // Уничтожаем старые кости
        foreach (var dice in diceArray)
        {
            if (dice != null) Destroy(dice);
        }

        // Создаем новые кости
        InitializeDice();

        // Сбрасываем позицию стакана
        transform.eulerAngles = Vector3.zero;
    }
}