using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiDupe : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer.Equals("Ground") && gameObject.layer.Equals("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
