
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VioletRewindEffect : MonoBehaviour
{
    IEnumerator EffectCoroutine(Color color, Queue<PosState> stateQ)
    {
        Vector2 rPos = transform.position;
        PosState[] wavePos = stateQ.ToArray();
        int f = (wavePos.Length - 1) / 8;
        if (f <= 0)
        {
            yield break;
        }
        int cnt = 0;
        float r, str;
        float h, s, v;
        Color.RGBToHSV(color, out h, out s, out v);
        s = 1F;
        for (int i = wavePos.Length - 1; i >= 0; i--)
        {
            if (i % f == 0)
            {
                cnt++;
                r = Mathf.Abs(cnt - 4) * 3F + 3F;
                h += 1F / 9F;
                if (h >= 1F)
                    h -= 1F;
                var be = new BgrEffect(BgrEffect.Type.Ring, Color.HSVToRGB(h,s,v), 0.16F, r, 1F,
                    (Vector2)wavePos[i].pos + (Vector2)transform.position - rPos);
                FindObjectOfType<BackgroundSystem>().AddEffect(be);
                yield return new WaitForSeconds(0.07F);
            }
        }
        Destroy(gameObject);
    }

    public void MakeEffect(Color color, Queue<PosState> stateQ)
    {
        StartCoroutine(EffectCoroutine(color, stateQ));
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
