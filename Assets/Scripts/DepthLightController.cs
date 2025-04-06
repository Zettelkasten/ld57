using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DepthLightController : MonoBehaviour
{
    public float oceanSurface = 100;
    public float darkestDepth = 0;

    public Light2D sceneLight;

    public UpgradeManager upgradeManager;

    private float maxSceneLightStrength = 1.0f;

    private int testLock = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxSceneLightStrength = sceneLight.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        float depthFactor = Mathf.Clamp((transform.position.y - darkestDepth) / (oceanSurface - darkestDepth), 0, 1);
        sceneLight.intensity = Mathf.Lerp(0, maxSceneLightStrength, depthFactor);
        if (depthFactor < 0.75 && testLock <= 0)
        {
            testLock++;
			upgradeManager.IncreaseLightsLevel();
        }
        else if (depthFactor < 0.5 && testLock <= 1)
        {
			testLock++;
			upgradeManager.IncreaseLightsLevel();
		}
        else if (depthFactor < 0.25 && testLock <= 2)
        {
			testLock++;
			upgradeManager.IncreaseLightsLevel();
		}
	}
}
