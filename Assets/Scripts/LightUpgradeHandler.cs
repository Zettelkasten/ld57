using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightUpgradeHandler : MonoBehaviour
{
    private Light2D myLight;
    public Upgrade lightUpgrade;
    public float currentIntensity = 0.0f;

    public float[] lightIntensities = new float[] { 0.0f, 1.0f, 4.0f, 8.0f };
    public float[] lightRanges = new float[] { 0.0f, 4.0f, 10.0f, 30.0f };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myLight = GetComponent<Light2D>();
    }

    public void HandleLightUpgradeEvent()
    {
        myLight.intensity = lightIntensities[UpgradeManager.Instance.GetSelectedLevelOfUpgrade(lightUpgrade)];
        myLight.pointLightOuterRadius = lightRanges[UpgradeManager.Instance.GetSelectedLevelOfUpgrade(lightUpgrade)];
        currentIntensity = lightIntensities[UpgradeManager.Instance.GetSelectedLevelOfUpgrade(lightUpgrade)];
	}

}
