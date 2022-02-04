using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMemoryLevelButton : MonoBehaviour
{
    GameSystem GS;
    public int index;
    public string audio;
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
        Debug.Log(gameObject.name + ": " + GS.state);
        if (GS.state == GameSystem.State.Memory)
        {
            GS.SetState(GameSystem.State.MemoryLevel, index);
            if (audio != "")
                FindObjectOfType<AudioSystem>().PlayAudio(audio);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GS.state == GameSystem.State.Memory)
        {
            float targetAlpha = 1f;
            if (!GS.MemoryLevelUnlocked(index))
                targetAlpha = 0.35f;
            if (alpha < 1F)
            {
                alpha += Time.deltaTime / GS.stateChangeTime;
                im.color = new Color(1, 1, 1, alpha * targetAlpha);
            }
            if(alpha* targetAlpha > 0.6f)
                im.raycastTarget = true;
        }
        else
        {
            float targetAlpha = 1f;
            if (!GS.MemoryLevelUnlocked(index))
                targetAlpha = 0.35f;
            if (alpha > 0F)
            {
                alpha -= Time.deltaTime / GS.stateChangeTime;
                im.color = new Color(1, 1, 1, alpha * targetAlpha);
            }
            im.raycastTarget = false;
        }
    }
}
