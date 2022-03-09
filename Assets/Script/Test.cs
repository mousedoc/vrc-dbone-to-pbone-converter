using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public float speed = 3f;
    public float distance = 1f;

    private Vector3 startPos;

    private void Awake()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        var offset = new Vector3(Mathf.Sin(Time.time * speed) * distance, 0f, 0f);
        transform.position = startPos + offset;
    }
}
