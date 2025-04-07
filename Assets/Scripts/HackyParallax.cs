using UnityEditor.UI;
using UnityEngine;

public class HackyParallax : MonoBehaviour
{
    public bool onlyX = true;
    public bool selfAsAnchor = true;
    public Transform parallaxCamera;
    public Transform parallaxAnchor;
    public Vector2 yLimit = new Vector2(-10000, 10000);
    public Vector2 xLimit = new Vector2(-10000, 10000);

    private Vector3 myStartPos = Vector3.zero;
    private Vector3 anchorPos = Vector3.zero;
    public float parallaxFactor = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myStartPos = transform.localPosition;
        anchorPos = (selfAsAnchor ? transform.localToWorldMatrix * myStartPos : parallaxAnchor.position);
	}

    // Update is called once per frame
    void LateUpdate()
    {
		Vector3 anchorToCam = parallaxCamera.position - anchorPos;
        // set position with parallax effect
        Vector3 shift;
        if (onlyX)
        {
			shift = new Vector3(parallaxFactor * anchorToCam.x, 0, 0);
        }
        else
        {
			shift = new Vector3(parallaxFactor * anchorToCam.x, parallaxFactor * anchorToCam.y, 0);
        }
		// clamp
		shift = new Vector3(Mathf.Clamp(shift.x, xLimit.x, xLimit.y), Mathf.Clamp(shift.y, yLimit.x, yLimit.y), 0);
        transform.localPosition = myStartPos + shift;
    }
}
