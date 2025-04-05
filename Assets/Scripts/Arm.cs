using UnityEngine;

public class Arm : MonoBehaviour
{
    public Player player;
    private float armLength = 6.0f;
    private float armStrength = 300f;
    public Rigidbody2D armRigidbody;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        armRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
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
        armRigidbody.linearVelocity += armStrength * Time.deltaTime * dif2;
        
        

    }
}
