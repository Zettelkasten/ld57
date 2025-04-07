using System;
using UnityEngine;

public class Fish : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    
    
    public float fishSpeed = 5f;
    public float newTargetTimerMax = 2f; //maximum time to wait for a new target
    public float timeNextTarget; // time in deltatime units
    public Vector2 currenttarget;
    public Vector2 currentwaypoint;
    public float toWaypointDistance = 1f; // distance from the current target into the direction of the current waypoint
    
    
    int i_waypoint = 0;

    private float verticalRange = 10f;
    private float horizontalRange = 1f;
    
    
    public Transform[] waypoints;
    
    private Rigidbody2D fishRigidbody;
    void Start()
    {
        fishRigidbody = GetComponent<Rigidbody2D>();
        i_waypoint = 0;
        currenttarget = fishRigidbody.position;
        setTimeNextTarget();
    }

    private void FixedUpdate()
    {
        fishRigidbody.linearVelocity *= 0.98f;
        if (Time.time >= timeNextTarget)
        {
            Vector2 dist_to_waypoint = currentwaypoint - currentwaypoint;
            if (dist_to_waypoint.magnitude < 2f)
            {
                nextWayPoint();
            }
            Vector2 nexttarget = currentwaypoint + (currentwaypoint - currenttarget).normalized * toWaypointDistance;
            setTimeNextTarget();
        }
        Vector2 dist = currenttarget - fishRigidbody.position;
        if(dist.magnitude < 1f)
        {
            return;
        }
        Vector2 movespeed = dist.normalized * fishSpeed;
        Vector2 speeddif = movespeed - fishRigidbody.linearVelocity;
        float traction = 1.0f / (1.0f + speeddif.magnitude);
        fishRigidbody.AddForce(traction * fishRigidbody.mass * movespeed, ForceMode2D.Force);
    }

    private void nextWayPoint()
    {
        i_waypoint++;
        i_waypoint %= waypoints.Length;
        currenttarget = waypoints[i_waypoint].position;
    }

    private void updateAngle()
    {
        // turn into movement dir
        Vector2 velocity = fishRigidbody.linearVelocity;
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        float dif = angle - transform.rotation.eulerAngles.z;
        if (dif > 180)
        {
            dif -= 360;
        }
        else if (dif < -180)
        {
            dif += 360;
        }

        fishRigidbody.angularVelocity = dif * 0.1f;
    }

    void setTimeNextTarget()
    {
        timeNextTarget = Time.time + UnityEngine.Random.Range(0f, newTargetTimerMax);
    }
}
