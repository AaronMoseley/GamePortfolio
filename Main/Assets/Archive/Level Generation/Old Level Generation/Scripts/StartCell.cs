using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCell : MonoBehaviour
{
    public GameObject player;
    
    Transform spawnPoint; 
    
    // Start is called before the first frame update
    void Awake()
    {
        spawnPoint = GameObject.FindGameObjectWithTag("Spawn Point").transform;

        Instantiate(player, spawnPoint.position, spawnPoint.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
