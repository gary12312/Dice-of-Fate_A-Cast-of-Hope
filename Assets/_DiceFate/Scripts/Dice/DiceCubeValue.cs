using DiceFate.EventBus;
using DiceFate.Events;
using System.Collections;
using TMPro;
using UnityEngine;


namespace DiceFate.Dice
{
    public class DiceCubeValue : MonoBehaviour, ISelectableDice
    {
        [Header("Dice Settings")]
        public Rigidbody diceRigidbody;
        public float sleepThreshold = 0.1f;
        public float checkDelay = 1f;
        public float powerShake = 2f;
            

        [Header("UI Settings")]
        public TextMeshProUGUI resultText;
        public string waitingText = "Бросок...";
        public string resultPrefix = "Результат: ";

        [Header("Dice Face Values")]
        public Vector3[] faceDirections = new Vector3[]
        {
        Vector3.up,      // 1
        Vector3.down,    // 6
        Vector3.right,   // 4
        Vector3.left,    // 3
        Vector3.forward, // 2
        Vector3.back     // 5
        };

        public int[] faceValues = new int[] { 1, 6, 4, 3, 2, 5 };

        private bool isChecking = false;
        private int lastResult = 0;

         [field: SerializeField] public bool IsShakeSelectDice { get; private set; }   // ISelectableDice

        private void Awake()
        {
            Bus<OnShakeEvent>.OnEvent += HandelDiceShake;
            Bus<OnDropEvent>.OnEvent += HandelDiceDrop;
        }


        private void OnDestroy()
        {
            Bus<OnShakeEvent>.OnEvent -= HandelDiceShake;
            Bus<OnDropEvent>.OnEvent -= HandelDiceDrop;
        }

        void Start()
        {
            if (diceRigidbody == null)
                diceRigidbody = GetComponent<Rigidbody>();

            if (resultText != null)
                resultText.text = "Готов к броску";
        }

        void Update()
        {
            // Если кубик движется и мы еще не начали проверку
            if (!isChecking && (diceRigidbody.linearVelocity.magnitude > sleepThreshold ||
                               diceRigidbody.angularVelocity.magnitude > sleepThreshold))
            {
                StartChecking();
            }
        }

        void StartChecking()
        {
            if (!isChecking)
            {
                isChecking = true;
                StartCoroutine(CheckDiceValue());
            }
        }

        IEnumerator CheckDiceValue()
        {
            if (resultText != null)
                resultText.text = waitingText;

            // Ждем пока кубик не остановится
            while (diceRigidbody.linearVelocity.magnitude > sleepThreshold ||
                   diceRigidbody.angularVelocity.magnitude > sleepThreshold)
            {
                yield return new WaitForSeconds(checkDelay);
            }

            // Добавляем дополнительную задержку для уверенности
            yield return new WaitForSeconds(0.5f);

            // Определяем значение кубика
            int result = GetDiceValue();

            // Обновляем UI
            if (resultText != null)
                resultText.text = resultPrefix + result;

            lastResult = result;
            isChecking = false;

            Debug.Log("Значение кубика: " + result);
        }

        int GetDiceValue()
        {
            int topFaceIndex = -1;
            float maxDot = -Mathf.Infinity;

            // Ищем грань с наибольшим скалярным произведением с вектором вверх
            for (int i = 0; i < faceDirections.Length; i++)
            {
                // Преобразуем направление грани в мировые координаты
                Vector3 worldDirection = transform.TransformDirection(faceDirections[i]);
                float dot = Vector3.Dot(worldDirection, Vector3.up);

                if (dot > maxDot)
                {
                    maxDot = dot;
                    topFaceIndex = i;
                }
            }

            if (topFaceIndex >= 0 && topFaceIndex < faceValues.Length)
            {
                return faceValues[topFaceIndex];
            }

            return 0; // Если не удалось определить
        }


  



        // Метод для принудительной проверки значения (можно вызвать извне)
        public void ForceCheck()
        {
            if (!isChecking)
            {
                StartChecking();
            }
        }

        // Метод для сброса текста
        public void ResetText()
        {
            if (resultText != null)
                resultText.text = "Готов к броску";

            lastResult = 0;
        }

        // Метод для получения последнего результата
        public int GetLastResult()
        {
            return lastResult;
        }

        // Метод для ручного броска кубика (можно вызвать из других скриптов)
        public void ThrowDice(Vector3 force, Vector3 torque)
        {
            ResetText();
            diceRigidbody.AddForce(force, ForceMode.Impulse);
            diceRigidbody.AddTorque(torque, ForceMode.Impulse);
        }

        public void ResetDice()
        {
            if (resultText != null)
                resultText.text = "Готов к броску";

            lastResult = 0;
            isChecking = false;

            // Сбрасываем физику
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }

        //-------------------- ISelectableDice реализация --------------
        public void SelectDice()
        {
            throw new System.NotImplementedException();
        }

        public void DeselectDice()
        {
            throw new System.NotImplementedException();
        }
        public void ShakeSelectDice()
        {
           
           // IsShakeSelectDice= true;
        }

        public void DropSelectDice()
        {
            
           // IsShakeSelectDice = false;
        }

        // --------------------- Реализация событий ----------------------

        private void HandelDiceShake(OnShakeEvent evt)
        {            
            ShakeDice();
        }

        private void HandelDiceDrop(OnDropEvent evt)
        {
            Debug.Log("Бросок");
            DropDice();
        }




        // --------------------- Бросок и тряска кубика ------------------

        public void ShakeDice()
        {
            float x = Random.Range(-powerShake, powerShake);
            float y = Random.Range(-powerShake, powerShake);
            float z = Random.Range(-powerShake, powerShake);
            transform.Rotate(x * Time.deltaTime * 500, y * Time.deltaTime * 500, z * Time.deltaTime * 500);
        }



        public void DropDice()
        {
            //drop = true;
            //RotateDice();
            GetComponent<Rigidbody>().AddForce(Vector3.forward * 500);
        }

        // --------------------- Визуализация направлений граней в редакторе (для отладки) ------------------
        void OnDrawGizmosSelected()
        {
            if (faceDirections == null) return;

            Gizmos.color = Color.red;
            for (int i = 0; i < faceDirections.Length; i++)
            {
                if (i < faceValues.Length)
                {
                    Vector3 worldDirection = transform.TransformDirection(faceDirections[i]);
                    Gizmos.DrawRay(transform.position, worldDirection * 0.5f);

                    // Подписываем грани для удобства отладки
#if UNITY_EDITOR
                    UnityEditor.Handles.Label(transform.position + worldDirection * 0.6f, faceValues[i].ToString());
#endif
                }
            }
        }

 
    }
}
