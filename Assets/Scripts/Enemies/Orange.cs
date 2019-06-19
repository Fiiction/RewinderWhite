using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orange : MonoBehaviour {
    
    public float speed,angSpeed;
    float curAngle;
    EnemyController EC;

    float TargetAngle()
    {
        Vector2 deltaPos = (Vector2)(EC.player.transform.position - transform.position);
        float ret = Mathf.Atan2(deltaPos.y, deltaPos.x);
        return ret;

    }


	// Use this for initialization
	void Start ()
    {
        EC = GetComponent<EnemyController>();
        Generator.orangeCnt++;
        curAngle = TargetAngle();
    }

    float tar;
    void Move()
    {
        if (EC.player)
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
        EC.vel = new Vector2(Mathf.Cos(curAngle), Mathf.Sin(curAngle)) * speed;
        transform.position += (Vector3)EC.vel * Time.deltaTime;
    }

    // Update is called once per frame
    void Update ()
    {
        Move();
    }


    private void OnDestroy()
    {
        Generator.orangeCnt--;
    }
}
