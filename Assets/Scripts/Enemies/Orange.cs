using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orange : MonoBehaviour {
    
    public float speed,angSpeed;
    float curAngle;

    float TargetAngle()
    {
        Vector2 deltaPos = (Vector2)(GameSystem.playerPos - transform.position);
        float ret = Mathf.Atan2(deltaPos.y, deltaPos.x);
        return ret;

    }


	// Use this for initialization
	void Start ()
    {
        GameSystem.orangeCnt++;
        
        curAngle = TargetAngle();


	}

    float tar;
    void Move()
    {
        //float tar = TargetAngle(), pi = Mathf.PI;
        if (GameSystem.alive)
            tar = TargetAngle();
        if (curAngle > tar + Mathf.PI)
            curAngle += angSpeed * Time.deltaTime;
        else if (curAngle > tar)
            curAngle -= angSpeed * Time.deltaTime;
        else if (curAngle < tar - Mathf.PI)
            curAngle -= angSpeed * Time.deltaTime;
        else if (curAngle < tar)
            curAngle += angSpeed * Time.deltaTime;

        if (curAngle > Mathf.PI)
            curAngle -= 2F * Mathf.PI;
        if (curAngle < -Mathf.PI)
            curAngle += 2F * Mathf.PI;
        transform.position += (Vector3)new Vector2(Mathf.Cos(curAngle), Mathf.Sin(curAngle))
            * speed * Time.deltaTime;



    }

    // Update is called once per frame
    void Update ()
    {
        Move();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Red" 
            || collision.gameObject.tag == "Orange"
            ||collision.gameObject.tag == "Player")
            collision.gameObject.GetComponent<KillControler>().Kill();
    }

    private void OnDestroy()
    {

    }
}
