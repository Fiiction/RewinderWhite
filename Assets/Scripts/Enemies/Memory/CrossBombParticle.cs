using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossBombParticle : MonoBehaviour
{
    float vel = 30f;
    public Vector2 vec;

    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<DropGraphics>().fadeSpeed = 4f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3)vec * vel * Time.deltaTime;
        if (Mathf.Abs(transform.position.x) > 30f || Mathf.Abs(transform.position.y) > 20f)
            GetComponent<EnemyController>().Kill(false);
    }
}
