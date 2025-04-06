using UnityEngine;

public class SwimInWaterAnimation : MonoBehaviour
{
    private Vector3 originalLocalPosition = Vector3.zero;
    
    public float wobbleSpeed;
    public float wobbleStrength;
    
    void Update()
    {
        if (originalLocalPosition == Vector3.zero)
        {
            originalLocalPosition = transform.localPosition;
        }
        // wobble a bit in the waves
        var y = Mathf.Sin(Time.time * wobbleSpeed) * wobbleStrength;
        transform.localPosition = new Vector3(originalLocalPosition.x, originalLocalPosition.y + y, originalLocalPosition.z);
    }
}
