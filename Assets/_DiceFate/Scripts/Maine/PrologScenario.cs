using DiceFate.Animation;
using DiceFate.MouseW;
using DiceFate.UI;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


namespace DiceFate.Maine
{
    public class PrologScenario : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private GameObject cameraMain;
        [SerializeField] private GameObject cameraVirtualOne;
        [SerializeField] private GameObject cameraTarget;
        [SerializeField] public bool isTesting;
        [SerializeField] public int prologNumber = 5;  // 1- часть.

        [Header("Настройки движения камеры")]       // Дополнительные параметры для управления камерой
        [SerializeField] private new Camera camera;
        [SerializeField] private Rigidbody cameraTargetRB;
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
        [SerializeField] private GameObject canvasBattle;
        [SerializeField] private GameObject CanvasScenario;
        [SerializeField] private GameObject playerInput;
        [SerializeField] private GameObject cold;
        [SerializeField] private GameObject[] texts;
        // 0 - Text 1
        // 1 - Text 2
        // 2 - Text 3

        [Header("Zero")]
        [SerializeField] private GameObject Screenloder;
        [SerializeField] private float durationScreenloder = 0.5f;
        [SerializeField] private ParticleSystem rainRocks;
        [SerializeField] private DT_AnimationRocks animationRocks;
        [SerializeField] private CinemachineImpulseSource impulseSource;

        [Header("Five")]
        [SerializeField] private GameObject iBackgroundOffClicker;



        [Header("Six")]
        [SerializeField] private GameObject diceMove;
        [SerializeField] private GameObject diceAttack;
        [SerializeField] private GameObject diceShild;
        [SerializeField] private GameObject diceConterAtack;


        [Space]
        [SerializeField] private MoveCameraTarget moveCameraTarget;
        [SerializeField] private CursorTutorial cursorTutorial;
        [SerializeField] private UI_ManeTutorial ui_ManeTutorial;

        [Space]
       

        private CanvasGroup currentCanvasGroup;
        private Vector2 mouseDelta;            // Изменение положения мыши
     
        private bool isColdActive = false;


        private void Awake()
        {
            
            if (isTesting)
            {
                Screenloder.SetActive(true);
                cameraMain.SetActive(false);
                cameraVirtualOne.SetActive(true);
            }
            else
            {
                cameraMain.SetActive(true);
                cameraVirtualOne.SetActive(false);
            }

            HideMouseAndPartical();

            rainRocks.Stop();
        }

        private void Start()
        {
            PrologOneBegin();
        }

        private void Update()
        {
            switch (prologNumber)
            {
                case 1:
                    RightClickAndHold();   // Обработка правого клика (перемещение)
                    break;
                case 2:
                    MiddleClickAndHold();  // Обработка центрального клика (поворот)
                    break;
            }
        }

        private void PrologOneBegin()
        {
            SetActiveAllObjectsOff(texts);
           // canvas.SetActive(false);
            canvasBattle.SetActive(false);
            iBackgroundOffClicker.SetActive(false);


            if (isTesting)
            {
                cold.SetActive(false);
                playerInput.SetActive(false);
                StartCoroutine(ScenarioOne());
            }
        }


        //---------------------------------- Клики мышы  -------------------------------------------------------------
        private void RightClickAndHold()
        {
            if (isTesting)
            {
                if (cameraTargetRB == null) return;

                if (Mouse.current.rightButton.isPressed)
                {
                    MoveCamera();
                }
                else
                {
                    // Останавливаем камеру при отпускании кнопки
                    cameraTargetRB.linearVelocity = Vector3.zero;
                }

                if (Mouse.current.rightButton.wasReleasedThisFrame)
                {
                    //StartCoroutine(ScenarioThree());
                    StartCoroutine(ScenarioTwo());
                }
            }
        }

