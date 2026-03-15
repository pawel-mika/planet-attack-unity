using System;
using System.Collections;
using UnityEngine;

public static class TransformExtensions
{

    public static void PopIn(this Transform target,
        MonoBehaviour runner,
        float duration = 1.5f,
        Vector3? targetScale = null,
        float frequency = 20f,
        float damping = 2f,
        float durationVariance = 0.75f,
        Action onComplete = null)
    {
        Vector3 finalScale = targetScale ?? (target.localScale == Vector3.zero ? Vector3.one : target.localScale);

        float varTime = duration * durationVariance;
        float finalDuration = duration + UnityEngine.Random.Range(-varTime, varTime);
        target.localScale = Vector3.zero;

        runner.RunWithDelay(() =>
        {
            runner.StartCoroutine(AnimateSpring(target, finalScale, finalDuration, frequency, damping, onComplete));
        }, 0.75f, 0.5f);
    }

    public static void PopOut(this Transform target, MonoBehaviour runner, float duration = 0.5f, Action onComplete = null)
    {
        runner.StopAllCoroutines();
        Vector3 initialScale = target.localScale;
        Vector3 finalScale = Vector3.zero;

        runner.StartCoroutine(AnimateScaleDown(target, initialScale, finalScale, duration, onComplete));
    }

    private static IEnumerator AnimateScaleDown(Transform target, Vector3 start, Vector3 end, float duration, Action onComplete)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float ease = t * t * (3f - 2f * t);
            target.localScale = Vector3.Lerp(start, end, ease);
            yield return null;
        }

        target.localScale = end;
        target.gameObject.SetActive(false);

        onComplete?.Invoke();
    }

    private static IEnumerator AnimateSpring(Transform target, Vector3 endScale, float duration, float freq, float damp, Action onComplete)
    {
        target.localScale = Vector3.zero;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float lerpVal = 1 - Mathf.Exp(-damp * t * 2) * Mathf.Cos(freq * t);

            target.localScale = endScale * lerpVal;

            yield return null;
        }

        target.localScale = endScale;
        onComplete?.Invoke();
    }

}