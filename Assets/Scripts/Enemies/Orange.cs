using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orange : MonoBehaviour {
    
    public float speed,angSpeed;
    public float curAngle = -100F;
    EnemyController EC;
    bool shown = false;
    GameSystem GS;
    float lifeTime = 0F;
    float TargetAngle()
    {
        Vector2 deltaPos = (Vector2)(EC.player.transform.position - transform.position);
        float ret = Mathf.Atan2(deltaPos.y, deltaPos.x);
        return ret;
    }


	// Use this for initialization
	void Start ()
    {
        GS = FindObjectOfType<GameSystem>();
        EC = GetComponent<EnemyController>();
        Generator.orangeCnt++;
        if(curAngle <=-100F)
            curAngle = TargetAngle();
    }

    float tar;
    void Move()
    {
        float speedMultiplier = 1F, dist = 1F;
        if (EC.player)
        {
            dist = ((Vector2)(EC.player.transform.position - transform.position)).magnitude;
            if (!shown && dist < 9F)
            {
                speedMultiplier = 1F - 0.25F * (9F - dist);
                speedMultiplier = Mathf.Clamp01(speedMultiplier);
            }
            tar = TargetAngle();
        }
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
        Vector2 vel = new Vector2(Mathf.Cos(curAngle), Mathf.Sin(curAngle)) * speed * speedMultiplier;
        transform.position += (Vector3)vel * Time.deltaTime;
    }

    // Update is called once per frame
    void Update ()
    {
        if (Mathf.Abs(transform.position.y) < 10.5F)
            shown = true;
        Move();
        lifeTime += Time.deltaTime;
        if (lifeTime >= 13F && GS.autoKillEnemy)
            EC.Kill();
    }


    private void OnDestroy()
    {
        Generator.orangeCnt--;
    }
}
