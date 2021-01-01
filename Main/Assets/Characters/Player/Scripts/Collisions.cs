using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collisions : MonoBehaviour
{
    public string collName;
    public bool colliding = false;

    void Start()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag != "Player" && !collision.isTrigger && !collision.gameObject.GetComponent<Hook>())
        {
            colliding = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player" && !collision.isTrigger && !collision.gameObject.GetComponent<Hook>())
        {
            StartCoroutine(EndColl());
        }
    }

    IEnumerator EndColl()
    {
        yield return new WaitForSecondsRealtime(0.05f);
        colliding = false;
    }
}
