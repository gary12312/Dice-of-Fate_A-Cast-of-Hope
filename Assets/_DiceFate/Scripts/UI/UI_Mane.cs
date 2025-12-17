using UnityEngine;
using DiceFate.UI_Dice;

namespace DiceFate.UI
{
    public class UI_Mane : MonoBehaviour
    {
        [SerializeField] private UiDiceTargetResult uiResultTargetDisplay;

        private void Start()
        {
            ValidateScripts();
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                uiResultTargetDisplay.InitializeResultDisplay();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                uiResultTargetDisplay.UpdateResultDisplay();
            }
        }

        // проверка установки скриптов  
        private void ValidateScripts()
        {
            if (uiResultTargetDisplay == null)
                Debug.LogError($" дл€ {this.name} Ќе установлена ссылка на uiResultDisplay!");
        }


        // ѕоказываем в UI какие кубики были брошены
        public void UiEnableResultDisplay()
        {
            uiResultTargetDisplay.InitializeResultDisplay();
        }

        // записываем в GameStats и обновл€ем значени€ на карточке в UI 
        public void SetResultToCard()
        {
            uiResultTargetDisplay.SaveResultsToGameStats();
            uiResultTargetDisplay.UpdateResultDisplay();
        }




        public void UiDisableResultDisplay() => uiResultTargetDisplay.OffResultOnDisplays();
    }
}
