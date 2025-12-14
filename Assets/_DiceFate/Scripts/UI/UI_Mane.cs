using UnityEngine;
using DiceFate.UI_Dice;

namespace DiceFate.UI
{
    public class UI_Mane : MonoBehaviour
    {
        [SerializeField] private UiDiceTargetResult uiResultDisplay;

        private void Start()
        {
            ValidateScripts();
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                uiResultDisplay.InitializeResultDisplay();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                uiResultDisplay.UpdateResultDisplay();
            }
        }

        // проверка установки скриптов  
        private void ValidateScripts()
        {
            if (uiResultDisplay == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на uiResultDisplay!");

        }


        // Показываем в UI какие кубики были брошены
        public void UiEnableResultDisplay()
        {
            uiResultDisplay.InitializeResultDisplay();
        }

        // Обновляем значения на карточке в UI
        public void UiSetResultToCard()
        {
            uiResultDisplay.UpdateResultDisplay();
        }

        public void UiDisableResultDisplay() => uiResultDisplay.OffResultOnDisplays();
    }
}
