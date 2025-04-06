using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Player : MonoBehaviour
{
    public int money;
    public float energy;

    public float maxEnergy;
    public float energyDecreaseFactor;
    
    Rigidbody2D playerRigidbody;
    public Vector2 armPosition = new Vector2(0, 0.45f);
    public PolygonCollider2D gearCollider;
    private float speed = 8f;
    public float floatingSpeedFactor;

    private ParticleSystem particleSystem;    
    public ParticleSystem burstParticleSystem;

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

    private Vector3 lastPosition;

    private float counterUntilRespawn = 0;
    private float respawnTime = 5f;
    
    public float jumpForce;
    public float verticalJumpForce;
    public float jumpCooldown;
    private float currentJumpCooldown;

    public int maxNumberOfJumps;
    private int currentNumberOfJumps = 0;

    public GameObject directionalChild;
    
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

        energy = maxEnergy;
    }

    public void fixJoints()
    {
        joint1.anchor = joint1Pos;
        joint2.anchor = joint2Pos;
        joint3.anchor = joint3Pos;
    }

    private void Update()
    {
        // update the direction of the player
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerPos = playerRigidbody.transform.position;
        // get the angle and rotate the directional child
        float angle = Mathf.Atan2(mousePos.y - playerPos.y, mousePos.x - playerPos.x) * Mathf.Rad2Deg;
        directionalChild.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void FixedUpdate()
    {
        // upright the player a bit
        playerRigidbody.angularVelocity *= 0.99f;
        playerRigidbody.angularVelocity -= playerRigidbody.rotation * 0.1f;

        if (lastPosition != null)
        {
            // decrease energy based on the distance moved
            float distance = Vector2.Distance(lastPosition, transform.position);
            energy -= distance * energyDecreaseFactor;
            if (energy < 0)
            {
                energy = 0;
            }
        }
        lastPosition = transform.position;  
        
        if (World.Instance.cage.state != CageState.Underwater)
        {
            energy = maxEnergy;
        }
        if (energy <= 0 && World.Instance.cage.state == CageState.Underwater)
        {
            // disable player movement
            playerRigidbody.linearVelocity = Vector2.zero;
            
            // if the player is out of energy, they respawn until the counter is up
            counterUntilRespawn += Time.deltaTime;
            if (counterUntilRespawn >= respawnTime)
            {
                // respawn the player
                World.Instance.RespawnPlayer();
                counterUntilRespawn = 0;
            }
        }
        
        currentJumpCooldown -= Time.fixedDeltaTime;
        
        // movement code
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
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space))
        {
            moveButtonPressed = true;
            verticalSpeed = 0;
        }
        else
        {
            verticalSpeed = 0;
        }

        var emission = particleSystem.emission;
        emission.enabled = moveButtonPressed && waterPhysics.Submerged();
        
        verticalSpeed *= speed;
        
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
        
        // add much slower movements if the player is in the air
        if (ground == null && waterPhysics.Submerged())
        {
            verticalSpeed *= floatingSpeedFactor;
        }

        // if the player is on the ground, move the player
        if (verticalSpeed != 0)
        {
            float direction = transform.rotation.eulerAngles.z;
            // force in the forward direction of the player with the speed of the player
            Vector2 movespeed = new Vector2(verticalSpeed * Mathf.Cos(direction * Mathf.Deg2Rad),
                verticalSpeed * Mathf.Sin(direction * Mathf.Deg2Rad));

            Vector2 speeddif = movespeed - playerRigidbody.linearVelocity;
            float traction = 1 / (1 + speeddif.magnitude);
            Vector2 moveforce = traction * 500 * speeddif;
            // hack by frithjof, we don't want the force to influence the y axis
            moveforce.y = 0;

            Vector2 forcepos = ((Vector2)transform.position) + Vector2.down * 0.2f;
            playerRigidbody.AddForceAtPosition(moveforce, forcepos, ForceMode2D.Force);
            if (ground != null)
            {
                // this code is to move the treasures that the player is moving over.
                // disabled for now
                
                // Rigidbody2D groundRigidbody = ground.attachedRigidbody;
                // if (groundRigidbody != null && groundRigidbody.bodyType == RigidbodyType2D.Dynamic)
                // {
                //     groundRigidbody.AddForceAtPosition(-moveforce, groundRigidbody.position, ForceMode2D.Force);
                // }
            }

        }

        if (ground != null)
        {
            // if the player is on the ground, reset the jump counter
            currentNumberOfJumps = maxNumberOfJumps;
        }

        // let him jump if he presses W
        if (currentNumberOfJumps > 0 && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space)) && currentJumpCooldown <= 0)
        {
            currentNumberOfJumps -= 1;
            Debug.Log("Jumping");
            float normalDirection = transform.rotation.eulerAngles.z + 90;
            // force in the forward direction of the player with the speed of the player
            Vector2 jumpDirection = new Vector2(Mathf.Cos(normalDirection * Mathf.Deg2Rad),
                Mathf.Sin(normalDirection * Mathf.Deg2Rad));
            float airFactor = waterPhysics.Submerged() ? 1 : 0.5f;
            playerRigidbody.AddForce(airFactor * (jumpForce * jumpDirection + verticalSpeed * verticalJumpForce * Vector2.right), ForceMode2D.Impulse);
            currentJumpCooldown = jumpCooldown;

            // play particle if underwater
            if (waterPhysics.Submerged())
            {
                var em = burstParticleSystem.emission;
                em.enabled = true;
                burstParticleSystem.Play();
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
        float rotation = angleDiff * rotationSpeed * Time.fixedDeltaTime;
        playerRigidbody.angularVelocity += rotation;
    }
    
    public string GetTopText()
    {
        // energy to int
        int energy = (int) this.energy;
        // depth to int
        int depth = (int) ((transform.position.y - 100) * -1);
        if (depth < 10)
        {
            depth = 0;
        }
        return energy + " ENERGY - " + depth + "m DEPTH - " + money + " GOLD";
    }
}

