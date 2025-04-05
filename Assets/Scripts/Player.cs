using UnityEngine;

public class Player : MonoBehaviour
{
    
    Rigidbody2D playerrigidbody;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void Walk(Vector2 speed, float strength)
    {
        var dif = speed - playerrigidbody.linearVelocity;
        var traction = 1 / (Vector3.Magnitude(dif) + 1); // the larger the speed difference, the lower the traction
        playerrigidbody.linearVelocity += traction * strength * dif;
    }

}
