using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameSystem : MonoBehaviour
{
    public float stateChangeTime = 0.6F;
    public enum State { None, Title, Menu, Arcade, Memory, Settings, 
        EasyLevel, HardLevel, Ending, MemoryLevel, MemoryWinning};
    public bool Gaming()
    {
        return state == State.EasyLevel || state == State.HardLevel || state == State.MemoryLevel;
    }
    public State state = State.Title, lastGameState;
    public float gameTime = 0F, score = 0F;
    public float bossHealth = 100F, maxBossHealth = 100F;
    public int memoryLevelIndex;
    float scoreCnt = 0F;
    Text ScoreText, TimeText;
    public Color killColor = Color.white;
    GameObject CS;
    bool endingDelay = false;
    public GameObject PlayerObj;
    public bool autoKillEnemy = false;
    public float tension = 0f;
    public Vector3 PlayerPos()
    {
        if (PlayerObj)
            return PlayerObj.transform.position;
        else
            return Vector3.zero;
    }

    public void AddScore(float r)
    {
        if (!Gaming())
            return;
        if (state == State.Memory)
            return;
        scoreCnt++;
        score += r * scoreCnt;
    }

    void PrepareLevel()
    {
        autoKillEnemy = false;
        switch (memoryLevelIndex)
        {
            case 1:
                GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Enemies/Memory/BigRed")
             , new Vector3(-22F, 0F), Quaternion.identity);
                break;
            case 2:
                GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Enemies/Memory/PongManager")
             , new Vector3(0F, 0F), Quaternion.identity);
                autoKillEnemy = true;
                break;
            case 3:
                GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Enemies/Memory/Inflator")
             , new Vector3(-36F, 6F), Quaternion.identity);
                break;
            case 4:
                GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Enemies/Memory/Snake")
             , new Vector3(-24F, 6F), Quaternion.identity);
                autoKillEnemy = true;
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Enemies/Memory/Violet")
             , new Vector3(-52F,0F), Quaternion.identity);
                GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Enemies/Memory/Violet")
             , new Vector3(52F, 0F), Quaternion.identity);
                GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Enemies/Memory/VioletGenerator")
             , new Vector3(0F, 0F), Quaternion.identity);
                break;
        }
    }


    IEnumerator SetStateCoroutine(State s)
    {
        endingDelay = true;
        if (Gaming())
            yield return new WaitForSeconds(0.0F);
        endingDelay = false;
        state = State.None;
        var bs = FindObjectOfType<BackgroundSystem>();
        if (s == State.Ending)
        {
            bs.StartCoroutine(bs.SetBgrColorCoroutine(killColor));
            yield return new WaitForSeconds(2F);
        }
        else if(s == State.MemoryWinning)
        {
            bs.StartCoroutine(bs.SetBgrColorCoroutine(Color.black));
            var pl = PlayerObj.GetComponent<Player>();
            if (pl)
                pl.Kill();
            yield return new WaitForSeconds(2F);
        }
        else
        {
            bs.StartCoroutine(bs.SetBgrColorCoroutine(Color.white));
            yield return new WaitForSeconds(stateChangeTime);
        }
        state = s;
        switch(s)
        {
            case State.EasyLevel:
            case State.HardLevel:
                autoKillEnemy = false;
                gameTime = score = 0f;
                scoreCnt = 0;
                GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Generator")
                    , Vector3.zero, Quaternion.identity);
                PlayerObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Player")
                     , Vector3.zero, Quaternion.identity);
                break;
            case State.MemoryLevel:
                gameTime = score = 0f;
                scoreCnt = 0;
                bossHealth = maxBossHealth;
                PlayerObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Player")
                     , Vector3.zero, Quaternion.identity);
                PrepareLevel();
                break;
            case State.Ending:
                break;
        }
        if (Gaming())
        {
            lastGameState = s;
            CS = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/ControllerSet")
                     , Vector3.zero, Quaternion.identity);
        }
        else if(CS)
        {
            Destroy(CS, 1F);
            CS = null;
        }

    }

    public void SetState(State s, int _memoryLevelIndex = 0)
    {
        if(_memoryLevelIndex != 0)
            memoryLevelIndex = _memoryLevelIndex;
        StartCoroutine(SetStateCoroutine(s));
    }

    // Start is called before the first frame update
    void Start()
    {
        gameTime = score = 0F;
        ScoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        TimeText = GameObject.Find("TimeText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Gaming() && !endingDelay)
        {
            gameTime += Time.deltaTime;
            if (state == State.MemoryLevel)
            {
                score = maxBossHealth - bossHealth;
                if (score > maxBossHealth)
                    score = maxBossHealth;
            }
            else
                score += Time.deltaTime * 10F;

        }
        if(state == State.MemoryLevel && bossHealth <=0F)
        {
            SetState(State.MemoryWinning);
        }

        ScoreText.text = score.ToString("0.");
        TimeText.text = gameTime.ToString("0.");
    }

}