        private void MiddleClickAndHold()
        {
            if (isTesting)
            {
                if (Mouse.current.middleButton.isPressed) { RotationCamera(); }
                if (Mouse.current.middleButton.wasReleasedThisFrame)
                {
                    //  StartCoroutine(ScenarioTwo());
                    StartCoroutine(ScenarioThree());
                }
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

            cameraTargetRB.linearVelocity = moveDirection; // Применяем движение к цели камеры
        }

        // Вращение камеры за счет вращения цели камеры ( cameraTarget )
        private void RotationCamera()
        {
            mouseDelta = Mouse.current.delta.ReadValue();                       // Получаем изменение положения мыши            
            float deltaY = mouseDelta.x * cameraRotationSpeed * Time.deltaTime; // Вращаем только по горизонтали (оси Y) на основе движения мыши по X

            if (cameraTargetRB != null)
            {
                Vector3 currentRotation = cameraTargetRB.rotation.eulerAngles;    // Получаем текущее вращение cameraTarget

                float newYRotation = currentRotation.y + deltaY;                // Добавляем вращение только по оси Y
                Quaternion newRotation = Quaternion.Euler(currentRotation.x, newYRotation, currentRotation.z);  // Создаем новое вращение, сохраняя X и Z неизменными

                cameraTargetRB.rotation = newRotation;                            // Применяем вращение к cameraTarget
            }

            ActiveCold();
        }

        // --------------------------- Сценарии ----------------------------
        private IEnumerator ScenarioOne()
        {
            ImageLoder();

            yield return new WaitForSeconds(0.3f);
            CameraShakeCinemachine();

            yield return new WaitForSeconds(0.1f);
            rainRocks.Play();
            animationRocks.StartAnimationRocks();

            yield return new WaitForSeconds(0.3f);
            // Переключаем камеры
            cameraMain.SetActive(true);
            cameraVirtualOne.SetActive(false);

            yield return new WaitForSeconds(1.5f);

            // Первый текст 
            ShowObject(0);
            FadeTextIn();

            yield return new WaitForSeconds(1f);      
            ShowMouse(mouse_R);
            prologNumber = 1;
        }

        private IEnumerator ScenarioTwo()
        {
            FadeTextOut();
           // psMouse_M.gameObject.SetActive(true);      
            HideMouse(mouse_R);

            // Второй текст 
            yield return new WaitForSeconds(0.5f);
            ShowObject(1);
            FadeTextIn();
            prologNumber = 2;
            ShowMouse(mouse_M);
        }

        private IEnumerator ScenarioThree()
        {
            prologNumber = 3;
            FadeTextOut();     
            HideMouse(mouse_M);
           

            moveCameraTarget.CameraTargetAnimationToOneTarget();


            // третий текст - появляется
            yield return new WaitForSeconds(0.5f);
            playerInput.SetActive(true);
            ShowObject(2);
            FadeTextIn();  // Свиток огня, я его нашол
            ShowMouse(mouse_L);
        }

        public void StartScenarioFour() => StartCoroutine(ScenarioFour());
        private IEnumerator ScenarioFour()
        {
            prologNumber = 4;
            FadeTextOut();
            HideMouse(mouse_L);
            iBackgroundOffClicker.SetActive(true);
            // canvas.SetActive(true);


            yield return new WaitForSeconds(0.5f);          
            ShowObject(3); // Немогу дотянуться
            FadeTextIn();

            yield return new WaitForSeconds(1f);
            cursorTutorial.AnimationCursorForPrologFour();
          

            yield return new WaitForSeconds(3f);
            cursorTutorial.StopAnimation();
           

        }

        public void StartScenarioFive()
        {
            if (prologNumber <= 4)
            {
                StartCoroutine(ScenarioFive());
            }           
        }    
        
        private IEnumerator ScenarioFive()
        {
            prologNumber = 5;
            FadeTextOut();
           // ui_ManeTutorial.UiBackgroundOffClicker(true);
          // iBackgroundOffClicker.SetActive(true);


            yield return new WaitForSeconds(1f);
            cursorTutorial.AnimationCursorForPrologFive();
            ShowObject(4); // За шаг не пройти, мне нужен магический куб
            FadeTextIn();


            yield return new WaitForSeconds(3f);
            cursorTutorial.StopAnimation();
        }

        public void StartScenarioSix()
        {
            if (prologNumber <= 5)
            {
                StartCoroutine(ScenarioSix());
            }
        }

        private IEnumerator ScenarioSix()
        {
            prologNumber = 6;
            iBackgroundOffClicker.SetActive(false);
            FadeTextOut();
           // ui_ManeTutorial.UiBackgroundOffClicker(false);



            yield return new WaitForSeconds(1f);
            cursorTutorial.AnimationCursorForPrologSix();


            yield return new WaitForSeconds(4.5f);
            cursorTutorial.StopAnimation();
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

        // --------------------------- 

        private void ActiveCold()
        {
            if (!isColdActive)
            {
                cold.SetActive(true);
            }

            isColdActive = true;
        }

        // -------------------------- Тряска камеры ---------------------------
        public void CameraShakeCinemachine()
        {
            CameraShakeManadger.instance.CameraShake(impulseSource);
        }

        private void ImageLoder()
        {
            UIImageLoading uiImage = Screenloder.GetComponent<UIImageLoading>();

            uiImage.DOFateImageLoadToZero(durationScreenloder);      
        }

    }
}