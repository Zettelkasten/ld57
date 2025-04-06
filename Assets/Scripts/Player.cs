using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Player : MonoBehaviour
{
    public int money;
    public float energy;

    public float maxEnergy;
    public float energyDecreaseFactor;
    
    Rigidbody2D playerRigidbody;
    public PolygonCollider2D gearCollider;
    private float speed = 8f;
    public float floatingSpeedFactor;

    private ParticleSystem particleSystem;    
    public ParticleSystem burstParticleSystem;

    private Waterphysics waterPhysics;

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
    
    public GameObject [] arms;
    int activatedArm = 0;
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        particleSystem = GetComponentInChildren<ParticleSystem>();
        waterPhysics = GetComponent<Waterphysics>();
        energy = maxEnergy;
        //activate_arm(0);
    }
    

    private void Update()
    {
        // update the direction of the player
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerPos = playerRigidbody.transform.position;
        // get the angle and rotate the directional child
        float angle = Mathf.Atan2(mousePos.y - playerPos.y, mousePos.x - playerPos.x) * Mathf.Rad2Deg;
        directionalChild.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        check_cheats();
        Vector3 targetRotation = new Vector3(0, 0, angle);
        directionalChild.transform.rotation = Quaternion.RotateTowards(directionalChild.transform.rotation, Quaternion.Euler(targetRotation), 360 * Time.deltaTime);

    }

    private void check_cheats()
    {
        // activate arm on strg + number
        if (Keyboard.current.leftShiftKey.isPressed)
        {
            if(Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                activate_arm(0);
            }
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                activate_arm(1);
            }
            else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                activate_arm(2);
            }
            else if (Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                activate_arm(3);
            }
        }
    }

    public void activate_arm(int i_arm)
    {
        // deactivate all arms
        for (int i = 0; i < arms.Length; i++)
        {
            arms[i].gameObject.SetActive(false);
        }
        // activate the selected arm
        arms[i_arm].gameObject.SetActive(true);
        activatedArm = i_arm;
        var hingeJoint = GetComponent<HingeJoint2D>();
        if (hingeJoint != null)
        {
            Destroy(hingeJoint);
        }
        // add a hinge joint to the player
        var arm = arms[i_arm];
        HingeJoint2D joint = gameObject.AddComponent<HingeJoint2D>();
        joint.anchor = arm.transform.localPosition;
        joint.connectedBody = arm.GetComponentInChildren<Arm>().Link1.GetComponent<Rigidbody2D>();
        
        activatedArm = i_arm;
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

        if (World.Instance.cage.state == CageState.OnShip)
        {
            // if we fell 10 blocks into the water, respawn
            if (transform.position.y < Waterphysics.waterlevel - 10)
            {
                World.Instance.RespawnPlayer();
                return;
            }
        }
        if (energy <= 0 && World.Instance.cage.state == CageState.Underwater)
        {
            
            // if the player is out of energy, they respawn until the counter is up
            counterUntilRespawn += Time.deltaTime;
            if (counterUntilRespawn >= respawnTime)
            {
                // respawn the player
                World.Instance.RespawnPlayer();
                counterUntilRespawn = 0;
            }
            // disable active player movement, so we will just return
            return;
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
            if(ground is null)
                moveforce.y = 0;

            Vector2 forcepos = ((Vector2)transform.position) + Vector2.down * 0.2f;
            playerRigidbody.AddForceAtPosition(moveforce, forcepos, ForceMode2D.Force);
            if (ground is not null)
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

