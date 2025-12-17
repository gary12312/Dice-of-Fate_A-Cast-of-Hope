using System;
using TMPro;
using UnityEngine;

public class PhaseNumberText : MonoBehaviour
{
    public TextMeshProUGUI phaseNumberText;


    internal void UpdatePhaseNumberText(int currentPhasePlayer)
    {

        phaseNumberText.text = $"Фаза {currentPhasePlayer}";
        Debug.Log($"Текущая фаза {currentPhasePlayer}");
        Debug.Log($"Текущая фаза 111 {phaseNumberText.text}");
    }


}
