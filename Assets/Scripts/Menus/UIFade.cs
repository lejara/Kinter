using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class UIFade : MonoBehaviour
{
    public float fadeInTime;
    public float fadeOutTime;
    public AnimationCurve fadeInCurve;
    public AnimationCurve fadeOutCurve;
    public UnityEvent onFadeInComplete;
    public UnityEvent onFadeOutComplete;

    CanvasGroup _canvasGroup;

    [ContextMenu("FadeIn")]
    public void StartFadeIn()
    {
        _canvasGroup.alpha = 0;
        StopAllCoroutines();

        StartCoroutine(FadeIn());
    }

    [ContextMenu("FadeOut")]
    public void StartFadeOut()
    {
        _canvasGroup.alpha = 1;
        StopAllCoroutines();

        StartCoroutine(FadeOut());
    }

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    IEnumerator FadeIn()
    {
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            _canvasGroup.alpha = fadeInCurve.Evaluate(normalizedTime);
            normalizedTime += Time.deltaTime / fadeInTime;
            yield return null;
        }
        _canvasGroup.alpha = 1;
        onFadeInComplete.Invoke();
    }

    IEnumerator FadeOut()
    {
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            _canvasGroup.alpha = 1 - fadeOutCurve.Evaluate(normalizedTime);
            normalizedTime += Time.deltaTime / fadeOutTime;
            yield return null;
        }
        _canvasGroup.alpha = 0;
        onFadeOutComplete.Invoke();
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

}
