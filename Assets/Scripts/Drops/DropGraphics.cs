using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropGraphics : MonoBehaviour
{
    TrailRenderer tail, line;
    public float tailLength = 1.0F, lineWidth = 1.0F, lineLength = 1.0F;
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
        tail.time = 0.15F * tailLength;
        line.time = 1.8F * lineLength;
        line.widthMultiplier = 0.02F * lineWidth;
    }

    Gradient SetGradient(Color color)
    {
        Gradient ret = new Gradient();
        GradientColorKey[] colorKey = new GradientColorKey[1];
        colorKey[0].color = color;
        colorKey[0].time = 0.0f;
        GradientAlphaKey[] alphaKey = new GradientAlphaKey[1];
        alphaKey[0].alpha = 1.0f - fadeRate;
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
        //Color color = Color.Lerp(basicColor, Color.white, fadeRate);
        Color color = basicColor;
        color.a = 1.0F - fadeRate;
        Gradient gra = SetGradient(color);
        sr.color = color;
        line.colorGradient = gra;
        tail.material.SetColor("_Color", color);
    }
    void UpdateTailMaterial()
    {
        float r = 0.016F * Screen.height* transform.lossyScale.x;
        Vector3 v = Camera.main.WorldToScreenPoint(transform.position);
        tail.material.SetFloat("_Rad", r);
        tail.material.SetFloat("_CenPosX", v.x);
        tail.material.SetFloat("_CenPosY", Screen.height - v.y);

    }
    // Update is called once per frame
    void Update()
    {
        if(fading)
        {
            transform.position += (Vector3)fadeVel * Time.deltaTime;
            fadeRate += Time.deltaTime * 3F;
            UpdateColor();
            if (fadeRate >= 1F)
                Destroy(gameObject);
        }
        else
        {
            if(fadeRate > 0F)
            {
                fadeRate -= Time.deltaTime * 1F;
                UpdateColor();
            }
        }
    }
    private void LateUpdate()
    {
        UpdateTailMaterial();
    }
}
