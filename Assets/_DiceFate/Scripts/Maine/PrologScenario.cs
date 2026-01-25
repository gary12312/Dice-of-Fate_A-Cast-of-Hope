using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DiceFate.Maine
{
    public class PrologScenario : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private GameObject cameraMain;
        [SerializeField] private GameObject cameraVirtualOne;
        [SerializeField] private bool isTesting;

        [Header("Настройки движения камеры")]       // Дополнительные параметры для управления камерой
        [SerializeField] private new Camera camera;
        [SerializeField] private Rigidbody cameraTarget;
        [SerializeField] private float cameraMoveSpeedX = 2.1f; // Скорость движения камеры
        [SerializeField] private float cameraMoveSpeedY = 4f; // Скорость движения камеры
        [SerializeField] private float cameraRotationSpeed = 10f; // Скорость вращения камеры

        [Header("Fade Settings")]
        [SerializeField] private float fadeDuration = 1f;

        [Header("Mouse")]
        [SerializeField] private GameObject mouse_L;
        [SerializeField] private ParticleSystem psMouse_L;
        [SerializeField] private GameObject mouse_M;
        [SerializeField] private ParticleSystem psMouse_M;
        [SerializeField] private GameObject mouse_R;
        [SerializeField] private ParticleSystem psMouse_R;

        [Header("Objects")]
        [SerializeField] private GameObject canvas;
        [SerializeField] private GameObject playerInput;
        [SerializeField] private GameObject[] texts;
        // 0 - Text 1
        // 1 - Text 2
        // 2 - Text 3




        private CanvasGroup currentCanvasGroup;
        private Vector2 mouseDelta;            // Изменение положения мыши
        private bool ifPrologInput = true;

        private bool isFerstPressMiddleButton = true;



        private void Awake()
        {
            if (isTesting)
            {
                cameraMain.SetActive(false);
                cameraVirtualOne.SetActive(true);
            }

            HideMouseAndPartical();
        }

        private void Start()
        {
            SetActiveAllObjectsOff(texts);
            canvas.SetActive(false);
            playerInput.SetActive(false);

            if (isTesting)
            {
                StartCoroutine(ScenarioOne());
            }
        }


        private void Update()
        {
            if (ifPrologInput)
            {
                MiddleClickAndHold();  // Обработка центрального клика (поворот)
                RightClickAndHold();   // Обработка правого клика (перемещение)
            }

        }

        //---------------------------------- Клики мышы  -------------------------------------------------------------
        private void MiddleClickAndHold()
        {
            if (Mouse.current.middleButton.isPressed) { RotationCamera(); }
            if (Mouse.current.middleButton.wasReleasedThisFrame && isFerstPressMiddleButton)
            {

                isFerstPressMiddleButton = false;
                StartCoroutine(ScenarioTwo());
            }
        }

        private void RightClickAndHold()
        {
            if (cameraTarget == null) return;

            if (Mouse.current.rightButton.isPressed)
            {
                MoveCamera();
            }
            else
            {
                // Останавливаем камеру при отпускании кнопки
                cameraTarget.linearVelocity = Vector3.zero;
            }
        }

        // -------------------------------- обучение Мышь ------------------------------------

        private void HideMouseAndPartical()
        {
            psMouse_M.gameObject.SetActive(false);
            psMouse_R.gameObject.SetActive(false);
            psMouse_L.gameObject.SetActive(false);

            mouse_M.SetActive(false);
            mouse_R.SetActive(false);
            mouse_L.SetActive(false);
        }

        public void ShowMouse(GameObject gameObject)
        {
            gameObject.SetActive(true);
        }
        public void HideMouse(GameObject gameObject)
        {
            gameObject.SetActive(false);
        }

        public void ShowPartical(ParticleSystem particleSystem)
        {
            particleSystem.gameObject.SetActive(true);
            particleSystem.Play();
        }

        //---------------------------------- Функции Перемещение и вращение камеры  ----------------------------------

        private void MoveCamera()
        {
            mouseDelta = Mouse.current.delta.ReadValue();                     // Получаем изменение положения мыши
            float deltaX = -mouseDelta.x * cameraMoveSpeedX * Time.deltaTime; // сколсть по X  Инвертируем
            float deltaY = -mouseDelta.y * cameraMoveSpeedY * Time.deltaTime;

            if (camera == null) return;

            // Получаем направления камеры и игнорируем вертикальную ось
            Vector3 moveDirection =
                camera.transform.right * deltaX +
                camera.transform.forward * deltaY; // Вычисляем направление движения на основе положения мыши
            moveDirection.y = 0;

            cameraTarget.linearVelocity = moveDirection; // Применяем движение к цели камеры
        }

        // Вращение камеры за счет вращения цели камеры ( cameraTarget )
        private void RotationCamera()
        {
            mouseDelta = Mouse.current.delta.ReadValue();                       // Получаем изменение положения мыши            
            float deltaY = mouseDelta.x * cameraRotationSpeed * Time.deltaTime; // Вращаем только по горизонтали (оси Y) на основе движения мыши по X

            if (cameraTarget != null)
            {
                Vector3 currentRotation = cameraTarget.rotation.eulerAngles;    // Получаем текущее вращение cameraTarget

                float newYRotation = currentRotation.y + deltaY;                // Добавляем вращение только по оси Y
                Quaternion newRotation = Quaternion.Euler(currentRotation.x, newYRotation, currentRotation.z);  // Создаем новое вращение, сохраняя X и Z неизменными

                cameraTarget.rotation = newRotation;                            // Применяем вращение к cameraTarget
            }
        }



        // --------------------------- Сценарии ----------------------------
        private IEnumerator ScenarioOne()
        {
            // Переключаем камеры
            cameraMain.SetActive(true);
            cameraVirtualOne.SetActive(false);

            yield return new WaitForSeconds(1.5f);

            // Первый текст - появляется
            ShowObject(0);
            FadeTextIn();

            yield return new WaitForSeconds(2f);
            ShowMouse(mouse_M);

            //yield return new WaitForSeconds(2f);
            //FadeTextOut();          
        }
        private IEnumerator ScenarioTwo()
        {
           
            FadeTextOut();
            psMouse_M.gameObject.SetActive(true);
            HideMouse(mouse_M);
            
            
            // Второй текст - появляется
            yield return new WaitForSeconds(0.5f);
            ShowObject(1);
            FadeTextIn();

            yield return new WaitForSeconds(1.5f);
            ShowMouse(mouse_R);

            yield return new WaitForSeconds(2f);
            FadeTextOut();
        }
       

        // --------------------------- Работа с текстами ----------------------------
        public void ShowObject(int index)
        {
            // Выключить все тексты
            SetActiveAllObjectsOff(texts);

            // Включить выбранный
            SetActiveTextOnIndex(texts, index, true);
        }

        private void SetActiveTextOnIndex(GameObject[] textObjects, int index, bool isActive)
        {
            if (textObjects == null || index < 0 || index >= textObjects.Length)
            {
                Debug.LogWarning($"Неверный индекс: {index}");
                return;
            }

            GameObject textObject = textObjects[index];
            if (textObject == null)
            {
                Debug.LogWarning($"TextObject[{index}] равен null");
                return;
            }

            textObject.SetActive(isActive);

            if (isActive)
            {
                // Получаем или добавляем CanvasGroup
                currentCanvasGroup = textObject.GetComponent<CanvasGroup>();
                if (currentCanvasGroup == null)
                {
                    currentCanvasGroup = textObject.AddComponent<CanvasGroup>();
                }

                // Устанавливаем начальную прозрачность
                currentCanvasGroup.alpha = 0f; // Начинаем с невидимого
            }
        }

        private void SetActiveAllObjectsOff(GameObject[] textObjects)
        {
            if (textObjects == null) return;

            foreach (var textObject in textObjects)
            {
                if (textObject != null)
                {
                    textObject.SetActive(false);

                    // Опционально: удаляем CanvasGroup у неактивных объектов
                    CanvasGroup cg = textObject.GetComponent<CanvasGroup>();
                    if (cg != null && Application.isPlaying)
                    {
                        Destroy(cg);
                    }
                }
            }
        }

        // --------------------------- Изменение прозрачности ----------------------------
        public void FadeTextOut()
        {
            if (currentCanvasGroup != null)
            {
                StartCoroutine(FadeCanvasGroup(currentCanvasGroup.alpha, 0f, fadeDuration));
            }
        }

        public void FadeTextIn()
        {
            if (currentCanvasGroup != null)
            {
                StartCoroutine(FadeCanvasGroup(currentCanvasGroup.alpha, 1f, fadeDuration));
            }
        }

        private IEnumerator FadeCanvasGroup(float startAlpha, float endAlpha, float duration)
        {
            if (currentCanvasGroup == null) yield break;

            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = Mathf.Clamp01(time / duration);

                currentCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
                yield return null;
            }

            currentCanvasGroup.alpha = endAlpha;

            // Если это исчезновение, можно скрыть объект
            if (endAlpha == 0f && currentCanvasGroup.gameObject.activeSelf)
            {
                currentCanvasGroup.gameObject.SetActive(false);
            }
        }
    }
}