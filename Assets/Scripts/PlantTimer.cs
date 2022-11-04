using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantTimer
{
    public Action onTimerFinish;
    /// <summary>
    /// how long does it take for a resting piece to place
    /// </summary>
    private float plantDuration;

    /// <summary>
    /// how long can you keep moving it along the ground until it plants itself anyways
    /// </summary>
    private float forcePlantDuration;

    private Coroutine plantTimer = null;
    private Coroutine forcePlantTimer = null;
    public PlantTimer(float plantDuration = 0.5f, float forcePlantDuration = 3f)
    {
        this.plantDuration = plantDuration;
        this.forcePlantDuration = forcePlantDuration;

        onTimerFinish = new Action(() => { });
    }

    public void RestartRest(MonoBehaviour ctx)
    {
        
        if (plantTimer != null)
        {
            ctx.StopCoroutine(plantTimer);
            plantTimer = null;
        }

        plantTimer = ctx.StartCoroutine(Timer(plantDuration));
    }

    public void Start(MonoBehaviour ctx)
    {
        if (plantTimer != null) return;

        plantTimer = ctx.StartCoroutine(Timer(plantDuration));
        forcePlantTimer = ctx.StartCoroutine(Timer(forcePlantDuration));
    }

    public void Stop(MonoBehaviour ctx)
    {
        if (plantTimer != null)
        {
            ctx.StopCoroutine(plantTimer);
            plantTimer = null;
        }

        if (forcePlantTimer != null)
        {
            ctx.StopCoroutine(forcePlantTimer);
            forcePlantTimer = null;
        }
    }

    private IEnumerator Timer(float duration)
    {
        yield return new WaitForSeconds(duration);
        onTimerFinish.Invoke();
    }
}
