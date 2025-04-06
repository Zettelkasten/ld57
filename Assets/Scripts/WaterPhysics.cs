using UnityEngine;
using UnityEngine.Serialization;

public class Waterphysics : MonoBehaviour
{
    public bool boyant = true;
    public float boyancy = 15f;
    public float gravity = 9.81f;
    public float waterdrag = 0.98f;
    public float waterdrag_angular = 0.98f;
    public Transform waterlevel_transform = null;
    public float wobble = 1f;
    public bool uprighting = true;
    public bool applyGravity = false;
    public float uprighting_force = 0.2f;
    public static float waterlevel = 98f;
    
    // bubbletail
    public bool bubbletail_enabled = true;
    private float bubbletailspeed = 2f;
    public Transform bubbletail_origin = null;
    private ParticleSystem bubbletail_particle = null;
    private bool bubbletail_active = false;
    public float bubbletail_base_emission = 1f;

    public bool debug = false;
    
    float transition_height = 0.3f;
    long nextwobble = 0;
    private GameObject bubbletail = null;
    
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

        if (bubbletail_enabled)
        {
            if(bubbletail_origin == null)
                bubbletail_origin = transform;
            bubbletail = Instantiate(World.Instance.Bubbletail, bubbletail_origin.position, Quaternion.identity);
            bubbletail.transform.parent = bubbletail_origin;
            bubbletail_particle = bubbletail.GetComponent<ParticleSystem>();
            bubbletail_base_emission = bubbletail_particle.emission.rateOverTime.constant;
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
            if (wobble > 0 && transition_factor < 1)
            {
                if(System.DateTime.Now.Ticks > nextwobble)
                {
                    nextwobble = System.DateTime.Now.Ticks + (long)(Random.Range(0f, 2f) * System.TimeSpan.TicksPerSecond);
                    //rb.AddForce(transition_factor * rb.mass * wobble_force, ForceMode2D.Force); //wobble
                    rb.angularVelocity += Random.Range(-wobble, wobble);
                    rb.AddForce(transition_factor * rb.mass * wobble * Vector2.up, ForceMode2D.Force); //wobble
                }
            }

            if (bubbletail_enabled)
            {
                var velocity = rb.linearVelocity;
                var speed = velocity.magnitude;
                if(speed > bubbletailspeed)
                {
                    float emission_factor = (speed - bubbletailspeed) / bubbletailspeed;
                    emission_factor = Mathf.Clamp(Mathf.Pow(emission_factor, 1.5f), 0, 10);
                    
                    var emission = bubbletail_particle.emission;
                    emission.rateOverTime = emission_factor * bubbletail_base_emission;
                    
                    if (!bubbletail_active)
                    {
                        emission.enabled = true;
                        bubbletail_active = true;
                    }
                }
                else
                {
                    if (bubbletail_active)
                    {
                        var emission = bubbletail_particle.emission;
                        emission.enabled = false;
                        bubbletail_active = false;
                    }
                }
            }
                
        }
        if(applyGravity)
            rb.AddForce(transition_factor * rb.mass * gravity * Vector2.down, ForceMode2D.Force); //gravity
        
        //angular drag and uprighting
        if(Submerged()){
            rb.angularVelocity *= waterdrag_angular;
            if (uprighting)
                rb.angularVelocity -= rb.rotation * uprighting_force;
        }
    }
}