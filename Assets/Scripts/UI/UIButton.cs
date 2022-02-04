using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    public GameSystem.State curState, toState;
    public bool retry = false;
    public string audio = "";
    public bool isReturn = false;
    public bool isTitle = false;
    GameSystem GS;
    Image im;
    float alpha = 0;
    // Start is called before the first frame update
    void Start()
    {
        GS = FindObjectOfType<GameSystem>();
        im = GetComponent<Image>();
        im.color = new Color(1, 1, 1, 0);
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        if (GS.state == curState)
        {
            if (retry)
                GS.SetState(GS.lastGameState);
            else
                GS.SetState(toState);
            if (audio != "")
                FindObjectOfType<AudioSystem>().PlayAudio(audio);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GS.state == curState)
        {
            float targetAlpha = 1f;
            if (toState == GameSystem.State.HardLevel && !GS.GetHardUnlocked())
                targetAlpha = 0.35f;
            if (toState == GameSystem.State.Memory && !GS.GetMemoryUnlocked())
                targetAlpha = 0.35f;
            if (alpha < 1f)
            {
                alpha += Time.deltaTime / GS.stateChangeTime;
                if (alpha > 1f)
                    alpha = 1f;
                im.color = new Color(1, 1, 1, alpha * targetAlpha);
            }
            if(alpha * targetAlpha > 0.6f)
                im.raycastTarget = true;
            if(isReturn && Input.GetKeyDown(KeyCode.Escape))
            {
                OnClick();
			}
            if(isTitle && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Home)))
            {
                Application.Quit();
            }
        }
        else
        {
            float targetAlpha = 1f;
            if (toState == GameSystem.State.HardLevel && !GS.GetHardUnlocked())
                targetAlpha = 0.4f;
            if (toState == GameSystem.State.Memory && !GS.GetMemoryUnlocked())
                targetAlpha = 0.4f;
            if (alpha > 0F)
            {
                alpha -= Time.deltaTime / GS.stateChangeTime;
                im.color = new Color(1, 1, 1, alpha * targetAlpha);
            }
            im.raycastTarget = false;
        }
    }
}
