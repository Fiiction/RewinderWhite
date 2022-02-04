using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackholeMissle : MonoBehaviour
{
    BlackholeMissleElem[] elems;
    EnemyController[] elemECs;
    public BlackholeBoss boss;
    public float radius = 0.5f;
    public float selfAngSpeed = 3.0f;
    public float selfAng = 0f;
    public float speed = 9f, angSpeed = 0.8f;
    public float curAngle = -100F;
    bool shown = false;
    GameSystem GS;
    float lifeTime = 0F;
    float TargetAngle()
    {
        Vector2 deltaPos = (Vector2)(GS.PlayerPos() - transform.position);
        float ret = Mathf.Atan2(deltaPos.y, deltaPos.x);
        return ret;
    }
    // Start is called before the first frame update
    void Start()
    {
        elems = GetComponentsInChildren<BlackholeMissleElem>();
        elemECs = GetComponentsInChildren<EnemyController>();
        foreach (var i in elemECs)
            i.OnKilled.AddListener(Kill);
        elemECs[0].transform.localPosition = Quaternion.Euler(new Vector3(0f, 0f, selfAng * Mathf.Rad2Deg)) * new Vector3(-radius, 0f);
        elemECs[1].transform.localPosition = Quaternion.Euler(new Vector3(0f, 0f, selfAng * Mathf.Rad2Deg)) * new Vector3(radius, 0f);
        GS = FindObjectOfType<GameSystem>();
        Generator.orangeCnt++;
        if (curAngle <= -100F)
            curAngle = TargetAngle();
    }

    float tar;
    Vector2 velNormalized;
    void Move()
    {
        float speedMultiplier = 1F, dist = 1F;
        //dist = ((Vector2)(GS.PlayerPos() - transform.position)).magnitude;
        //if (!shown && dist < 9F)
        //{
        //    speedMultiplier = 1F - 0.25F * (9F - dist);
        //    speedMultiplier = Mathf.Clamp01(speedMultiplier);
        //}
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
        velNormalized = new Vector2(Mathf.Cos(curAngle), Mathf.Sin(curAngle));
        Vector2 vel = velNormalized * speed * speedMultiplier;
        transform.position += (Vector3)vel * Time.deltaTime;
    }

    void SelfRotate()
    {
        selfAng += selfAngSpeed * Time.deltaTime;
        //elemECs[0].transform.localPosition = Quaternion.Euler(new Vector3(0f, 0f, selfAng * Mathf.Rad2Deg)) * new Vector3(-radius, 0f);
        //elemECs[1].transform.localPosition = Quaternion.Euler(new Vector3(0f, 0f, selfAng * Mathf.Rad2Deg)) * new Vector3(radius, 0f);
        elemECs[0].transform.localPosition = new Vector3(-velNormalized.y, velNormalized.x) * radius * Mathf.Sin(selfAng);
        elemECs[1].transform.localPosition = new Vector3(velNormalized.y, -velNormalized.x) * radius * Mathf.Sin(selfAng);
        //transform.Rotate(new Vector3(0f, 0f, selfAngSpeed * Mathf.Rad2Deg));
    }

    bool killed = false;
    public void Kill()
	{
        if (killed)
            return;
        killed = true;
        //score
        GS.DamageBoss(12f);
        DestroyThis();
	}


    void DestroyThis()
    {
        if (boss)
            boss.blackholeMissles.Remove(this);
        foreach (var i in elemECs)
        {
            i.transform.SetParent(null);
            i.Kill();
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        SelfRotate();
        //CheckKill();
        if (!GS.Gaming())
            DestroyThis();
    }
}
