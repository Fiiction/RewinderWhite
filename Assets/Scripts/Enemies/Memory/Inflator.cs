using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inflator : MonoBehaviour
{
    public enum State { Chase, Revolve };
    public State state = State.Chase;
    public float moveAngle = 0F, moveSpeed = 6F, revolveDir = 1F, chaseLength, chaseDist = 0F;
    float spinDir = 1F, spinSpeed = 1F, health;
    public bool firstChase = true, playerIn = false, ending = false, endingChase = false;
    int revolveCnt = 0;
    GameObject PL;
    GameSystem GS;

    public const float iv = 1F;
    public float size = 0F, basicSize = 10F, targetSize = 10F, radMultiplier = 1F, curRad = 0F;
    public int elemCnt = 0;
    public float theta = 0F;

    void SetState(State s)
    {
        if(s == State.Revolve && endingChase)
        {
            ending = true;
            StartCoroutine(EndingCoroutine());
            return;
        }
        state = s;

        switch (s)
        {
            case State.Chase:
                chaseDist = 0F;
                if (firstChase)
                {
                    chaseLength = Mathf.Abs(transform.position.x);
                    moveAngle = 0F;
                }
                else
                {
                    if (!endingChase)
                        chaseLength = (PL.transform.position - transform.position).magnitude + 6F;
                    else
                    {
                        chaseLength = transform.position.magnitude - 1F;
                    }
                }
                break;
            case State.Revolve:
                revolveCnt++;
                firstChase = false;
                spinDir = -spinDir;
                Vector3 v = PL.transform.position - transform.position;
                float ang = Mathf.Atan2(v.y, v.x);
                float d = ang - moveAngle;
                if (d > 0 && d < Mathf.PI)
                    revolveDir = -1F;
                else if (d > Mathf.PI)
                    revolveDir = 1F;
                else if (d > -Mathf.PI)
                    revolveDir = 1F;
                else
                    revolveDir = -1F;
                break;
        }
    }

    public Stack<InflElement> ElemStack = new Stack<InflElement>();
    GameObject ElementPrefab;

    void AddElem()
    {
        var obj = GameObject.Instantiate(ElementPrefab, transform).GetComponent<InflElement>();
        elemCnt++;
        obj.index = elemCnt;
        obj.mother = this;
        //obj.transform.localPosition = obj.InitPos();
        ElemStack.Push(obj);
        //Debug.Break();
    }

    void DeElem()
    {
        elemCnt--;
        var obj = ElemStack.Pop();
        obj.Kill();
    }


    // Start is called before the first frame update
    void Start()
    {
        GS = FindObjectOfType<GameSystem>();
        ElementPrefab = Resources.Load<GameObject>("Prefabs/Enemies/Memory/InflElement");
        PL = FindObjectOfType<Player>().gameObject;
        SetState(State.Chase);
        StartCoroutine(CheckEnemiesCoroutine());
    }
    void Move()
    {
        switch (state)
        {
            case State.Chase:
                radMultiplier -= radMultiplier * Time.deltaTime * 0.8F;
                if (!firstChase)
                {
                    if (playerIn)
                        moveSpeed += Mathf.Clamp(8F - moveSpeed, 0F, 6F) * Time.deltaTime * 0.4F;
                    else
                        moveSpeed += Mathf.Clamp(10F - moveSpeed, 0F, 6F) * Time.deltaTime * 0.4F;
                }
                chaseDist += moveSpeed * Time.deltaTime;
                if(endingChase && chaseDist >=chaseLength)
                {
                    SetState(State.Revolve);
                    break;
                }
                if(!endingChase)
                {
                    if (!playerIn && chaseDist >= chaseLength)
                        SetState(State.Revolve);
                    if (!firstChase && chaseDist > 5F &&
                        (Mathf.Abs(transform.position.x) >= 13F || Mathf.Abs(transform.position.y) >= 9F))
                        SetState(State.Revolve);
                    if (playerIn && chaseDist > 4F &&
                        (Mathf.Abs(transform.position.x) >= 10F || Mathf.Abs(transform.position.y) >= 6F))
                        SetState(State.Revolve);
                }
                break;
            case State.Revolve:
                moveAngle += 2F * revolveDir * Time.deltaTime;
                if (moveAngle > Mathf.PI) moveAngle -= Mathf.PI * 2F;
                if (moveAngle < -Mathf.PI) moveAngle += Mathf.PI * 2F;
                moveSpeed -= Mathf.Clamp(moveSpeed - 1F, 0F, 4F) * Time.deltaTime * 1.1F;
                radMultiplier += Mathf.Clamp(1F - radMultiplier, 0F, 0.5F) * Time.deltaTime * 0.5F;

                Vector3 v;
                if(GS.bossHealth > 1F)
                    v = PL.transform.position - transform.position;
                else
                {
                    endingChase = true;
                    v = -transform.position;
                }
                float ang = Mathf.Atan2(v.y, v.x);
                if (Mathf.Abs(ang - moveAngle) < 0.05F)
                    SetState(State.Chase);
                break;
        }
        radMultiplier = Mathf.Pow(moveSpeed / 5F, -0.6F);

        Vector3 vec = new Vector3(Mathf.Cos(moveAngle), Mathf.Sin(moveAngle));
        transform.position += vec * moveSpeed * Time.deltaTime;
    }
    public void Damage()
    {
        if (playerIn)
            GS.bossHealth -= Mathf.Min(4F,GS.bossHealth-1F);
        else
            GS.bossHealth -= Mathf.Min(8F, GS.bossHealth - 1F);
    }

    IEnumerator EndingCoroutine()
    {
        Color c = new Color(0.8396F, 0.8396F, 0F);
        BgrEffect be = new BgrEffect(BgrEffect.Type.Focus, c, 0.7F, 32F, 4F, transform.position);
        FindObjectOfType<BackgroundSystem>().AddEffect(be);
        spinSpeed = 2F;
        targetSize *= 1.6F;

        float maxSize = targetSize;
        bool fl = false;
        for(float i = 0F; i <= 3.5F;i+=Time.deltaTime)
        {
            spinSpeed -= spinSpeed * 0.5F * Time.deltaTime;
            yield return 0;
            if(!fl && i >= 2.5F)
            {
                Debug.Log("!!!");
                be = new BgrEffect(BgrEffect.Type.Focus, c, 0.7F, 32F, 4F, transform.position);
                FindObjectOfType<BackgroundSystem>().AddEffect(be);
                fl = true;
            }
        }

        spinSpeed = 0F;
        for(float phase = 1F; phase > 0.02F; phase -= 0.18F * Time.deltaTime)
        {
            spinSpeed += (3F-spinSpeed) * 0.3F * Time.deltaTime;
            targetSize = phase *  maxSize;
            yield return 0;
        }

        be = new BgrEffect(BgrEffect.Type.Ring, c, 0.7F, 24F, 2F, transform.position);
        FindObjectOfType<BackgroundSystem>().AddEffect(be);
        spinDir *= -1F;
        foreach (var i in ElemStack)
            i.Kill();
        yield return new WaitForSeconds(0.8F);
        GS.bossHealth = 0F;
    }
    int endingPhase = 1;
    // Update is called once per frame
    IEnumerator CheckEnemiesCoroutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.3F);
            CheckEnemies();
        }
    }
    void CheckEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        EnemyController ec;
        foreach(var i in enemies)
            if((i.GetComponent<Red>()||i.GetComponent<Orange>())
                && (i.transform.position - transform.position).magnitude < 0.2F * curRad
                &&  i.GetComponent<EnemyController>())
            {
                ec = i.GetComponent<EnemyController>();
                if (ec.timeAlive <= 0.5F)
                    ec.Kill();
            }
    }
    void Update()
    {
        if (!GS.Gaming())
        {
            transform.DetachChildren();
            Destroy(gameObject);
        }

        playerIn = ((PL.transform.position - transform.position).magnitude < 0.2F * curRad);
        if (ending)
        {
            moveSpeed -= Mathf.Clamp(moveSpeed, 0F, 4F) * Time.deltaTime * 6F;
            Vector3 vec = new Vector3(Mathf.Cos(moveAngle), Mathf.Sin(moveAngle));
            transform.position += vec * moveSpeed * Time.deltaTime;
            theta += Mathf.Clamp(36F / (size + 6F) * spinSpeed * spinDir, -1F, 1F) * Time.deltaTime;
            size += (targetSize - size) * 5F * Time.deltaTime;
            curRad += Mathf.Clamp((size * radMultiplier - curRad), -5, 5F) * Time.deltaTime * 4F;
        }
        else
        {
            health = GS.bossHealth / GS.maxBossHealth;
            if (playerIn)
                basicSize = 10F + Mathf.Pow((1 - health), 1F) * 40F;
            else
                basicSize = 10F + Mathf.Pow((1 - health), 1F) * 48F;

            targetSize += Mathf.Clamp((basicSize - targetSize), -3F, 3F) * 1.8F * Time.deltaTime;

            size += Mathf.Clamp((targetSize - size), -3F, 3F) * 3F * Time.deltaTime;
            theta += 32F / (size + 6F) * spinSpeed * spinDir * Time.deltaTime;
            spinSpeed = 1F * Mathf.Pow(radMultiplier, -0.8F);

            if (size >= elemCnt + 1)
                AddElem();

            curRad += Mathf.Clamp((size * radMultiplier - curRad), -2, 2F) * Time.deltaTime * 4F;
            Move();
        }
    }
}
