using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DepthLightController : MonoBehaviour
{
    public float oceanSurface = 100;
    public float darkestDepth = 0;

    public Light2D sceneLight;

    private float maxSceneLightStrength = 1.0f;

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
	}
}
