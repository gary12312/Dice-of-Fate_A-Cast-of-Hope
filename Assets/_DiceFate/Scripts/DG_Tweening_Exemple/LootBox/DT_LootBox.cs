using DG.Tweening;
using UnityEngine;

namespace DG_Tweening_Exemple
{
    public class DT_LootBox : MonoBehaviour
    {
       [field: SerializeField] public int CoinsForOpen { get; private set; }


        public YieldInstruction Open()
        {
            return transform
                .DOScale(0, 0.5f)
                .SetEase(Ease.InBack)
                .Play()
                .WaitForCompletion();
        
        }

    }
}
