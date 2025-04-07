using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Fish : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public int fishtype = 0;
    //type0: deepfish
    //type1: clownfish
    public float maxSpeed = 5f;
    public float speed = 5f;
    private float newTargetTimerMax = 2f; //maximum time to wait for a new target
    public float timeNextTarget;
    private float newSpeedTimerMax = 2f; //maximum time to wait for a new target
    public float timeNextSpeed;
    public Vector2 currenttarget;
    public Vector2 currentwaypoint;
    public float toWaypointDistance = 1f; // distance from the current target into the direction of the current waypoint
    
    SpriteRenderer fishSpriteRenderer;
    
    public int i_waypoint = 0;

    private float verticalRange = 5f;
    private float horizontalRange = 0.5f;

    public bool randomizeOrder;
    
    
    public Transform[] waypoints;
    
    private Rigidbody2D fishRigidbody;

    public Sprite[] fishsprites;
    void Start()
    {
        fishRigidbody = GetComponent<Rigidbody2D>();
        i_waypoint = 0;
        if (randomizeOrder)
        {
            i_waypoint = Random.Range(0, waypoints.Length);
        }
        currenttarget = fishRigidbody.position;
        setTimeNextTarget();
        setTimeNextSpeed();
        currentwaypoint = waypoints[i_waypoint].position;
        fishSpriteRenderer = GetComponent<SpriteRenderer>();
        if (fishtype >= fishsprites.Length || fishtype < 0)
            fishtype = Random.Range(0, fishsprites.Length);
        SetFishType(fishtype);
    }

    private void SetFishType(int newfishtype)
    {
        fishSpriteRenderer.sprite = fishsprites[newfishtype];
    }

    private void FixedUpdate()
    {
        fishRigidbody.linearVelocity *= 0.95f;
        if (Time.time >= timeNextTarget)
        {
            Vector2 dist_to_waypoint = currenttarget - currentwaypoint;
            if (dist_to_waypoint.magnitude < 2f)
            {
                nextWayPoint();
            }

            Vector2 nexttarget = currenttarget + (currentwaypoint - currenttarget).normalized * toWaypointDistance;
            nexttarget += new Vector2(Random.Range(-verticalRange, verticalRange), Random.Range(-horizontalRange, horizontalRange));
            currenttarget = nexttarget;
            setTimeNextTarget();
        }
        
         //flee player
         Vector2 playerdistance = (Vector2)World.Instance.player.transform.position - fishRigidbody.position;
         float distance = playerdistance.magnitude;
         if (distance < 2)
         {
             currenttarget = fishRigidbody.position - playerdistance.normalized * 3;
             speed = Random.Range(0,maxSpeed * 2f);
         }
         
        // upright fish
        var fishrotation = fishRigidbody.rotation;
        float goal = 0;
        float dif = goal - fishrotation;
        if(dif > 180)
        {
            dif -= 360;
        }
        if (dif < -180)
        {
            dif += 360;
        }
        fishRigidbody.angularVelocity += dif * 2f;
        fishRigidbody.angularVelocity *= 0.95f;
        
        // set new speed
        
        
        if (Time.time >= timeNextSpeed)
        {
            speed = Random.Range(0f, maxSpeed);
            setTimeNextSpeed();
        }
        
        Vector2 dist = currenttarget - fishRigidbody.position;
        if(dist.magnitude < 1f)
        {
            return;
        }
        Vector2 movespeed = dist.normalized * speed;
        Vector2 speeddif = movespeed - fishRigidbody.linearVelocity;
        float traction = 1.0f / (1.0f + speeddif.magnitude) * 8;
        fishRigidbody.AddForce(traction * fishRigidbody.mass * movespeed, ForceMode2D.Force);
    }

    private void Update()
    {
        updateDirection();
    }

    private void nextWayPoint()
    {
        i_waypoint++;
        i_waypoint %= waypoints.Length;
        if (randomizeOrder)
        {
            i_waypoint = Random.Range(0, waypoints.Length);
        }
        currentwaypoint = waypoints[i_waypoint].position;
    }

    private void updateDirection()
    {
        // flip the fish
        if (fishRigidbody.linearVelocity.x >= 0)
        {
            fishSpriteRenderer.flipX = false;
        }
        else
        {
            fishSpriteRenderer.flipX = true;
        }
    }

    void setTimeNextTarget()
    {
        timeNextTarget = Time.time + UnityEngine.Random.Range(0f, newTargetTimerMax);
    }
    
    void setTimeNextSpeed()
    {
        timeNextSpeed = Time.time + UnityEngine.Random.Range(0f, newSpeedTimerMax);
    }
}
