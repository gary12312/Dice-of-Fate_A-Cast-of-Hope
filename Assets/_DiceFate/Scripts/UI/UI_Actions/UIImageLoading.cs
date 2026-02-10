using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIImageLoading : MonoBehaviour
{
    [SerializeField] private Image image;

    public void DOFateImageLoadToZero(float duration)
    {  
        image.DOFade(0, duration)
           .OnStepComplete(() => { gameObject.SetActive(false); })
           .Play();
    }

    public void DOFateImageLoadToOne(float duration) => image.DOFade(1, duration).Play();
 
}
