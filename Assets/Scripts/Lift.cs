using System;
using UnityEngine;

public class Lift : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Rigidbody2D liftRigidbody;
    public Rigidbody2D platformRigidbody;
    GameObject platform;
    private Vector2 up_pos;
    public Transform target;
    public float maxspeed = 10f;
    public float height;
    private float speed;
    public int direction;
    bool is_up = true;
    private float descenttime;
    private float descenttimer = 0f;
    
    
    
    
    void Start()
    {
        platform = platformRigidbody.gameObject;
        height = platform.transform.position.y - target.position.y;
        descenttime = height / maxspeed * 2;
    }

    float get_progress()
    {
        return up_pos.y - platform.transform.position.y;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        float player_dist_to_lift = Vector2.Distance(World.Instance.player.transform.position, platform.transform.position);
        if (Input.GetKeyDown(KeyCode.E) && player_dist_to_lift < 2f)
        {
            if (direction == 0){
                // start moving
                if (is_up)
                {
                    direction = -1;
                }
                else
                {
                    direction = 1;
                }
                descenttimer = 0f;
                
            }
        }
        if(direction != 0)
        {
            descenttimer += Time.deltaTime;
            if (descenttimer >= descenttime)
            {
                if(direction == 1)
                {
                    is_up = true;
                }
                else
                {
                    is_up = false;
                }
                direction = 0;
                descenttimer = 0f;
                speed = 0f;
            }
            else
            {
                float progress = descenttimer/descenttime;
                progress *= 2;
                if (progress > 1)
                {
                    progress = 2 - progress;
                }
                speed = maxspeed * progress;
            }
            liftRigidbody.linearVelocity = new Vector2(0, speed * direction);
        }
    }
}