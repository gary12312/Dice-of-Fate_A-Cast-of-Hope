using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    public GameObject green_dice;
    public GameObject red_dice;
    public GameObject blue_dice;
    public GameObject total;

    public static GameObject green_dice_static;
    public static GameObject red_dice_static;
    public static GameObject blue_dice_static;
    public static GameObject total_static;
    void Start()
    {
        green_dice_static = green_dice;
        red_dice_static = red_dice;
        blue_dice_static = blue_dice;
        total_static = total;
    }

    public static void ShowTotal(int value)
    {
        total_static.GetComponent<TextMeshProUGUI>().text = "Total: " + value.ToString();
    }

    public static void ShowResultGreenDice(int value)
    {
        green_dice_static.GetComponent<TextMeshProUGUI>().text = "Green Dice: " + value.ToString();
    }

    public static void ShowResultRedDice(int value)
    {
        red_dice_static.GetComponent<TextMeshProUGUI>().text = "Red Dice: " + value.ToString();
    }

    public static void ShowResultBlueDice(int value)
    {
        blue_dice_static.GetComponent<TextMeshProUGUI>().text = "Blue Dice: " + value.ToString();
    }

    public static void ResetDiceResult()
    {
        green_dice_static.GetComponent<TextMeshProUGUI>().text = "Green Dice: ";
        red_dice_static.GetComponent<TextMeshProUGUI>().text = "Red Dice: ";
        blue_dice_static.GetComponent<TextMeshProUGUI>().text = "Blue Dice: ";
    }
}
