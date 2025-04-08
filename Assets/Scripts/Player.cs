using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering.Universal;

public class Player : MonoBehaviour
{
    public int money;
    public float energy;

    public float maxEnergy;
    public float energyDecreaseFactor;

    public Rigidbody2D playerRigidbody;
    public PolygonCollider2D gearCollider;
    private float speed = 8f;
    private float gearStrength = 1f;
    public float floatingSpeedFactor;

    private ParticleSystem particleSystem;
    public ParticleSystem burstParticleSystem;

    private Waterphysics waterPhysics;

    private Vector3 lastPosition;

    public float counterUntilRespawn = 0;
    private float respawnTime = 5f;

    public float jumpForce;
    public float verticalJumpForce;
    public float jumpCooldown;
    private float currentJumpCooldown;

    public int maxNumberOfJumps;
    private int currentNumberOfJumps = 0;

    public GameObject directionalChild;

    public GameObject[] arms;
    int activatedArm = 0;

    int [] armStrengths = { 400, 600, 800, 1200 };
    float [] armLengths = {   2f, 2.8f, 4.3f, 5f};

    int [] gearSpeeds = { 5, 8, 14, 22 };
    float [] gearStrengths = { 1, 1.5f, 2, 3 };
    
    int [] energyLevels = { 60, 120 , 240, 480 };

    public GameObject gear;
    float [] gear_upgrade_scales = {1f, 1.15f, 1.3f, 1.4f};
    public GameObject battery;
    float [] battery_upgrade_scales = {1f, 1.15f, 1.3f, 1.6f};
    
    public Upgrade armlengthupgrade;
    public Upgrade armstrengthupgrade;
    public Upgrade speedupgrade;
    public Upgrade jumpupgrade;
    public Upgrade energyupgrade;

    public LightUpgradeHandler lightUpgradeHandler;
    private float lightActiveTime;
    private string lightFlickerStates = "mmmaaammmaaammmabcdefaaaammmmabcdefmmmaaaa";  // a is dimmest, z ist lightest
    
    public float dontMoveTime = 0f;

    private float movementAudioDelay = 0;
    public AudioSource movementAudio;
    public AudioSource armGrabSound;
    public AudioSource armReleaseSound;
    public AudioSource jumpSound;

    private bool spotlightActive = false;

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

        // don't change it too suddenly,
        // iterpolate it a bit directionalChild.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        Vector3 targetRotation = new Vector3(0, 0, angle);
        directionalChild.transform.rotation = Quaternion.RotateTowards(directionalChild.transform.rotation, Quaternion.Euler(targetRotation), 360 * Time.deltaTime);

        var lightActivate = !(World.Instance.cage.state is CageState.OnShip or CageState.Sinking);
        if (!spotlightActive && lightActivate)
        {
            lightActiveTime = 0;
        }
        spotlightActive = lightActivate;
		if (spotlightActive)
        {
            var turnOffTimeDiff = 1f;
            var turningOff = counterUntilRespawn > turnOffTimeDiff;
            var targetLight = turningOff ? 0 : lightUpgradeHandler.currentIntensity;
            var states = turningOff ? "abcdefaaaammmmabcdefmmmaaaa" : lightFlickerStates;
            
            lightActiveTime += Time.deltaTime;
            // flickering according to lookup table
            int index = (int) ((turningOff ? counterUntilRespawn : lightActiveTime) * 20);
            if (index >= states.Length)
            {
                directionalChild.GetComponentInChildren<Light2D>().intensity = targetLight;
            }
            else
            {
                char flickerState = states[index];
                // a is dimmest, z is lightest
                float lightIntensity = (float) (flickerState - 'a') / ('z' - 'a');
                directionalChild.GetComponentInChildren<Light2D>().intensity = targetLight * lightIntensity;
            }
        }
        else
        {
            directionalChild.GetComponentInChildren<Light2D>().intensity = 0;
        }
        
