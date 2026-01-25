using System.Collections;
using UnityEngine;

public class UI_FadeObject : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1f;
    private CanvasGroup currentCanvasGroup;

    private void Start()
    {
        currentCanvasGroup = GetComponent<CanvasGroup>();
        
        if (currentCanvasGroup == null)
        {
            Debug.LogWarning($"CanvasGroup component not found on the {this.gameObject}.");
        }
    }


    public void FadeOut()
    {
        if (currentCanvasGroup != null)
        {
            StartCoroutine(FadeCanvasGroup(currentCanvasGroup.alpha, 0f, fadeDuration));
        }
    }

    public void FadeIn()
    {
        if (currentCanvasGroup != null)
        {
            StartCoroutine(FadeCanvasGroup(currentCanvasGroup.alpha, 1f, fadeDuration));
        }
    }

    private IEnumerator FadeCanvasGroup(float startAlpha, float endAlpha, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);

            currentCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }

        currentCanvasGroup.alpha = endAlpha;
        
    }
}
