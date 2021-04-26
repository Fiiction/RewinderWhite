using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomber : MonoBehaviour
{
    float maxVel = 4f;
    float moveDist = 12f, curMoveDist = 0f;
    float phase = 0f;
    GameObject CrossBombPrefab;
    EnemyController EC;
    GameSystem GS;
    Vector2 moveVec;

    int maxBombCnt = 1, curBombCnt = 0;

    bool inScreen = false;

    void ResetMove()
	{
        RefreshPosition();
        phase = 0f;
        curMoveDist = 0f;
        moveVec = new Vector2(Mathf.Sign(GS.PlayerPos().x - transform.position.x),
            Mathf.Sign(GS.PlayerPos().y - transform.position.y));
        curBombCnt = 0;
        maxBombCnt = 1;
        if (GS.bossHealth < 60f)
            maxBombCnt = 2;
        if (GS.bossHealth < 30f)
            maxBombCnt = 3;
	}

    void RefreshPosition()
	{
        transform.position = new Vector3(Mathf.Round(transform.position.x),
            Mathf.Round(transform.position.y));
        curMoveDist = Mathf.Round(curMoveDist);
        phase = curMoveDist / moveDist;
	}

    int lastRem = 1;

    void CheckRushingToBomb()
	{

        if (!inScreen)
            return ;
        if (Mathf.Abs(transform.position.x - Mathf.Round(transform.position.x)) > 0.1f)
            return;

        if (phase > 0.6f)
        {
            CrossBomb[] bombs = FindObjectsOfType<CrossBomb>();
            foreach (var i in bombs)
            {
                if ((transform.position + (moveDist - curMoveDist) * (Vector3)moveVec - i.transform.position).magnitude < 0.3f)
                {
                    RefreshPosition();
                    moveVec = new Vector2(-moveVec.y, moveVec.x);
                    return;
                }
            }
        }
    }
    bool CanReboundToPlayer()
	{
        if (!inScreen)
            return false;
        if (phase < 0.2f || phase > 0.8f)
            return false;
        if (Mathf.Abs(transform.position.x - Mathf.Round(transform.position.x)) > 0.1f)
            return false;

        int rem = Mathf.RoundToInt(transform.position.x) % 2;
        if (rem == lastRem)
            return false;
		
        float f = (Mathf.Abs(transform.position.x - GS.PlayerPos().x) + 0.1f) /
            (Mathf.Abs(transform.position.y - GS.PlayerPos().y) + 0.1f);
        if (Mathf.Abs(f) < 1.25f && Mathf.Abs(f) > 0.8f && (transform.position - GS.PlayerPos()).magnitude > 3f)
		{
            lastRem = rem;
            return true;
        }
        
        return false;
	}

    void ReboundToPlayer()
    {
        RefreshPosition();
        moveVec = new Vector2(Mathf.Sign(GS.PlayerPos().x - transform.position.x),
            Mathf.Sign(GS.PlayerPos().y - transform.position.y));
    }

    void Bomb()
	{
        if (!inScreen)
            return;
        RefreshPosition();
        GameObject.Instantiate(CrossBombPrefab, transform.position, Quaternion.identity);
	}

    IEnumerator BomberCoroutine()
	{
        while(GS.bossHealth >= 00f)
		{
            ResetMove();
            yield return new WaitForSeconds(4.5f);
            Bomb();
            yield return new WaitForSeconds(0.4f);
        }
	}


    // Start is called before the first frame update
    void Start()
    {
        EC = GetComponent<EnemyController>();
        GS = FindObjectOfType<GameSystem>();
        CrossBombPrefab = Resources.Load<GameObject>("Prefabs/Enemies/Memory/CrossBomb");
        StartCoroutine(BomberCoroutine());
    }

    void Rebound()
	{
        if (!inScreen)
            return;
        RefreshPosition();
        if (Mathf.Abs(transform.position.x) >= 11f && inScreen)
            moveVec.x *= -1;
        if (Mathf.Abs(transform.position.y) >= 7f)
            moveVec.y *= -1;

        if (inScreen)
        {
            var be = new BgrEffect(BgrEffect.Type.BoundaryCircle, EC.color, 0.7F, 10F, 0.6F, (Vector2)(transform.position * 0.83F));
            FindObjectOfType<BackgroundSystem>().AddEffect(be);
        }
    }

    float vel;

    float MoveRate()
	{
        float f = Mathf.Clamp01(Mathf.Min(1f - phase, phase) * 10f);
        return Mathf.Lerp(f, 1f, 0.2f);
	}

	void Move()
	{
        if (phase >= 1f)
            return;
        vel = MoveRate() * maxVel * Time.deltaTime;
        if (curMoveDist + vel > moveDist)
            vel = moveDist - curMoveDist;

        transform.position += (Vector3)moveVec * vel;
        curMoveDist += vel;
        phase = curMoveDist / moveDist;

        if (CanReboundToPlayer())
            ReboundToPlayer();
        CheckRushingToBomb();
        if (Mathf.Abs(transform.position.x) >= 11f || Mathf.Abs(transform.position.y) >= 7f)
            Rebound();

    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(transform.position.x) <= 11f && Mathf.Abs(transform.position.y) <= 7f)
            inScreen = true;
        Move();
        if (phase > (curBombCnt + 1.0f) / (float)maxBombCnt && maxBombCnt > 1)
		{
            curBombCnt++;
            Bomb();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            var c = collision.gameObject.GetComponent<CrossBombParticle>();
            if (!c)
                return;
            if(GS.bossHealth > 60f)
                GS.DamageBoss(0.8f);
            else if (GS.bossHealth > 30f)
                GS.DamageBoss(0.6f);
            else
                GS.DamageBoss(0.4f);


        }
    }
}
