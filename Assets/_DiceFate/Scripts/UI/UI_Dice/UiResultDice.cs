using UnityEngine;
using UnityEngine.UI;

namespace DiceFate.UI_Dice
{
    public class UiResultDice : MonoBehaviour
    {
        [Header("UI Elements для значений")]
        [SerializeField] private GameObject value1;
        [SerializeField] private GameObject value2;
        [SerializeField] private GameObject value3;
        [SerializeField] private GameObject value4;
        [SerializeField] private GameObject value5;
        [SerializeField] private GameObject value6;
        [Space]
        [SerializeField] private GameObject ferstParticle;
        [SerializeField] private GameObject secondParticle;

        private Color colorOfType;

        private void Awake() => ResetDisplay();

        public void Test()
        {
            Debug.Log("Test UiResultDice_q");
        }
        public void OnEnabledValue(int result)
        {
            ResetDisplay();

            switch (result)
            {
                case 1:
                    value1.SetActive(true);
                    break;
                case 2:
                    value2.SetActive(true);
                    break;
                case 3:
                    value3.SetActive(true);
                    break;
                case 4:
                    value4.SetActive(true);
                    break;
                case 5:
                    value5.SetActive(true);
                    break;
                case 6:
                    value6.SetActive(true);
                    break;
                default:
                    Debug.LogWarning($"Некорректный результат кубика: {result}");
                    break;
            }
        }

        public void ColorDiceResult(string typeDice)
        {
            switch (typeDice)
            {
                case "Movement":
                    colorOfType = Color.blue;
                    break;

                case "Attack":
                    colorOfType = Color.red;
                    break;

                case "Shield":
                    colorOfType = Color.green;
                    break;

                case "Counterattack":
                    colorOfType = new Color(1f, 0.8f, 0f); // Желтый
                    break;

                default:
                    colorOfType = Color.white;
                    break;
            }

            value1.GetComponent<Image>().color = colorOfType;
            value2.GetComponent<Image>().color = colorOfType;
            value3.GetComponent<Image>().color = colorOfType;
            value4.GetComponent<Image>().color = colorOfType;
            value5.GetComponent<Image>().color = colorOfType;
            value6.GetComponent<Image>().color = colorOfType;
        }

        private void ResetDisplay()
        {
            value1.SetActive(false);
            value2.SetActive(false);
            value3.SetActive(false);
            value4.SetActive(false);
            value5.SetActive(false);
            value6.SetActive(false);
        }



    }
}

