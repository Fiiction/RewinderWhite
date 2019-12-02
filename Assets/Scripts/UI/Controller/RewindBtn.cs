using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindBtn : MonoBehaviour
{
    Player player;
    GameSystem GS;
    float alpha = 0F;
    SpriteRenderer SR;
    Color c;
    // Start is called before the first frame update
    void Start()
    {
        Input.simulateMouseWithTouches =
        player = FindObjectOfType<Player>();
        GS = FindObjectOfType<GameSystem>();
        SR = GetComponent<SpriteRenderer>();
        c = SR.color;
    }
    void Click()
    {
        if (player.Rewind())
            alpha = 0F;
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var i in Input.touches)
        {
            if(i.phase == TouchPhase.Began)
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(i.position);
                pos.z = 0F;
                if ((pos - transform.position).magnitude <= 8F)
                    Click();
            }
        }

        if(!player)
            player = FindObjectOfType<Player>();
        if (GS.Gaming() && alpha < 1F)
            alpha += Time.deltaTime / player.rewindTime;
        if (!GS.Gaming())
        {
            alpha -= Time.deltaTime;
        }
        c.a = alpha;
        SR.color = c;
    }

}
