using UnityEngine;

public class Player : MonoBehaviour
{
    
    Rigidbody2D playerRigidbody;
    public Vector2 armPosition = new Vector2(0, 0.45f);
    public PolygonCollider2D gearCollider;
    private float speed = 5f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        upright();
        
        
        // A and D keys to move left and right
        float verticalSpeed = 0;
        if (Input.GetKey(KeyCode.A))
        {
            verticalSpeed = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            verticalSpeed = 1;
        }
        else
        {
            verticalSpeed = 0;
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

