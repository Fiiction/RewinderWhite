using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Violet : MonoBehaviour
{    
    public float speed, angSpeed;
    public float rewindTime = 2F;
    public Color color;
    float startTime;
    float curAngle;
    EnemyController EC;
    GameSystem GS;
    GameObject VioletPrefab,EffectPrefab;
    
    public Vector3 rewindPos;
    public bool starting = true;
    public Vector3 basicPos;
    public bool canRewind;
    Queue<PosState> stateQ = new Queue<PosState>();


    public bool Rewind()
    {
        if (!canRewind)
        {
            GS.bossHealth -= 10;
            if(GS.bossHealth >= 20)
            {
                var obj = GameObject.Instantiate(VioletPrefab, basicPos, Quaternion.identity);
                obj.GetComponent<Violet>().starting = false;
                obj.GetComponent<Violet>().basicPos = basicPos * 0.9F;
            }
            EC.Kill();
            return false;
        }
        else
        {
            var obj = GameObject.Instantiate(VioletPrefab, rewindPos, Quaternion.identity);
            obj.GetComponent<Violet>().starting = false;
            obj.GetComponent<Violet>().basicPos = basicPos;
            //EC.waveS = 0F;
            EC.Kill();
            var e = GameObject.Instantiate(EffectPrefab, rewindPos, Quaternion.identity);
            e.GetComponent<VioletRewindEffect>().MakeEffect(color, stateQ);
            stateQ.Clear();
            return true;
        }

    }

    float TargetAngle()
    {
        Vector2 deltaPos = (Vector2)(GS.PlayerPos() - transform.position);
        float ret = Mathf.Atan2(deltaPos.y, deltaPos.x);
        return ret;

    }


    // Use this for initialization
    void Start()
    {
        VioletPrefab = Resources.Load<GameObject>("Prefabs/Enemies/Memory/Violet");
        EffectPrefab = Resources.Load<GameObject>("Prefabs/Enemies/Memory/VioletRewindEffect");
        GS = FindObjectOfType<GameSystem>();
        startTime = Time.time;
        EC = GetComponent<EnemyController>();
        curAngle = TargetAngle();
        if (starting)
            basicPos = transform.position;
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
        Vector2 vel = new Vector2(Mathf.Cos(curAngle), Mathf.Sin(curAngle)) * speed;
        transform.position += (Vector3)vel * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        stateQ.Enqueue(new PosState(transform.position, Time.time));
        while (stateQ.Peek().tim < Time.time - rewindTime)
            stateQ.Dequeue();

        rewindPos = stateQ.Peek().pos;
        canRewind = (Time.time >= startTime + rewindTime);
    }

    private void LateUpdate()
    {
        Move();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var obj = collision.gameObject;
        if (obj.GetComponent<Violet>())
        {
            Rewind();
        }
    }

}
