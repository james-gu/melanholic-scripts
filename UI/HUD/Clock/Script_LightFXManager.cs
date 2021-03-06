using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_LightFXManager : MonoBehaviour
{
    [SerializeField] float currentIntensity = 0.15f;
    [SerializeField] float defaultIntensity = 0.15f;
    [SerializeField] float endingIntensity = 20f;

    [SerializeField] private AnimationCurve lightCurve;
    
    [SerializeField] private Script_Game game;
    [SerializeField] private List<Light> directionalLights;
    [SerializeField] private Script_ClockManager clockManager;

    [Tooltip("Set to buffer intensity changes so we're not doing this every frame")]
    // Also gives an interesting old school affect when getting down to Danger Time.
    [SerializeField] private float bufferTime;

    private float timer;

    void OnDisable()
    {
        foreach (Light l in directionalLights)
        {
            if (l != null)  l.gameObject.SetActive(false);
        }
    }
    
    void Start()
    {
        SetDefaultIntensity();    
    }
    
    void Update()
    {
        if (game.IsInHotel())   return;
        
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            UpdateDirectionalLights();
            
            timer = bufferTime;
        }
    }

    private void UpdateDirectionalLights()
    {
        foreach (Light l in directionalLights)
        {
            if (l.type != LightType.Directional)    return;

            float lightCurvePercent = lightCurve.Evaluate(clockManager.PercentTimeElapsed);
            float intensityDelta = endingIntensity - defaultIntensity;
            float newIntensity = lightCurvePercent * intensityDelta + defaultIntensity;
            
            l.intensity = newIntensity;
            currentIntensity = l.intensity;
        }
    }

    private void SetDefaultIntensity()
    {   
        foreach (Light l in directionalLights)
        {
            if (l.type != LightType.Directional)    return;

            l.intensity = defaultIntensity;
            currentIntensity = l.intensity;
        }
    }
}
