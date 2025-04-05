using UnityEngine;

public class Player : MonoBehaviour
{

    public Rigidbody2D rigidbody;

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
        var dif = speed - rigidbody.linearVelocity;
        var traction = 1 / (Vector3.Magnitude(dif) + 1); // the larger the speed difference, the lower the traction
        rigidbody.linearVelocity += traction * strength * dif;
    }
}
