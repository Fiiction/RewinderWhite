using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropGraphics : MonoBehaviour
{
    TrailRenderer tail, line;
    public float _tailLength = 1.0F, lineWidth = 1.0F, lineLength = 1.0F;
    public float tailLength
    {
        get { return _tailLength;  }
        set
        {
            _tailLength = value;
            if(tail)
                tail.time = 0.18F * value * transform.lossyScale.x;
        }
    }
    SpriteRenderer sr;
    public Color _basicColor;
    public Color basicColor
    {
        get { return _basicColor; }
        set
        {
            _basicColor = value;
            if(sr)
                UpdateColor();
        }
    }
    float fadeRate = 1F,fadeSpeed = 1F;
    bool fading = false;
    Vector2 fadeVec;

    private void Awake()
    {
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
    }

    public void Fade(Vector2 fv)
    {
        transform.SetParent(null);
        fadeVec = fv;
        if(fadeVec.magnitude > 8F)
            fadeSpeed = fadeVec.magnitude / 8F;
        fading = true;
    }
    public void Fade() => Fade(Vector2.zero);


    // Start is called before the first frame update
    void Start()
    {
        tail = transform.Find("Tail").GetComponent<TrailRenderer>();
        line = transform.Find("Line").GetComponent<TrailRenderer>();
        sr = GetComponent<SpriteRenderer>();
        float scale = transform.lossyScale.x;
        if (tailLength == 0F)
            Destroy(tail.gameObject);
        else
        {
            tail.time = 0.18F * tailLength * scale;
            tail.widthMultiplier = 0.6F * scale;
        }
        if (lineLength == 0F)
            Destroy(line.gameObject);
        else
        {
            line.time = 2.2F * lineLength;
            line.widthMultiplier = 0.024F * lineWidth * scale;
        }
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
        color.a = basicColor.a * (1.0F - fadeRate);
        Gradient gra = SetGradient(color);
        if(sr)
            sr.color = color;
        if(line)
            line.colorGradient = gra;
        if(tail)
            tail.material.SetColor("_Color", color);
    }
    void UpdateTailMaterial()
    {
        if (!tail)
            return;
        float r = 0.016F * Screen.height* transform.lossyScale.x;
        Vector3 v = Camera.main.WorldToScreenPoint(transform.position);
        tail.material.SetFloat("_Rad", r);
        tail.material.SetFloat("_CenPosX", v.x);
        tail.material.SetFloat("_CenPosY", Screen.height - v.y);

    }
    // Update is called once per frame
    void Update()
    {
        float scale = transform.lossyScale.x;
        if(tail)
        {
            tail.time = 0.18F * tailLength * scale;
            tail.widthMultiplier = 0.6F * scale;
        }
        if(line)
        {
            line.time = 2.2F * lineLength;
            line.widthMultiplier = 0.024F * lineWidth * scale;
        }
        if (fading)
        {
            transform.position += (Vector3)fadeVec * Time.deltaTime;
            fadeRate += Time.deltaTime * fadeSpeed * 3F;
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
