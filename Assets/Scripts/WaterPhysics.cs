using UnityEngine;

public class Waterphysics : MonoBehaviour
{
    public bool boyant = true;
    public float boyancy = 15f;
    public float gravity = 9.81f;
    public float waterdrag = 0.98f;
    public float waterdrag_angular = 0.98f;
    public Transform waterlevel_transform = null;
    public float wobble = 1f;
    public bool upright = true;
    
    float transition_height = 0.3f;
    private float waterlevel = 98f;
    private bool applyGravity = false;
    long nextwobble = 0;
    
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (waterlevel_transform == null)
            waterlevel_transform = transform;
        if(wobble > 0)
            // next wave in 0-1 seconds
        {
            nextwobble = System.DateTime.Now.Ticks + (long)(Random.Range(0f, 2f) * System.TimeSpan.TicksPerSecond);
        }
    }
    
    public bool Nearsurface()
    {
        return (waterlevel_transform.position.y - waterlevel) > -transition_height;
    }
    public bool Submerged()
    {
        return (waterlevel_transform.position.y - waterlevel) < 0;
    }
    
    public bool Fullysubmerged()
    {
        return (waterlevel_transform.position.y - waterlevel) < -transition_height;
    }

    private void FixedUpdate()
    {
        // [0-1] scale on how much water acts on the submarine
        // 0 = not submerged, 1 = fully submerged
        var transition_factor = 1f; 

        if ((waterlevel_transform.position.y - waterlevel) > -transition_height && (waterlevel_transform.position.y - waterlevel) < 0)
            transition_factor = -(waterlevel_transform.position.y - waterlevel) / transition_height;

        if (!Submerged())
            transition_factor = 0f;

        if (Submerged())
        {
            rb.linearVelocity *= transition_factor * waterdrag + (1 - transition_factor) * 1.0f; //drag

            if(boyant)
                rb.AddForce(transition_factor * rb.mass * boyancy * Vector2.up, ForceMode2D.Force); //buoyancy
            if (wobble > 0)
            {
                if(System.DateTime.Now.Ticks > nextwobble)
                {
                    nextwobble = System.DateTime.Now.Ticks + (long)(Random.Range(0f, 2f) * System.TimeSpan.TicksPerSecond);
                    //rb.AddForce(transition_factor * rb.mass * wobble_force, ForceMode2D.Force); //wobble
                    rb.angularVelocity += Random.Range(-wobble, wobble);
                    rb.AddForce(transition_factor * rb.mass * wobble * Vector2.up, ForceMode2D.Force); //wobble
                }
            }
                
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