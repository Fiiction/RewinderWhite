using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueFakeBlock : MonoBehaviour
{
    public GameObject mother;
    float phase = 0;
    float maxAlpha = 0.5F, posRate = 0F,scale = 1F;
    SpriteRenderer sr;
    Color c;
    Vector3 basicPos;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        c = StandardColors.Adjust(StandardColors.COLORBLUE);
        c.a = 0F;
        sr.color = c;
        basicPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (phase < 1F)
        {
            c.a = phase * maxAlpha;
            sr.color = c;
            phase += Time.deltaTime * 2F;
        }
        else if (phase < 2F)
        {
            c.a = (2F - phase) * maxAlpha;
            sr.color = c;
            scale = 1.5F - 0.5F * phase;
            posRate = (phase - 1F) * 0.7F;
            transform.localScale = new Vector3(scale, scale);
            if (mother)
                transform.position = Vector3.Lerp(basicPos, mother.transform.position, posRate);
            phase += Time.deltaTime * 1.3F;
        }
        else
            Destroy(gameObject);

    }
}
