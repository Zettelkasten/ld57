using UnityEngine;

public class Player : MonoBehaviour
{
    
    Rigidbody2D playerRigidbody;
    public Vector2 armPosition = new Vector2(0, 0.45f);
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // A and D keys to move left and right
        var speed = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.A))
        {
            speed.x = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            speed.x = 1;
        }
        else
        {
            speed.x = 0;
        }
        Walk(speed, 0.1f);
    }
    
    void Walk(Vector2 speed, float strength)
    {
        var dif = speed - playerRigidbody.linearVelocity;
        var traction = 1 / (Vector3.Magnitude(dif) + 1); // the larger the speed difference, the lower the traction
        playerRigidbody.linearVelocity += traction * strength * dif;
    }
}
