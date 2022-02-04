using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FastestFinishText : MonoBehaviour
{
    public int level = 1;
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
        if (GS.state == GameSystem.State.MemoryFastestFinishes)
        {
            string str;
            if (GS.GetMemoryFastestFinish(level) > 9999f)
                str = "--";
            else
                str = GS.GetMemoryFastestFinish(level).ToString("0.##");
            text.text = str;
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
