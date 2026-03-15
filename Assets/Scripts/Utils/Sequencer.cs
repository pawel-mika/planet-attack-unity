using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public static class CoroutineExtensions
{
    public static void RunWithDelay(this MonoBehaviour runner, System.Action action, float baseDelay, float variance = 0f)
    {
        float finalDelay = baseDelay + Random.Range(-baseDelay * variance, baseDelay * variance);

        if (finalDelay <= 0)
        {
            action.Invoke();
        }
        else
        {
            runner.StartCoroutine(WaitAndRun(finalDelay, action));
        }
    }

    private static IEnumerator WaitAndRun(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }
}