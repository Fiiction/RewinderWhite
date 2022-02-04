using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ScoreText : MonoBehaviour
{
    public enum Type { Score,Time};
    public Type type = Type.Score;
    public List<GameSystem.State> statesAvaliable;
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
        if (statesAvaliable.Contains(GS.state))
        {
            if (type == Type.Score)
                text.text = GS.score.ToString("0.");
            if (type == Type.Time)
                text.text = GS.gameTime.ToString("0.");
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
