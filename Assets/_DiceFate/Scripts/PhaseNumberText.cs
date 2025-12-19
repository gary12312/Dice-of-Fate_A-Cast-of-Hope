using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PhaseNumberText : MonoBehaviour
{
    
    public TextMeshProUGUI phaseNumberText;

    public List<int> listNumbers = new List<int> { 1,2,4};
 
    internal void UpdatePhaseNumberText(int currentPhasePlayer)
    {
        phaseNumberText.text = $"Текущая фаза {currentPhasePlayer}";

        listNumbers.Clear();
    
    }

    


}
