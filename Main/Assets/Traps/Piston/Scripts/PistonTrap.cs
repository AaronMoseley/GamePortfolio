using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistonTrap : MonoBehaviour
{
    public float pushForce;
    public float retractForce;

    public GameObject pistonBar;

    public bool pushing = false;
    public bool retracting = false;

    public float length;

    float initLoc;
    
    void Start()
    {
        initLoc = gameObject.transform.position.y;
        gameObject.transform.parent.gameObject.GetComponent<LineRenderer>().SetPosition(0, gameObject.transform.position);
    }

    
    void Update()
    {
        gameObject.transform.parent.gameObject.GetComponent<LineRenderer>().SetPosition(1, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - length));

        if (retracting && pushing)
        {
            pushing = false;
        }

        if(Input.GetKey(KeyCode.A))
        {
            pushing = true;
        }
    }

    private void FixedUpdate()
    {
        if(pushing && !retracting)
        {
            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -pushForce));
        } else if(!pushing && gameObject.transform.position.y < initLoc && retracting)
        {
            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, retractForce));
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag != "Player" && collision.gameObject != gameObject.transform.parent.gameObject)
        {
            retracting = true;
        } else if(collision.gameObject == gameObject.transform.parent.gameObject)
        {
            retracting = false;
            pistonBar.transform.localScale = new Vector2(pistonBar.transform.localScale.x, 1);
        }
    }
}
