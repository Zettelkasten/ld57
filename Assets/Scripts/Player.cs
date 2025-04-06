using System;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int money;
    
    Rigidbody2D playerRigidbody;
    public Vector2 armPosition = new Vector2(0, 0.45f);
    public PolygonCollider2D gearCollider;
    private float speed = 8f;
    private bool isMoving = false;

    private ParticleSystem particleSystem;
    private Waterphysics waterPhysics;

    private HingeJoint2D joint1;
    Vector2 joint1Pos;
    GameObject joint1Object;
    
    HingeJoint2D joint2;
    Vector2 joint2Pos; 
    GameObject joint2Object;
    
    HingeJoint2D joint3;
    Vector2 joint3Pos;
    GameObject joint3Object;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        particleSystem = GetComponentInChildren<ParticleSystem>();
        waterPhysics = GetComponent<Waterphysics>();
        
        
        joint1 = GetComponent<HingeJoint2D>();
        joint1Pos = joint1.anchor;
        joint1Object = joint1.connectedBody.gameObject;
        joint2 = joint1Object.GetComponent<HingeJoint2D>();
        joint2Pos = joint2.anchor;
        joint2Object = joint2.connectedBody.gameObject;
        joint3 = joint2Object.GetComponent<HingeJoint2D>();
        joint3Pos = joint3.anchor;
        joint3Object = joint3.connectedBody.gameObject;
        
        
    }

    public void fixJoints()
    {
        joint1.anchor = joint1Pos;
        joint2.anchor = joint2Pos;
        joint3.anchor = joint3Pos;
    }

    private void FixedUpdate()
    {
        // upright the player a bit
        playerRigidbody.angularVelocity *= 0.99f;
        playerRigidbody.angularVelocity -= playerRigidbody.rotation * 0.1f;
    }

    void Update()
    {
        upright();
        
        
        // A and D keys to move left and right
        float verticalSpeed = 0;
        bool moveButtonPressed = false;
        if (Input.GetKey(KeyCode.A))
        {
            verticalSpeed = -1;
            moveButtonPressed = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            verticalSpeed = 1;
            moveButtonPressed = true;
        }
        else
        {
            verticalSpeed = 0;
        }

        if (moveButtonPressed)
        {
            if(!isMoving )
            {
                isMoving = true;
                // enable emission
                if (waterPhysics.Submerged())
                {
                    var emission = particleSystem.emission;
                    emission.enabled = true;
                }
            }
        }else if (isMoving)
        {
            isMoving = false;
            // disable emission
            var emission = particleSystem.emission;
            emission.enabled = false;
        }
        
        verticalSpeed *= speed;

        if (verticalSpeed != 0)
        {
            // get objects that contact the gear collider
            var contacts = new Collider2D[5];
            var contactCount = gearCollider.Overlap(new ContactFilter2D(), contacts);
            Collider2D ground = null;
            for (int i = 0; i < contactCount; i++)
            {
                var contact = contacts[i];
                if (contact != null && contact.gameObject != playerRigidbody.gameObject &&
                    contact.gameObject != gearCollider.gameObject)
                {
                    ground = contact;
                    break;
                }
            }

            // if the player is on the ground, move the player
            if (ground != null)
            {
                float direction = transform.rotation.eulerAngles.z;
                // force in the forward direction of the player with the speed of the player
                Vector2 movespeed = new Vector2(verticalSpeed * Mathf.Cos(direction * Mathf.Deg2Rad),
                    verticalSpeed * Mathf.Sin(direction * Mathf.Deg2Rad));
                
                Vector2 speeddif = movespeed - playerRigidbody.linearVelocity;
                float traction = 1 / (1 + speeddif.magnitude);
                Vector2 moveforce = traction * 500 * speeddif;
                
                
                Vector2 forcepos = ((Vector2)transform.position) + Vector2.down * 0.2f;
                playerRigidbody.AddForceAtPosition(moveforce, forcepos, ForceMode2D.Force);
                //playerRigidbody.linearVelocity += moveforce;
                Rigidbody2D groundRigidbody = ground.attachedRigidbody;
                if (groundRigidbody != null && groundRigidbody.bodyType == RigidbodyType2D.Dynamic)
                {
                    groundRigidbody.AddForceAtPosition(-moveforce, groundRigidbody.position, ForceMode2D.Force);
                }

            }
        }
        
    }

    private void upright()
    {
        float angle = transform.rotation.eulerAngles.z;
        float targetAngle = 0;
        if (angle > 180)
        {
            targetAngle = 360 - angle;
        }
        else
        {
            targetAngle = angle;
        }
        float angleDiff = targetAngle - transform.rotation.eulerAngles.z;
        // rotate the player to the target angle
        float rotationSpeed = 0.1f;
        float rotation = angleDiff * rotationSpeed * Time.deltaTime;
        playerRigidbody.angularVelocity += rotation;
        // 
    }
}

