using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryMeteor : MonoBehaviour
{
    Color color;
    float strength, x, dir,y,speed;
    MemoryLevelButtonEffect MLB;
    BackgroundSystem BS;

    public void Set(Color _c, float _str, float _x, float _dir,float _s,MemoryLevelButtonEffect _mlb)
    {
        color = _c;
        strength = _str;
        x = _x;
        dir = _dir;
        speed = _s;
        y = _dir * -10.5F;
        MLB = _mlb;
    }

    IEnumerator MeteorCoroutine()
    {
        speed *= Random.Range(0.8F,1.2F);
        float interval = 1F / speed;
        for(int i =1;i<=7;i++)
        {
            y += dir;
            BS.AddEffect(new BgrEffect(BgrEffect.Type.Single, color,
                strength, 1F, interval * 2.7F, new Vector2(x, y)));
            yield return new WaitForSeconds(interval);
            strength *= 0.91F;
        }
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        BS = FindObjectOfType<BackgroundSystem>();
        StartCoroutine(MeteorCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
