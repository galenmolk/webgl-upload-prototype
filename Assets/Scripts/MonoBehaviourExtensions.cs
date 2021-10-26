using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MonoBehaviourExtensions
{
    private static readonly Dictionary<float, WaitForSeconds> yieldInstructions = new Dictionary<float, WaitForSeconds>();
    
    public static void Invoke(this MonoBehaviour monoBehaviour, Action action, float delay)
    {
        monoBehaviour.StartCoroutine(InvokeRoutine(action, delay));
    }

    private static IEnumerator InvokeRoutine(Action action, float delay)
    {
        yield return GetYieldInstruction(delay);
        action();
    }

    private static WaitForSeconds GetYieldInstruction(float delay)
    {
        if (yieldInstructions.TryGetValue(delay, out var yieldInstruction))
            return yieldInstruction;

        var newYieldInstruction = new WaitForSeconds(delay);
        yieldInstructions.Add(delay, newYieldInstruction);
        return newYieldInstruction;
    }
}
