using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSound : MonoBehaviour
{

    AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (gameObject.activeSelf)
        {
            source.pitch = Random.Range(0.8f, 1.2f);
            source.Play();
        }
    }
}
