using DiceFate.EventBus;
using DiceFate.Events;
using System.Collections;
using TMPro;
using UnityEngine;


namespace DiceFate.Dice
{
    public class DiceCubeValue : MonoBehaviour
    {
        [Header("Dice Settings")]
        [SerializeField] private Rigidbody diceRigidbody;
        [SerializeField] private float sleepThreshold = 0.1f;
        [SerializeField] private float checkDelay = 1f;
        [SerializeField] private float powerShake = 2f;
        [SerializeField] public float moveSpeed = 30f; //управляется из Mane
        [SerializeField] public float moveSpeed2 = 500f; //управляется из Mane
        private Vector3 start_position; // для движения совместно со станканом  к мыши
        private Vector3 positionBeforeToMove; // сохранить позицию перед движением к мыши
        private Vector3 vectorEntrePositionAndZero;
        private Vector3 randomOffset;
       // private bool IsShake = false;


        // Публичный параметр типа кубика с использованием enum
        [Header("Тип кубика")]
        public DiceType diceType;
        public enum DiceType
        {
            Movement,
            Attack,
            Shield,
            Counterattack
        }

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
            Bus<OnShakeEvent>.OnEvent += HandelDiceShakeAndMove;
            Bus<OnDropEvent>.OnEvent += HandelDiceDrop;
            Bus<OnMoveToMouseEvent>.OnEvent += HandelMoveToMouse;

        }
        private void OnDestroy()
        {
            Bus<OnShakeEvent>.OnEvent -= HandelDiceShakeAndMove;
            Bus<OnDropEvent>.OnEvent -= HandelDiceDrop;
            Bus<OnMoveToMouseEvent>.OnEvent -= HandelMoveToMouse;
        }

        void Start()
        {
            if (diceRigidbody == null)
                diceRigidbody = GetComponent<Rigidbody>();

            start_position = transform.position;

            // Генерируем случайное смещение в пределах 0.5f
            float randomX = Random.Range(-0.5f, 0.5f);
            float randomZ = Random.Range(-0.5f, 0.5f);

            // Смещение только по горизонтали (X и Z), высота (Y) остается как есть
            randomOffset = new Vector3(randomX, 0f, randomZ);
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

            vectorEntrePositionAndZero = transform.position - start_position;

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


        // Метод для получения последнего результата
        public int GetLastResult()
        {
            return lastResult;
        }

        // Метод для ручного броска кубика (можно вызвать из других скриптов)
        public void ThrowDice(Vector3 force, Vector3 torque)
        {
            diceRigidbody.AddForce(force, ForceMode.Impulse);
            diceRigidbody.AddTorque(torque, ForceMode.Impulse);
        }

        public void ResetDice()
        {

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
  

        // --------------------- Реализация событий ----------------------

        private void HandelDiceShakeAndMove(OnShakeEvent power) { }

        private void HandelMoveToMouse(OnMoveToMouseEvent point) { }


        private void HandelDiceDrop(OnDropEvent evt)
        {
            Debug.Log("Бросок");
            DropDice();
        }




        // --------------------- Бросок и тряска и перемещение кубика ------------------
        public void ShakeDice()
        {
            float x = Random.Range(-powerShake, powerShake);
            float y = Random.Range(-0, 0);
            float z = Random.Range(-powerShake, powerShake);
            transform.Rotate(x * Time.deltaTime * 500, y * Time.deltaTime * 500, z * Time.deltaTime * 500);
        }

        public void MoveDiceToMouse(OnMoveToMouseEvent point)
        {
            Vector3 targetPoint = point.Point + randomOffset;
            targetPoint.y = transform.position.y;
            transform.position = Vector3.Lerp(transform.position, targetPoint, moveSpeed * Time.deltaTime);
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

            Gizmos.color = UnityEngine.Color.red;
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
