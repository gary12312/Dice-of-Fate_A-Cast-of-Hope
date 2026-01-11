using DG.Tweening;
using UnityEngine;

namespace DG_Tweening_Exemple
{
    public class DT_Coins : MonoBehaviour
    {

        public YieldInstruction Pickup()
        {
            return DOTween.Sequence()
                .Append(transform.DOMove(transform.position + Vector3.up * 3, 0.5f))
                .Join(transform.DORotate(Vector3.up * 180, 0.5f))
                .Append(transform.DOScale(0, 0.25f).SetEase(Ease.InQuint))
                .Join(transform.DORotate(Vector3.up * 720, 0.25f, RotateMode.FastBeyond360))
                .Play()
                .WaitForCompletion();


        }


    }
}