        check_cheats();
    }

    private void check_cheats()
    {
        // arm
        if (Keyboard.current.aKey.isPressed)
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
        }
        // speed
        if (Keyboard.current.sKey.isPressed)
        {
            if(Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                setGearLevel(0);
            }
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                setGearLevel(1);
            }
            else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                setGearLevel(2);
            }
            else if (Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                setGearLevel(3);
            }
        }
        // arm strength k
        if (Keyboard.current.kKey.isPressed)
        {
            if(Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                setArmStrengthLevel(0);
            }
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                setArmStrengthLevel(1);
            }
            else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                setArmStrengthLevel(2);
            }
            else if (Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                setArmStrengthLevel(3);
            }
        }
        // energy b
        if (Keyboard.current.bKey.isPressed)
        {
            if(Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                setEnergyLevel(0);
            }
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                setEnergyLevel(1);
            }
            else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                setEnergyLevel(2);
            }
            else if (Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                setEnergyLevel(3);
            }
        }
        // jump j
        if (Keyboard.current.jKey.isPressed)
        {
            if(Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                setJumpLevel(0);
            }
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                setJumpLevel(1);
            }
            else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                setJumpLevel(2);
            }
            else if (Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                setJumpLevel(3);
            }
        }
        // gear g
        if (Keyboard.current.gKey.isPressed)
        {
            if(Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                setGearLevel(0);
            }
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                setGearLevel(1);
            }
            else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                setGearLevel(2);
            }
            else if (Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                setGearLevel(3);
            }
        }
        
        // give money on shift + m
        if (Keyboard.current.mKey.isPressed && Keyboard.current.leftShiftKey.wasPressedThisFrame)
        {
            money += 500;
        }
    }

    public void activate_arm(int i_arm)
    {
        Debug.Log("Activating arm " + i_arm);
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
        arm.GetComponentInChildren<Arm>().armLength = armLengths[i_arm];
        HingeJoint2D joint = gameObject.AddComponent<HingeJoint2D>();
        joint.anchor = arm.transform.localPosition;
        joint.connectedBody = arm.GetComponentInChildren<Arm>().Link1.GetComponent<Rigidbody2D>();
        
        activatedArm = i_arm;
    }
    
    public void setArmStrengthLevel(int level)
    {
        Debug.Log("Setting arm strength to " + armStrengths[level]);
        for(int i = 0; i < arms.Length; i++)
        {
            arms[i].GetComponentInChildren<Arm>().armStrength = armStrengths[level];
        }
    }
    
    public void setGearLevel(int level)
    {
        Debug.Log("Setting gear speed to " + gearSpeeds[level]);
        speed = gearSpeeds[level];
        
        gear.transform.localScale = new Vector3(gear_upgrade_scales[level], gear_upgrade_scales[level], 1);
    }
    
    public void setJumpLevel(int level)
    {
        Debug.Log("Setting jump level to " + level);
        maxNumberOfJumps = 1 + level;
    }
    public void setEnergyLevel(int level)
    {
        Debug.Log("Setting energy level to " + level);
        maxEnergy = energyLevels[level];
        energy = maxEnergy;
        Debug.Log("Max energy: " + maxEnergy);
        battery.transform.localScale = new Vector3(battery_upgrade_scales[level], battery_upgrade_scales[level], 1);
    }
    
    public void onArmLengthUpgradeEvent()
    {
        int current_level = UpgradeManager.Instance.GetSelectedLevelOfUpgrade(armlengthupgrade);
        activate_arm(current_level);
    }
    
    public void onArmStrengthUpgradeEvent()
    {
        int current_level = UpgradeManager.Instance.GetSelectedLevelOfUpgrade(armstrengthupgrade);
        setArmStrengthLevel(current_level);
    }
    public void onSpeedUpgradeEvent()
    {
        int current_level = UpgradeManager.Instance.GetSelectedLevelOfUpgrade(speedupgrade);
        setGearLevel(current_level);
    }
    public void onJumpUpgradeEvent()
    {
        int current_level = UpgradeManager.Instance.GetSelectedLevelOfUpgrade(jumpupgrade);
        setJumpLevel(current_level);
    }
    public void onEnergyUpgradeEvent()
    {
        int current_level = UpgradeManager.Instance.GetSelectedLevelOfUpgrade(energyupgrade);
        setEnergyLevel(current_level);
    }
    
    

    private void FixedUpdate()
    {
        if (!World.Instance.BottomUIAvailable())
        {
            // if the player is in a dialogue, don't move
            return;
        }
        dontMoveTime -= Time.fixedDeltaTime;
        if (dontMoveTime > 0)
        {
            // don't move
            return;
        }
        
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
        
        if (World.Instance.cage.state != CageState.Underwater && World.Instance.cage.state != CageState.SinkingWithoutPlayer)
        {
            energy = maxEnergy;
        }

        if (World.Instance.cage.state != CageState.OnShip && World.Instance.cage.state != CageState.Underwater && World.Instance.cage.state != CageState.SinkingWithoutPlayer)
        {
            // player cannot move in other states
            return;
        }

        if (World.Instance.cage.state == CageState.OnShip)
        {
            // if we fell 10 blocks into the water, respawn
            if (transform.position.y < Waterphysics.waterlevel - 10 && transform.position.y > Waterphysics.waterlevel - 20)
            {
                World.Instance.RespawnPlayer();
                Debug.Log("Respawning player because they fell of the ship");
            }
        }
        if (energy <= 0 && World.Instance.cage.state == CageState.Underwater)
        {
            
            // if the player is out of energy, they respawn until the counter is up
            counterUntilRespawn += Time.deltaTime;
            if (counterUntilRespawn >= respawnTime)
            {
                // respawn the player
                // create a copy of the player
                var playerCopy = Instantiate(playerRigidbody.gameObject, playerRigidbody.transform.position, playerRigidbody.transform.rotation);
                // remove its Player component
                Destroy(playerCopy.GetComponent<Player>());
                // get all Arm components recursively and remove those too
                var arms = playerCopy.GetComponentsInChildren<Arm>();
                foreach (var arm in arms)
                {
                    Destroy(arm);
                }
                // get rigid bodies and remove "simulation"
                var rigidbodies = playerCopy.GetComponentsInChildren<Rigidbody2D>();
                foreach (var rb in rigidbodies)
                {
                    rb.simulated = false;
                }
                // add a Treasure script
                var treasure = playerCopy.AddComponent<Treasure>();
                treasure.value = 10;
                // set layer to "Things"
                playerCopy.layer = LayerMask.NameToLayer("Things");
                // add component
                var attachedDialogue = playerCopy.AddComponent<AttachedDialogue>();
                attachedDialogue.trigger = DialogueTrigger.PlayWhenItemIsSold;
                attachedDialogue.dialogue = new List<string>();
                attachedDialogue.dialogue.Add("Robot:I think I have seen this robot before.");
                attachedDialogue.dialogue.Add("Robot:It looks like it was a player once.");
                attachedDialogue.dialogue.Add("Robot:Sad to see it like this.");
                
                // actually respawn
                World.Instance.RespawnPlayer();
                counterUntilRespawn = 0;
                World.Instance.aboveSea.deathDialogue.PlayDialogue();
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
            // don't want objects on layer Playerconstruction
            if (contact != null && contact.gameObject != playerRigidbody.gameObject &&
                contact.gameObject.layer != LayerMask.NameToLayer("Playerconstruction") &&
                contact.gameObject.layer != LayerMask.NameToLayer("Player"))
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
            float traction = 1 / (1 + speeddif.magnitude) * gearStrength;
            Vector2 moveforce = traction * 500 * speeddif;
            // hack by frithjof, we don't want the force to influence the y axis
            if(ground is null)
                moveforce.y = 0;

            Vector2 forcepos = ((Vector2)transform.position) + Vector2.down * 0.5f;
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
            
            // jump audio
            SoundManager.PlaySource(jumpSound);
        }
        
        // audio
        if (verticalSpeed != 0)
        {
            if (movementAudioDelay <= 0)
            {
                movementAudioDelay = 0.3f;
                movementAudio.Play();
                movementAudio.volume = 1;
                if (SoundManager.Instance != null)
                {
                    movementAudio.volume = SoundManager.Instance.MusicSource.volume;
                }
            }
        }
        else
        {
            movementAudioDelay -= Time.deltaTime;
            if (movementAudioDelay <= 0)
            {
                movementAudio.volume = 0;
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

