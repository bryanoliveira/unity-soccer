using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    [SerializeField]
    private float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                new Vector3(target.position.x, transform.position.y, transform.position.z),
                ref velocity,
                smoothTime
            );
        }
    }
}
