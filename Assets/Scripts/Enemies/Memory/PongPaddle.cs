using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongPaddle : MonoBehaviour
{
    public bool isLeft;
    public PongManager manager;
    public float speed = 4F;
    public float targetVel = 0F, curVel = 0F;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Move()
    {
        PongBall p = null;
        float minDist = 11.5F;
        if(manager.ballAlive)
        {
            foreach (var i in manager.PongBallList)
            {
                if (!i)
                    continue;
                float d = i.transform.position.x - transform.position.x;
                float v = i.vel.x;
                if (v * d >= 0F)
                    continue;
                if (!isLeft)
                    d = -d;
                if (d > 0F && d < minDist)
                {
                    minDist = d;
                    p = i;
                }
            }
        }
        if (p)
            targetVel = Mathf.Clamp(p.transform.position.y - transform.position.y, -1F, 1F);
        else
            targetVel = Mathf.Clamp( - transform.position.y, -0.7F,0.7F);
        curVel += (targetVel - curVel) * 4F * Time.deltaTime;
        transform.position += Vector3.up * speed * curVel * Time.deltaTime;
    }

    public void Kill()
    {
        Destroy(gameObject, 1F);
        EnemyController[] ECs = GetComponentsInChildren<EnemyController>();
        foreach(var i in ECs)
        {
            i.Kill();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var b = collision.gameObject.GetComponent<PongBall>();
        if (b)
            b.HorizontalRebound();
    }
}
