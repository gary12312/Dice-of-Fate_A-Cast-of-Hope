using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Unity.Mathematics;

public class ButtonAnimation : MonoBehaviour
{
    [SerializeField] private Image imageButton;
    private Sequence _animationSequence;

    private void OnEnable()
    {
        _animationSequence = DOTween.Sequence();

        _animationSequence  
           .Append(imageButton.DOFade(0, 0f))
           .Append(imageButton.DOFade(1, 1f))
           .Play();
    }
}
