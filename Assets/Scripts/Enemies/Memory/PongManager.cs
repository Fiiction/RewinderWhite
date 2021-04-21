using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongManager : MonoBehaviour
{
    public List<PongBall> PongBallList = new List<PongBall>();
    PongPaddle LeftPaddle, RightPaddle;
    GameObject BallPrefab, LeftPaddlePrefab, RightPaddlePrefab;
    bool initiated = false;
    public bool ballAlive = true;
    float[] paddleSpeed = { 5.4F, 6F, 7.5F, 8F, 9F, 9.5F};
    float[] ballSpeed = { 9.5F, 10F, 10F, 10F, 10F ,10F};
    int ballCnt = 0;
    GameSystem GS;
    IEnumerator ResetCoroutine()
    {
        ballAlive = false;
        if (initiated)
        {
            yield return new WaitForSeconds(1F);
            foreach (var i in PongBallList)
                i.Kill();
            PongBallList.Clear();
            LeftPaddle.Kill();
            Destroy(LeftPaddle.gameObject, 1F);
            RightPaddle.Kill();
            Destroy(RightPaddle.gameObject, 1F);
            GS.DamageBoss(16.7f);
            if (GS.bossHealth <= 0F)
                Destroy(gameObject);
        }
        else
        {
            yield return new WaitForSeconds(0.5F);
            initiated = true;
        }
        yield return new WaitForSeconds(1F);

        LeftPaddle = GameObject.Instantiate(LeftPaddlePrefab, new Vector3(-12.5F, 0F, 0F), Quaternion.identity)
            .GetComponent<PongPaddle>();
        LeftPaddle.manager = this;
        LeftPaddle.speed = paddleSpeed[ballCnt];
        RightPaddle = GameObject.Instantiate(RightPaddlePrefab, new Vector3(12.5F, 0F, 0F), Quaternion.identity)
            .GetComponent<PongPaddle>();
        RightPaddle.manager = this;
        RightPaddle.speed = paddleSpeed[ballCnt];
        var obj = GameObject.Instantiate(BallPrefab, new Vector3(-11.5F, 0F, 0F), Quaternion.identity);
        PongBallList.Add(obj.GetComponent<PongBall>());
        if (ballCnt>=2)
        {
            yield return new WaitForSeconds(0.25F);
            obj = GameObject.Instantiate(BallPrefab, new Vector3(-11.5F, 0F, 0F), Quaternion.identity);
            PongBallList.Add(obj.GetComponent<PongBall>());
            obj.GetComponent<EnemyController>().color = new Color(0.510F, 0.8F, 0.239F);
        }
        if (ballCnt >= 4)
        {
            yield return new WaitForSeconds(0.25F);
            obj = GameObject.Instantiate(BallPrefab, new Vector3(-11.5F, 0F, 0F), Quaternion.identity);
            PongBallList.Add(obj.GetComponent<PongBall>());
            //obj.GetComponent<EnemyController>().color = new Color(0.275F, 0.549F, 0.537F);
        }
        foreach (var i in PongBallList)
            i.speed = ballSpeed[ballCnt];
        ballAlive = true;
        ballCnt++;
    }

    public void Reset()
    {
        if (ballAlive)
            StartCoroutine(ResetCoroutine());
    }


    // Start is called before the first frame update
    void Start()
    {
        GS = FindObjectOfType<GameSystem>();
        BallPrefab = Resources.Load<GameObject>("Prefabs/Enemies/Memory/PongBall");
        LeftPaddlePrefab = Resources.Load<GameObject>("Prefabs/Enemies/Memory/PongLeftPaddle");
        RightPaddlePrefab = Resources.Load<GameObject>("Prefabs/Enemies/Memory/PongRightPaddle");
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        PongBall db = null;
        foreach(var i in PongBallList)
        {
            if (Mathf.Abs(i.transform.position.x) > 16.5F)
            {
                db = i;
                break;
            }
        }
        if(db)
        {
            PongBallList.Remove(db);
            db.Kill();
            foreach (var i in PongBallList)
                i.speed *= 1.25F;
            if (PongBallList.Count == 0)
                Reset();
        }

        if(!GS.Gaming())
        {
            if (LeftPaddle) LeftPaddle.Kill();
            if (RightPaddle) RightPaddle.Kill();
            foreach (var i in PongBallList)
                i.Kill();
            Destroy(gameObject);
        }
    }
}
