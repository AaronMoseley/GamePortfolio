using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollManager : MonoBehaviour
{
    public GameObject[] colliders;

    //Feet, Left wall, Right wall
    public bool[] bools;

    void Start()
    {
        colliders = new GameObject[gameObject.transform.childCount];
        bools = new bool[colliders.Length];

        for(int i = 0; i < gameObject.transform.childCount; i++)
        {
            colliders[i] = gameObject.transform.GetChild(i).gameObject;
        }
    }

    private void Update()
    {
        for(int i = 0; i < bools.Length; i++)
        {
            bools[i] = colliders[i].GetComponent<Collisions>().colliding;
        }
    }
}
