using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenSplit : MonoBehaviour
{

    public float maxRad = 4F, phaseVel = 5F, angularVel = 2F, angle = 0F;
    float phase = 0F, rad;
    public Vector2 centerPos = Vector2.zero;
    EnemyController EC;
    // Start is called before the first frame update
    void Start()
    {
        EC = GetComponent<EnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        phase += phaseVel * Time.deltaTime;
        angle += angularVel * Time.deltaTime;
        if (phase >= Mathf.PI * 2F)
            EC.Kill(false);
        rad = (1 - Mathf.Cos(phase)) * 0.5F * maxRad;
        transform.position = centerPos + rad * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }
}
