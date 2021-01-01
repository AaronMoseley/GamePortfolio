using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootCollider : MonoBehaviour
{
    GameObject player;
    public AudioSource jumpEffect;
    public int groundLayer;

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
            jumpEffect.Play();
        }
    }
}
