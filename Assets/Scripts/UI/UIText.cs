using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIText : MonoBehaviour
{
    public GameSystem.State curState;
    GameSystem GS;
    Image im;
    float alpha = 0;
    // Start is called before the first frame update
    void Start()
    {
        GS = FindObjectOfType<GameSystem>();
        im = GetComponent<Image>();
        im.color = new Color(1, 1, 1, 0);
        im.raycastTarget = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (GS.state == curState)
        {
            if (alpha < 1F)
            {
                alpha += Time.deltaTime / GS.stateChangeTime;
                im.color = new Color(1, 1, 1, alpha);
            }
        }
        else
        {
            if (alpha > 0F)
            {
                alpha -= Time.deltaTime / GS.stateChangeTime;
                im.color = new Color(1, 1, 1, alpha);
            }
        }
    }
}
