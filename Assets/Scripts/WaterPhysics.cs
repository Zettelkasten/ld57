using UnityEngine;

public class Waterphysics : MonoBehaviour
{
    public bool boyant = true;
    public float boyancy = 15f;
    public float gravity = 9.81f;
    public float waterdrag = 0.98f;
    public float waterdrag_angular = 0.98f;
    
    float transition_height = 0.3f;
    private float waterlevel = 95f;
    private bool applyGravity = false;
    public bool upright = true;
    
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    public bool Nearsurface()
    {
        return (transform.position.y - waterlevel) > -transition_height;
    }
    public bool Submerged()
    {
        return (transform.position.y - waterlevel) < 0;
    }
    
    public bool Fullysubmerged()
    {
        return (transform.position.y - waterlevel) < -transition_height;
    }

    private void FixedUpdate()
    {
        // [0-1] scale on how much water acts on the submarine
        // 0 = not submerged, 1 = fully submerged
        var transition_factor = 1f; 

        if ((transform.position.y - waterlevel) > -transition_height && (transform.position.y - waterlevel) < 0)
            transition_factor = -(transform.position.y - waterlevel) / transition_height;

        if (!Submerged())
            transition_factor = 0f;

        if (Submerged())
        {
            rb.linearVelocity *= transition_factor * waterdrag + (1 - transition_factor) * 1.0f; //drag

            if(boyant)
                rb.AddForce(transition_factor * rb.mass * boyancy * Vector2.up, ForceMode2D.Force); //buoyancy
        }
        if(applyGravity)
            rb.AddForce(transition_factor * rb.mass * gravity * Vector2.down, ForceMode2D.Force); //gravity
        
        //angular drag and uprighting
        if(Submerged()){
            rb.angularVelocity *= waterdrag_angular;
            if(upright)
                rb.angularVelocity -= rb.rotation * 0.1f;
        }
    }
}