// Пример вызова из другого скрипта:
using DiceFate.UI_Dice;
using UnityEngine;

public class T_DiceManager : MonoBehaviour
{
    [SerializeField] private UiDiceTargetResult uiResultDisplay;
    private void Update()
    { 
        if (Input.GetKeyDown(KeyCode.Q))
        {
           // uiResultDisplay.InitializeResultDisplay();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            uiResultDisplay.UpdateResultDisplay(); 
        }
    }

   

 
}