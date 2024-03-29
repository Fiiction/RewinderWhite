﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BigRed : MonoBehaviour
{
    public enum State { Orange, Red, Rush, Burst};
    public State state = State.Orange;
    public bool revolving = false;
    public float basicSpeed = 5F,speed = 5F,angSpeed = 1.4F;
    public float orangeSpeed = 5f, redSpeed = 7f;
    float orangeAngle = 0F;
    public Vector2 moveVec = Vector2.right;
    float curDist = 0F, distToGo;

    const float MX = 10.85F, MY = 6.85F;
    int reboundCnt = 0;
    int swapCnt = 0;
    EnemyController EC;
    public GameObject Orange, Red;
    GameSystem GS;

    Color WarmColor()
    {
        return new Color(255f / 255f, 165f / 255f, 63f / 255f);
        //float h = Random.Range(0.96F, 1.36F);
        //if (h >= 1F)
        //    h -= 1F;
        //return Color.HSVToRGB(h, 0.75F, 0.85F);
    }

    Color ColdColor()
    {
		return new Color(110f / 255f, 91f / 255f, 132f / 255f);
		//float h = Random.Range(0.48F, 0.88F);
		//return Color.HSVToRGB(h, 0.59F, 0.5F);
	}
    Vector2 RandomBoundPos(float minL = 18F)
    {
        return GS.PlayerPos();
        if (reboundCnt == 1)
            return (new Vector2(10.5F, 0F));
        float r = Random.Range(-1F, 1F);
        Vector2 p;
        if (r < -0.6F)
            p = new Vector2(-MX, Random.Range(MY, -MY));
        else if(r<0F)
            p = new Vector2(Random.Range(-MX,MX), -MY);
        else if (r < 0.4F)
            p = new Vector2(MX, Random.Range(MY, -MY));
        else 
            p = new Vector2(Random.Range(-MX, MX), MY);

        if (((Vector2)transform.position - p).magnitude < minL)
            return RandomBoundPos(Mathf.Max(minL - 0.6F,10F));
        else
            return p;
    }

    void Rebound()
    {
        reboundCnt++;
        Vector2 targetPos = RandomBoundPos();
        curDist = 0F;
        distToGo = (targetPos - (Vector2)transform.position).magnitude;
        moveVec = (targetPos - (Vector2)transform.position).normalized;

        if(swapCnt > 1 && reboundCnt > 1)
        {
            var be = new BgrEffect(BgrEffect.Type.BoundaryCircle, EC.color, 0.7F, 10F, 0.6F, (Vector2)(transform.position * 0.83F));
            FindObjectOfType<BackgroundSystem>().AddEffect(be);
        }
    }
    bool DToGo()
    {
        return distToGo > 0F;
    }

    float emitAngle,curAngle,angle;
    void RedEmit()
    {
        emitAngle += Random.Range(0.6F,1.2F);
        if (emitAngle > 0.6F*Mathf.PI)
            emitAngle -= 0.6F * Mathf.PI;
        curAngle = Mathf.Atan2(moveVec.y, moveVec.x);
        angle = emitAngle + curAngle + 0.7F * Mathf.PI;
        Vector3 emitVec = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
        var obj = GameObject.Instantiate(Red, transform.position + 0.6F * emitVec, Quaternion.identity);
        obj.GetComponent<Red>().vel = emitVec * 0.65F;
        obj.GetComponent<EnemyController>().color = WarmColor();

    }

    IEnumerator RedEmitCoroutine()
    {
        while(state == State.Red)
        {
            float phase = curDist / (curDist + distToGo);
            if (reboundCnt % 2 == 0 &&
                (Mathf.Abs(transform.position.x) <= MX-1f || Mathf.Abs(transform.position.y) <= MY-1f))
                RedEmit();
            yield return new WaitForSeconds(Random.Range(0.45F, 0.6F));
        }
    }

    IEnumerator RedCoroutine()
    {
        swapCnt++;
        reboundCnt = 0;
        if (swapCnt > 1)
        {
            DOTween.To(() => basicSpeed, x => basicSpeed = x, 0, 0.6f);
            yield return new WaitForSeconds(1f);
            revolving = true;
            DOTween.To(() => basicSpeed, x => basicSpeed = x, redSpeed * 0.85f, 0.5f).SetEase(Ease.OutSine);
            float angle = Mathf.Atan2(transform.position.y, transform.position.x);
            for (int i = 1; i <= 3; i++)
            {
                moveVec = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                angle += 0.6667F * Mathf.PI;
                yield return new WaitForSeconds(0.7F);
            }
            revolving = false;
        }
        basicSpeed = redSpeed;
        StartCoroutine(RedEmitCoroutine());
        for(int i =1;i<=6;i++)
        {
            Rebound();
            //yield return new WaitUntil(() => distToGo<=0F);
            yield return new WaitForSeconds(0.2F);
            yield return new WaitUntil(() =>(Mathf.Abs(transform.position.x) >= MX || Mathf.Abs(transform.position.y) >= MY));
        }
        Rebound();
        SetState(State.Orange);
    }


    float TargetAngle()
    {
        Vector2 deltaPos = (Vector2)(GS.PlayerPos() - transform.position);
        float ret = Mathf.Atan2(deltaPos.y, deltaPos.x);
        return ret;
    }

    void OrangeMove()
    {
        float tar = 0F;
        if (EC.player)
            tar = TargetAngle();
        if (orangeAngle > tar + Mathf.PI)
            orangeAngle += angSpeed * Time.deltaTime;
        else if (orangeAngle > tar)
            orangeAngle -= angSpeed * Time.deltaTime;
        else if (orangeAngle < tar - Mathf.PI)
            orangeAngle -= angSpeed * Time.deltaTime;
        else if (orangeAngle < tar)
            orangeAngle += angSpeed * Time.deltaTime;

        if (orangeAngle > Mathf.PI)
            orangeAngle -= 2F * Mathf.PI;
        if (orangeAngle < -Mathf.PI)
            orangeAngle += 2F * Mathf.PI;
    }
    void OrangeEmit()
    {
        Color c = ColdColor();
        float oAngle = orangeAngle + 0.5F * Mathf.PI;
        Vector3 oVec = new Vector3(Mathf.Cos(oAngle), Mathf.Sin(oAngle));
        var obj = GameObject.Instantiate(Orange, transform.position + 0.6F * oVec, Quaternion.identity);
        obj.GetComponent<Orange>().curAngle = oAngle;
        obj.GetComponent<EnemyController>().color = c;

        oAngle = orangeAngle - 0.5F * Mathf.PI;
        oVec = new Vector3(Mathf.Cos(oAngle), Mathf.Sin(oAngle));
        obj = GameObject.Instantiate(Orange, transform.position + 0.6F * oVec, Quaternion.identity);
        obj.GetComponent<Orange>().curAngle = oAngle;
        obj.GetComponent<EnemyController>().color = c;
    }
    IEnumerator OrangeEmitCoroutine()
    {
        yield return new WaitForSeconds(Random.Range(0.8F, 1.2F));
        while (state == State.Orange)
        {
            float phase = curDist / (curDist + distToGo);
            OrangeEmit();
            yield return new WaitForSeconds(Random.Range(1.2F, 1.6F));
        }
    }
    IEnumerator OrangeCoroutine()
    {
        swapCnt++;
        DOTween.To(() => basicSpeed, x => basicSpeed = x, 0, 0.6f);
        yield return new WaitForSeconds(1f);
        revolving = true;
        float angle = 0F;
        DOTween.To(() => basicSpeed, x => basicSpeed = x, orangeSpeed * 0.7f, 0.5f).SetEase(Ease.OutSine);
        orangeAngle = Mathf.Atan2(moveVec.y, moveVec.x);
        Debug.Log("A:" + Time.time.ToString());
        while(angle<Mathf.PI*2F)
        {
            angle += Time.deltaTime * 3F;
            orangeAngle += Time.deltaTime * 3F;
            yield return 0;
        }
        StartCoroutine(OrangeEmitCoroutine());
        basicSpeed = orangeSpeed;
        Debug.Log("B:" + Time.time.ToString());
        revolving = false;
        yield return new WaitForSeconds(13F);
        SetState(State.Red);
    }
    void SetState(State s)
    {
        state = s;
        switch(s)
        {
            case State.Red:
                StartCoroutine(RedCoroutine());
                break;
            case State.Orange:
                StartCoroutine(OrangeCoroutine());
                break;

        }
    }
    // Start is called before the first frame update
    void Start()
    {
        EC = GetComponent<EnemyController>();
        GS = FindObjectOfType<GameSystem>();
        SetState(state);
        Orange = Resources.Load<GameObject>("Prefabs/Enemies/Orange");
        Red = Resources.Load<GameObject>("Prefabs/Enemies/Red");
    }

    // Update is called once per frame
    void Update()
    {
        speed += (basicSpeed - speed) * Time.deltaTime * 0.8F;
        EC.tailLength = Mathf.Min(4.6F / speed, 1f);
        switch(state)
        {
            case State.Red:
                transform.position += (Vector3)moveVec * basicSpeed * Time.deltaTime;
                if (!revolving)
                {
                    distToGo -= Time.deltaTime * basicSpeed;
                    curDist += Time.deltaTime * basicSpeed;
                }
                break;
            case State.Orange:
                moveVec = new Vector2(Mathf.Cos(orangeAngle), Mathf.Sin(orangeAngle));
                transform.position += (Vector3)moveVec * speed * Time.deltaTime;
                if (!revolving)
                    OrangeMove();
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Enemy")
        {
            var c = collision.gameObject.GetComponent<EnemyController>();
            if (c.timeAlive < 0.5F)
                return;
            GS.DamageBoss(5f);
        }
    }

}
