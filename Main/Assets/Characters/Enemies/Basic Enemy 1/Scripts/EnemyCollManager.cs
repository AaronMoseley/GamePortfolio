using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollManager : MonoBehaviour
{
    public bool colliding = false;
    public int groundLayer;
    public AudioSource jumpSound;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == groundLayer)
        {
            jumpSound.Play();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.layer == groundLayer)
        {
            colliding = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.layer == groundLayer)
        {
            colliding = false;
        }
    }
}
