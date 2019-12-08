using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameSystem : MonoBehaviour
{
    public float stateChangeTime = 0.6F;
    public enum State { None, Title, Menu, Arcade, Memory, Settings, Easy, Hard, Ending };
    public bool Gaming()
    {
        return state == State.Easy || state == State.Hard;
    }
    public State state = State.Title, lastGameState;
    public float gameTime = 0F, score = 0F;
    float scoreCnt = 0F;
    Text ScoreText, TimeText;
    public Color killColor = Color.white;
    GameObject CS;
    public void AddScore(float r)
    {
        if (!Gaming())
            return;
        scoreCnt++;
        score += r * scoreCnt;
    }

    IEnumerator SetStateCoroutine(State s)
    {
        state = State.None;
        var bs = FindObjectOfType<BackgroundSystem>();
        if (s == State.Ending)
        {
            bs.StartCoroutine(bs.SetBgrColorCoroutine(killColor));
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
            case State.Easy:
            case State.Hard:
                gameTime = score = 0f;
                scoreCnt = 0;
                GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Generator")
                    , Vector3.zero, Quaternion.identity);
                GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Player")
                     , Vector3.zero, Quaternion.identity);
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

    public void SetState(State s) => StartCoroutine(SetStateCoroutine(s));

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
        if(Gaming())
        {
            gameTime += Time.deltaTime;
            score += Time.deltaTime * 10F;
        }

        ScoreText.text = score.ToString("0.");
        TimeText.text = gameTime.ToString("0.");
    }

}
