using System;
using UnityEngine;

public class Arm : MonoBehaviour
{
    public Rigidbody2D userrigidbody;
    private float armLength = 2.5f;
    public float armStrength = 500f;
    private Rigidbody2D armRigidbody;
    HingeJoint2D joint;

    public GameObject Link1;
    public GameObject Link2;
    public Transform armOrigin;
    
    public Sprite grabberSpriteOpen;
    public Sprite grabberSpriteClosed;
    SpriteRenderer grabberSpriteRenderer;
    

    bool isGrabbing = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        armRigidbody = GetComponent<Rigidbody2D>();
        grabberSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!World.Instance.BottomUIAvailable())
        {
            // if the player is in a dialogue, don't move
            return;
        }
        
        if(joint is not null)
        {
            if (joint.connectedBody == null)
            {
                Destroy(joint);
                joint = null;
            }
        }
        
            
        
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 attachmentPoint = armOrigin.position;
        Vector2 dif_to_attach = (mousePos - attachmentPoint);
        Vector2 dif_to_grapper = mousePos - (Vector2)transform.position;
        float length = dif_to_attach.magnitude;
        if (length > armLength)
        {
            float newlength = Mathf.Min(length, armLength);
            dif_to_attach = dif_to_attach.normalized * newlength; // Limit the length of the arm
        }

        // Rotate the arm to point towards the mouse position
        float angle = Mathf.Atan2(dif_to_attach.y, dif_to_attach.x) * Mathf.Rad2Deg;
        float currentAngle = transform.rotation.eulerAngles.z;
        // Set the rotation of the arm
        float angleDiff = angle - currentAngle;
        if (angleDiff > 180)
            angleDiff -= 360;
        else if (angleDiff < -180)
            angleDiff += 360;
        armRigidbody.angularVelocity = angleDiff * armStrength * Time.fixedDeltaTime * 0.01f;
        armRigidbody.angularVelocity *= 0.90f; // Dampen the rotation
        //transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Set the arm position to the player's arm position plus the direction vector without breaking physics
        Vector2 force = armStrength * dif_to_grapper.normalized;
        if (joint is null)
            force *= 0.2f;
        armRigidbody.AddForce(force);
        userrigidbody.AddForce(-force);
        if (joint is not null)
        { 
            //if(joint.connectedBody.bodyType == RigidbodyType2D.Dynamic)
            //    joint.connectedBody.AddForceAtPosition(force, transform.position);
            //, transform.position);
            
        }
        
        
        
        if (Input.GetMouseButton(0))
        {
            if (!isGrabbing)
            {
                UpdateGrabberSprite(true);
                isGrabbing = true;
                if (joint is null)
                {

                    // create a joint to the object
                    float distance = 0.4f;
                    // find all colliders in the area
                    Vector2 grabberPos = transform.position;
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(grabberPos, distance);
                    // find the first collider that is not the player
                    Collider2D hit = null;
                    foreach (Collider2D collider in colliders)
                    {
                        if (collider.gameObject != userrigidbody.gameObject &&
                            collider.gameObject != gameObject &&
                            collider.gameObject.layer != LayerMask.NameToLayer("Playerconstruction") &&
                            !collider.gameObject.CompareTag("Cage"))
                        {
                            hit = collider;
                            break;
                        }
                    }

                    if (hit is not null)
                    {
                        GameObject obj = hit.gameObject;
                        joint = gameObject.AddComponent<HingeJoint2D>();
                        joint.connectedBody = obj.GetComponent<Rigidbody2D>();
                        isGrabbing = true;
                    }
                }
            }
        }else{
            // if the mouse is released, release the object
            if (isGrabbing)
            {
                UpdateGrabberSprite(false);
                isGrabbing = false;
            }
            if (joint is not null)
            {
                Destroy(joint);
                joint = null;
            }
        }
    }
    void UpdateGrabberSprite(bool closed)
    {
        if (closed)
        {
            grabberSpriteRenderer.sprite = grabberSpriteClosed;
        }
        else
        {
            grabberSpriteRenderer.sprite = grabberSpriteOpen;
        }
    }


}
