using UnityEngine;

public class Arm : MonoBehaviour
{
    public Player player;
    private Rigidbody2D playerRigidbody;
    private float armLength = 2.5f;
    private float armStrength = 80f;
    private Rigidbody2D armRigidbody;
    FixedJoint2D joint;
    
    public Sprite grabberSpriteOpen;
    public Sprite grabberSpriteClosed;
    SpriteRenderer grabberSpriteRenderer;

    bool isGrabbing = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        armRigidbody = GetComponent<Rigidbody2D>();
        grabberSpriteRenderer = GetComponent<SpriteRenderer>();
        playerRigidbody = player.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(joint is not null)
        {
            if (joint.connectedBody == null)
            {
                Destroy(joint);
                joint = null;
            }
        }
        
            
        
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerPos = player.transform.position;
        Vector2 attachmentPoint = playerPos + player.armPosition;
        Vector2 dif = (mousePos - attachmentPoint);
        float length = dif.magnitude;
        if (length > armLength)
        {
            dif = dif.normalized * armLength; // Limit the length of the arm
        }

        // Rotate the arm to point towards the mouse position
        float angle = Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg;
        // Set the rotation of the arm
        float angleDiff = angle - transform.rotation.eulerAngles.z;
        armRigidbody.angularVelocity = angleDiff * armStrength * Time.deltaTime;
        //transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Set the arm position to the player's arm position plus the direction vector without breaking physics
        Vector2 targetposition = attachmentPoint + dif;
        Vector2 dif2 = targetposition - (Vector2)transform.position;
        Vector2 force = armStrength * dif2;
        if (joint is null)
            force *= 0.2f;
        armRigidbody.AddForce(force);
        playerRigidbody.AddForce(-force);
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
                        if (collider.gameObject != player.gameObject && collider.gameObject != gameObject && !collider.gameObject.CompareTag("Cage"))
                        {
                            hit = collider;
                            break;
                        }
                    }

                    if (hit != null)
                    {
                        GameObject obj = hit.gameObject;
                        joint = gameObject.AddComponent<FixedJoint2D>();
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
