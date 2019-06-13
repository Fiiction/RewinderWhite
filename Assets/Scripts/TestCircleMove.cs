using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCircleMove : MonoBehaviour
{
    public float startTheta, angularVel, radius;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {

        float theta = startTheta + Time.time * angularVel;
        transform.position = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta)) * radius;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
