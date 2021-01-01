using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVolCurves : MonoBehaviour
{
    public Transform target;
    public float curveScale;
    AudioSource source;

    void Start()
    {
        source = gameObject.GetComponent<AudioSource>();

        if(target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        float volume = curveScale / Vector2.Distance(target.position, gameObject.transform.position);
        source.volume = Mathf.Clamp(volume, 0, 1);
    }
}
