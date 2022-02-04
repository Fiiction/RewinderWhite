using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameSystem : MonoBehaviour
{
    public float stateChangeTime = 0.6F;
    public enum State { None, Title, Menu, Arcade, Memory, Settings, 
        EasyLevel, HardLevel, Ending, MemoryLevel, MemoryWinning,
        ArcadeHighScores, MemoryFastestFinishes, Staff};
    public bool newRecord = false;
    public bool hardUnlocking = false, memoryUnlocking = false;
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
    Vector3 playerPos = Vector3.zero;
    AudioSystem AS;
    public Vector3 PlayerPos()
    {
        if (PlayerObj)
            playerPos = PlayerObj.transform.position;
        return playerPos;
    }

    public void AddScore(float r)
    {
        if (!Gaming())
            return;
        if (state == State.MemoryLevel)
            return;
        if (endingDelay)
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
                // In Progress...........
                GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Enemies/Memory/Snake")
             , new Vector3(-24F, 6F), Quaternion.identity);
                autoKillEnemy = true;
                break;
            case 5:
                GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Enemies/Memory/BlackholeBoss")
             , new Vector3(-24F, 0F), Quaternion.identity);
                break;
            case 6:
                GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Enemies/Memory/Bomber")
             , new Vector3(-31F, 6F), Quaternion.identity);
                autoKillEnemy = true;
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
        if (s == state || state == State.None || endingDelay)
            yield break;
        endingDelay = true;
        if (Gaming())
        {
            yield return new WaitForSeconds(0.5F);
            AS.FadeAllPianos();
        }
        endingDelay = false;
        state = State.None;
        var bs = FindObjectOfType<BackgroundSystem>();
        if (s == State.Ending)
        {
            AS.PlayAudio("ending");
            bs.StartCoroutine(bs.SetBgrColorCoroutine(killColor));
            yield return new WaitForSeconds(2F);
        }
        else if(s == State.MemoryWinning)
        {
            AS.PlayAudio("winning");
            bs.StartCoroutine(bs.SetBgrColorCoroutine(Color.black));
            if(PlayerObj)
            {
                var pl = PlayerObj.GetComponent<Player>();
                if (pl)
                    pl.Kill();
            }
            
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
                if (s == State.EasyLevel && GetEasyHighScore() < 150)
                    SetHelpTexts();
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
    public void SetHelpTexts()
    {
        UIHelpText[] helpTexts = FindObjectsOfType<UIHelpText>();
        foreach (var i in helpTexts)
            i.SetHelp();
    }
    public void SetState(State s, int _memoryLevelIndex = 0)
    {
        if(_memoryLevelIndex != 0)
            memoryLevelIndex = _memoryLevelIndex;
        StartCoroutine(SetStateCoroutine(s));
    }

    public float GetEasyHighScore()
	{
        if (!PlayerPrefs.HasKey("EasyHighScore"))
            return 0f;
        else return PlayerPrefs.GetFloat("EasyHighScore");
	}

    public void SetEasyHighScore(float s)
	{
        PlayerPrefs.SetFloat("EasyHighScore", s);
    }

    public float GetHardHighScore()
    {
        if (!PlayerPrefs.HasKey("HardHighScore"))
            return 0f;
        else return PlayerPrefs.GetFloat("HardHighScore");
    }

    public void SetHardHighScore(float s)
    {
        PlayerPrefs.SetFloat("HardHighScore", s);
    }

    public float GetMemoryFastestFinish(int l)
	{
        if (l <= 0 || l > 7)
            return 99999f;

        string str = ("MemoryFastestFinish" + l);
        if (!PlayerPrefs.HasKey(str))
            return 99999f;
        return PlayerPrefs.GetFloat(str);

    }
    public void SetMemoryFastestFinish(int l, float s)
    {
        string str = ("MemoryFastestFinish" + l);
        PlayerPrefs.SetFloat(str, s);
    }

    public bool GetHardUnlocked()
	{
        return PlayerPrefs.HasKey("HardUnlocked");
	}

    public bool GetMemoryUnlocked()
    {
        return PlayerPrefs.HasKey("MemoryUnlocked");
    }
    
    public void UnlockHard()
	{
        PlayerPrefs.SetInt("HardUnlocked", 1);
    }

    public void UnlockMemory()
    {
        PlayerPrefs.SetInt("MemoryUnlocked", 1);
    }

    public void AddMemoryLevelTotalDamage(int l, float damage)
	{
        string str = ("MemoryLevelTotalDamage" + l);
        if (!PlayerPrefs.HasKey(str))
            PlayerPrefs.SetFloat(str, 0f);
        PlayerPrefs.SetFloat(str, PlayerPrefs.GetFloat(str) + damage);
	}

    public float GetMemoryLevelTotalDamage(int l)
	{
        string str = ("MemoryLevelTotalDamage" + l);
        if (!PlayerPrefs.HasKey(str))
            return 0f;
        else return PlayerPrefs.GetFloat(str);
    }
    public bool MemoryLevelUnlocked(int l)
	{
        if (l == 1)
            return true;
        return (GetMemoryLevelTotalDamage(l - 1) >= maxBossHealth || GetMemoryFastestFinish(l-1) < 9999f);
	}
    public void PlayerDie()
    {
        newRecord = false;
        hardUnlocking = false;
        memoryUnlocking = false;
        if (state == State.EasyLevel)
		{
            if (score > GetEasyHighScore())
            {
                SetEasyHighScore(score);
                if (score >= 500)
                    newRecord = true;
                if (!GetHardUnlocked() && score >= 1500)
                {
                    UnlockHard();
                    hardUnlocking = true;
                }
            }
            SetState(State.Ending);
		}

        if (state == State.HardLevel)
        {
            if (score > GetHardHighScore())
            {
                SetHardHighScore(score);
                if (score >= 500)
                    newRecord = true;
            }
            if (!GetMemoryUnlocked() && score >= 1500)
            {
                UnlockMemory();
                memoryUnlocking = true;
            }
            SetState(State.Ending);
        }

        if(state == State.MemoryLevel)
        {
            AddMemoryLevelTotalDamage(memoryLevelIndex, score);
            SetState(State.Ending);
        }
    }
    public void BossDie()
    {
        newRecord = false;
        if (gameTime < GetMemoryFastestFinish(memoryLevelIndex))
		{
            newRecord = true;

            AddMemoryLevelTotalDamage(memoryLevelIndex, maxBossHealth);
            SetMemoryFastestFinish(memoryLevelIndex, gameTime);
        }
        SetState(State.MemoryWinning);
    }
    // Start is called before the first frame update
    void Start()
    {
        gameTime = score = 0F;
        ScoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        TimeText = GameObject.Find("TimeText").GetComponent<Text>();
        AS = FindObjectOfType<AudioSystem>();
    }
    public void DamageBoss(float dmg)
    {
        if (state != State.MemoryLevel)
            return;
        bossHealth -= dmg;
        AS.PlayAudio("bosshurt", AudioSystem.LoopType.Loop, memoryLevelIndex == 6 ? 0.2f : 1f);
        score = maxBossHealth - bossHealth;
        if (bossHealth <= 0f)
        {
            bossHealth = 0f;
            score = maxBossHealth;
            BossDie();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(Gaming() && !endingDelay)
        {
            gameTime += Time.deltaTime;
            if (state != State.MemoryLevel)
            {
                score += Time.deltaTime * 10F;
            }

        }
        if (!Gaming())
            tension = Mathf.Max(0f, tension - 0.3f * Time.deltaTime);

        if(state == State.MemoryLevel && bossHealth <=0F)
        {
            SetState(State.MemoryWinning);
        }

        ScoreText.text = score.ToString("0.");
        TimeText.text = gameTime.ToString("0.");
    }

}
