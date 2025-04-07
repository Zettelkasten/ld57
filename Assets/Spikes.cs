using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spikes : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    ParticleSystem particles;
    private float deathtime = 3f;
    float deathtimer = 0f;
    private Player hitplayer = null;
    private float playerenergy = 0f;
    
    
    void Start()
    {
        particles = GetComponent<ParticleSystem>();
        //randomly flip
        //the spikes
        if (Random.Range(0, 2) == 0)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.flipX = true;
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (hitplayer != null)
        {
            hitplayer.playerRigidbody.linearVelocity *= 0.9f;
            deathtimer += Time.deltaTime;
            float deathtimerpercent = deathtimer / deathtime;
            hitplayer.energy = Mathf.Lerp(playerenergy, 0f, deathtimerpercent);
            if (deathtimer >= deathtime)
            {
                hitplayer.energy = 0;
                hitplayer.playerRigidbody.linearVelocity = Vector2.zero;
                hitplayer = null;
            }
        }
    }

    //kill player on collision
    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player != null)
        {
            hitplayer = player;
            particles.Play();
            deathtimer = 0f;
            playerenergy = player.energy;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player != null)
        {
            hitplayer = null;
            deathtimer = 0f;
            particles.Stop();
        }
    }
    
    
}
