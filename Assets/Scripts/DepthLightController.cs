using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DepthLightController : MonoBehaviour
{
    public float oceanSurface = 100;
    public float darkestDepth = 0;

    public Light2D sceneLight;

    public Light2D playerAuraLight;

    private float maxSceneLightStrength = 1.0f;
    private float maxPlayerAuraLightStrength = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxSceneLightStrength = sceneLight.intensity;
        maxPlayerAuraLightStrength = playerAuraLight.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        float depthFactor = Mathf.Clamp((transform.position.y - darkestDepth) / (oceanSurface - darkestDepth), 0, 1);
        sceneLight.intensity = Mathf.Lerp(0, maxSceneLightStrength, depthFactor);

		float playerAuroFactor = Mathf.Clamp((oceanSurface - 30 - transform.position.y) / 50, 0, 1);
        playerAuraLight.intensity = Mathf.Lerp(0, maxPlayerAuraLightStrength, playerAuroFactor);
	}
}
