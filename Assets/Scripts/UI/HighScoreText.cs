using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreText : MonoBehaviour
{
    public enum Type { Easy, Hard };
    public Type type = Type.Easy;
    float alpha = 0F;
    Color c;
    Text text;
    GameSystem GS;
    // Start is called before the first frame update
    void Start()
    {
        GS = FindObjectOfType<GameSystem>();
        text = GetComponent<Text>();
        c = text.color;
        c.a = alpha;
        text.color = c;
        text.raycastTarget = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GS.state == GameSystem.State.ArcadeHighScores)
        {
            float sc = 0f;
            if (type == Type.Hard)
                sc = GS.GetHardHighScore();
            if (type == Type.Easy)
                sc = GS.GetEasyHighScore();
            if (sc > 0f)
                text.text = sc.ToString("0.");
            else
                text.text = sc.ToString("--");
            if (alpha < 1F)
            {
                alpha += Time.deltaTime / GS.stateChangeTime;
                c.a = alpha;
                text.color = c;
            }
        }
        else
        {
            if (alpha > 0F)
            {
                alpha -= Time.deltaTime / GS.stateChangeTime;
                c.a = alpha;
                text.color = c;
            }
        }
    }
}
