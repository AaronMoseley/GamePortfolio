using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinking : MonoBehaviour
{
    public float timeBetweenBlinks;
    public float blinkTime;
    bool blinking;
    float blinkTimer;

    public Sprite normalPlayer;
    public Sprite playerBlink;
    public Sprite crouchedPlayer;
    public Sprite crouchPlayerBlink;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        blinkTimer += Time.deltaTime;

        if (!blinking && blinkTimer >= timeBetweenBlinks)
        {
            blinking = true;
            blinkTimer = 0;

            if (gameObject.GetComponent<SpriteRenderer>().sprite == normalPlayer)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = playerBlink;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = crouchPlayerBlink;
            }
        }
        else if (blinking && blinkTimer >= blinkTime)
        {
            blinking = false;
            blinkTimer = 0;

            if (gameObject.GetComponent<SpriteRenderer>().sprite == playerBlink)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = normalPlayer;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = crouchedPlayer;
            }
        }
    }
}
