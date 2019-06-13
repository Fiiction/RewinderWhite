using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropGraphics : MonoBehaviour
{
    TrailRenderer tail, line;
    SpriteRenderer sr;
    public Color basicColor;
    float fadeRate = 1F;
    bool fading = false;
    Vector2 fadeVel;

    private void Awake()
    {
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
    }

    public void Fade(Vector2 fv)
    {
        transform.SetParent(null);
        fadeVel = fv;
        fading = true;
    }
    public void Fade() => Fade(Vector2.zero);

    // Start is called before the first frame update
    void Start()
    {
        tail = transform.Find("Tail").GetComponent<TrailRenderer>();
        line = transform.Find("Line").GetComponent<TrailRenderer>();
        sr = GetComponent<SpriteRenderer>();
    }

    Gradient SetGradient(Color color)
    {
        Gradient ret = new Gradient();
        GradientColorKey[] colorKey = new GradientColorKey[1];
        colorKey[0].color = color;
        colorKey[0].time = 0.0f;
        GradientAlphaKey[] alphaKey = new GradientAlphaKey[1];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        ret.SetKeys(colorKey, alphaKey);
        return ret;
    }
    public void SetColor(Color c)
    {
        basicColor = c;
        UpdateColor();
    }
    void UpdateColor()
    {
        Color color = Color.Lerp(basicColor, Color.white, fadeRate);
        Gradient gra = SetGradient(color);
        sr.color = color;
        line.colorGradient = gra;
        tail.colorGradient = gra;
    }

    // Update is called once per frame
    void Update()
    {
        if(fading)
        {
            fadeRate += Time.deltaTime * 1.6F;
            UpdateColor();
            if (fadeRate >= 1F)
                Destroy(gameObject);
        }
        else
        {
            if(fadeRate > 0F)
            {
                fadeRate -= Time.deltaTime * 1.0F;
                UpdateColor();
            }
        }
    }
}
