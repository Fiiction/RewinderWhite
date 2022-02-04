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
    public bool isNewRecord = false;
    public bool isHardUnlocking = false;
    public bool isMemoryUnlocking = false;
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
        bool flag = GS.state == curState;
        if (isNewRecord && (!GS.newRecord))
            flag = false;
        if(isHardUnlocking && !GS.hardUnlocking)
            flag = false;
        if (isMemoryUnlocking && !GS.memoryUnlocking)
            flag = false;

        if (flag)
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
