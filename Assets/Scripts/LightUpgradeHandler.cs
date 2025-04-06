using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightUpgradeHandler : MonoBehaviour
{
    private Light2D myLight;
    public UpgradeManager upgradeManager;

    public float[] lightIntensities = new float[] { 1.0f, 2.0f, 3.0f, 4.0f };
    public float[] lightRanges = new float[] { 5.0f, 8.0f, 15.0f, 30.0f };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myLight = GetComponent<Light2D>();
    }

    public void HandleLightUpgradeEvent()
    {
        myLight.intensity = lightIntensities[upgradeManager.lightUpgradeLevel];
        myLight.pointLightOuterRadius = lightRanges[upgradeManager.lightUpgradeLevel];
    }

}
