using DiceFate.UI_Dice;
using UnityEngine;
using UnityEngine.UI;

public class UI_Menu : MonoBehaviour
{

    [SerializeField] private Button newGame;
    [SerializeField] private Button continueGame;
    [SerializeField] private Button settingsGame;



    private void Start()
    {
        ValidateScriptsAndObject();


        newGame.onClick.AddListener(NewGame);

    }


    private void NewGame()
    {

    }







    private void ValidateScriptsAndObject()
    {
        if (newGame == null)
            Debug.LogError($" для {this.name} Не установлена ссылка на newGame!");
        if (continueGame == null)
            Debug.LogError($" для {this.name} Не установлена ссылка на continueGame!");
        if (settingsGame == null)
            Debug.LogError($" для {this.name} Не установлена ссылка на settingsGame!");
    }

}
