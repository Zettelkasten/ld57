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

    private Vector3 originalStartPos;
    
    void Start()
    {
        platform = platformRigidbody.gameObject;
        height = platform.transform.position.y - target.position.y;
        descenttime = height / maxspeed * 2;
        originalStartPos = platform.transform.position;
    }

    float get_progress()
    {
        return up_pos.y - platform.transform.position.y;
    }

    private void Update()
    {
        float distToLift = Vector2.Distance(World.Instance.player.transform.position, platform.transform.position);
        float distToSource = Vector2.Distance(World.Instance.player.transform.position, originalStartPos);
        float distToTarget = Vector2.Distance(World.Instance.player.transform.position, target.position);
        bool nextToOther = (!is_up && distToSource < 8f) || (is_up && distToTarget < 8f);
        if (direction == 0 && (distToLift < 2f || nextToOther))
        {
            World.Instance.ShowBottomText(distToLift < 2f ? "Press [E] to operate elevator" : "Press [E] to call elevator");
            if (Input.GetKeyDown(KeyCode.E) || World.Instance.CheckBottomButtonClicked())
            {
                if (direction == 0) {
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