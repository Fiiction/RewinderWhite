using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgrEffect
{
    public enum Type { Single, Circle, Ring, OutSide, InnerRing, OuterRing};
    public Type type;
    public Color color;
    public float strength, radius, life,startTime,phase;
    public Vector2 pos;

    public BgrEffect(Type t, Color c, float s, float r, float l, Vector2 p)
    {
        float hh, ss, vv;
        Color.RGBToHSV(c,out hh,out ss,out vv);
        ss = 1F - (1F - ss) * (1F - ss);
        c = Color.HSVToRGB(hh, ss, vv);
        type = t; color = c;
        strength = s; radius = r;
        life = l; startTime = Time.time;
        pos = p;
        phase = 0;
    }

}


public class BackgroundSystem : MonoBehaviour
{
    static Vector2 BlockCenterPos(Vector2Int i)
    {
        return new Vector2(i.x - 19.5F, i.y - 9.5F);
    }
    static Vector2Int BlockIndex(Vector2 p)
    {
        int x = Mathf.FloorToInt(p.x + 20F), y = Mathf.FloorToInt(p.y + 10F);
        return new Vector2Int(x, y);
    }
    static bool InBoundary(Vector2Int i)
    {
        return (i.x >= 8 && i.x <= 31 && i.y >= 2 && i.y <= 17);
    }
    static Vector2[] EPS = { new Vector2(-0.33F, -0.33F), new Vector2(-0.33F, 0F), new Vector2(-0.33F, 0.33F),
                new Vector2(0F, -0.33F), new Vector2(0F, 0F), new Vector2(0F, 0.33F),
                new Vector2(0.33F, -0.33F), new Vector2(0.33F, 0F), new Vector2(0.33F, 0.33F)};
    static public Color COLORORANGE = new Color(0.522F, 0.690F, 0.592F), COLORRED = new Color(1.000F, 0.235F, 0.235F),
        COLORBLUE = new Color(0.176F, 0.365F, 0.376F), COLORGREEN = new Color(0.851F, 0.976F, 0.337F);

    Dictionary<Vector2Int, SpriteRenderer> BlockSprite;
    Dictionary<Vector2Int, Color> BlockColor;
    List<BgrEffect> Effect, DeleteList;
    GameObject BlockObj;

    void Generate()
    {
        for (int i = 0; i < 40; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                Vector2Int index = new Vector2Int(i, j);
                var obj = GameObject.Instantiate(BlockObj, (Vector3)BlockCenterPos(index), Quaternion.identity, transform);
                BlockSprite.Add(index, obj.GetComponent<SpriteRenderer>());
                BlockColor.Add(index, Color.white);
            }
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        BlockObj = Resources.Load<GameObject>("Prefabs/BackgroundBlock");
        BlockSprite = new Dictionary<Vector2Int, SpriteRenderer>();
        BlockColor = new Dictionary<Vector2Int, Color>();
        Effect = new List<BgrEffect>();
        Generate();
        //StartCoroutine(StartingWaveCoroutine());
    }
    IEnumerator StartingWaveCoroutine()
    {
        BgrEffect be;
        for (int i = 1; i <= 12; i++)
        {
            Vector2 pos = new Vector2(Random.Range(-12F, 12F), Random.Range(-9F, 9F));
            bool inner = Random.Range(0F, 1F) < 0.5F;
            float str = 0.6F * (30F - i) / 30F;
            if (inner)
                be = new BgrEffect(BgrEffect.Type.InnerRing, new Color(0.8F, 0.8F, 0.8F), str, 24F, 3F, pos);
            else
                be = new BgrEffect(BgrEffect.Type.OuterRing, COLORORANGE, str, 24F, 3F, pos);

            AddEffect(be);
            yield return new WaitForSeconds(0.2F);

        }

    }
    IEnumerator StartingDiamoundCoroutine(Vector2Int index)
    {
        Vector2 pos = BlockCenterPos(index);
        //float str = Mathf.PerlinNoise(12F * pos.x, 0.1F * pos.y);
        float str = (Mathf.Cos(12F * pos.x + 0.5F * pos.y) + 1) * 0.5F;
        float waitTime = 0.06F * Mathf.Abs(pos.y) + 0.12F * Mathf.Abs(pos.x) + Random.Range(0F, 0.4F);
        Color color = InBoundary(index) ? Color.black : new Color(0.522F, 0.690F, 0.592F);
        float life = Random.Range(0.7F, 1.1F);
        if (InBoundary(index))
            str *= 0.6F;
        else
            str *= 1.2F;

        yield return new WaitForSeconds(waitTime * 0.2F);
        AddEffect(new BgrEffect(BgrEffect.Type.Single, color, str * 0.2F, 0, life * 0.5F, pos));
        yield return new WaitForSeconds(waitTime * 0.7F);
        AddEffect(new BgrEffect(BgrEffect.Type.Single, color, str, 0, life, pos));

    }
    void SingleEffect(Vector2Int index, Color color, float str)
    {
        if (!BlockColor.ContainsKey(index))
            return;
        BlockColor[index] = Color.Lerp(BlockColor[index], color, str);
    }

    void CalcColor()
    {
        Vector2Int index;
        for (int i = 0; i < 40; i++)
            for (int j = 0; j < 20; j++)
            {
                index = new Vector2Int(i, j);
                BlockColor[index] = Color.white;
            }
        DeleteList = new List<BgrEffect>();
        foreach (var i in Effect)
        {
            i.phase = (Time.time - i.startTime) / i.life;
            if (i.phase > 1)
            {
                DeleteList.Add(i);
                continue;
            }
            float str, rad;
            float dist, strRate;
            switch (i.type)
            {
                case BgrEffect.Type.Single:
                    index = BlockIndex(i.pos);
                    str = i.strength * Mathf.Min(1F, 2F * (1F - i.phase), 5F * i.phase);
                    SingleEffect(index, i.color, str);
                    break;
                case BgrEffect.Type.Circle:
                    str = i.strength * Mathf.Min(1F, 2F * (1F - i.phase));
                    rad = i.radius * Mathf.Min(1F, 3F * (i.phase));
                    for (int x = Mathf.FloorToInt(i.pos.x - (rad + 1) + 20F); x <= Mathf.FloorToInt(i.pos.x + (rad + 1) + 20F); x++)
                        for (int y = Mathf.FloorToInt(i.pos.y - (rad + 1) + 10F); y <= Mathf.FloorToInt(i.pos.y + (rad + 1) + 10F); y++)
                        {
                            index = new Vector2Int(x, y);
                            dist = (i.pos - BlockCenterPos(index)).magnitude;
                            SingleEffect(index, i.color, str * (rad - dist) / rad);
                        }
                    break;
                case BgrEffect.Type.Ring:
                case BgrEffect.Type.InnerRing:
                case BgrEffect.Type.OuterRing:
                    str = i.strength * (1F - i.phase) * (1F - i.phase);
                    rad = i.radius * Mathf.Pow(i.phase, 1.3F);
                    float halfWidth = rad * 0.3F + 1F;
                    for (int x = Mathf.FloorToInt(i.pos.x - (rad * 1.6F + 1) + 20F); x <= Mathf.FloorToInt(i.pos.x + (rad * 1.6F + 1) + 20F); x++)
                        for (int y = Mathf.FloorToInt(i.pos.y - (rad * 1.6F + 1) + 10F); y <= Mathf.FloorToInt(i.pos.y + (rad * 1.6F + 1) + 10F); y++)
                        {
                            index = new Vector2Int(x, y);
                            if (i.type == BgrEffect.Type.InnerRing && !InBoundary(index))
                                continue;
                            if (i.type == BgrEffect.Type.OuterRing && InBoundary(index))
                                continue;
                            strRate = 0F;
                            foreach (var v in EPS)
                            {
                                dist = (i.pos - BlockCenterPos(index) - v).magnitude;
                                if (dist < rad + halfWidth && dist >= rad)
                                    strRate += (rad + halfWidth - dist) / halfWidth;
                                if (dist < rad && dist > rad - halfWidth)
                                    strRate += (dist - rad + halfWidth) / halfWidth;
                            }
                            strRate /= EPS.Length;
                            SingleEffect(index, i.color, str * strRate);
                        }
                    break;
                case BgrEffect.Type.OutSide:
                    str = i.strength * Mathf.Min(1F, 3F * (1F - i.phase));
                    rad = i.radius * Mathf.Min(1F, 6F * (i.phase));
                    for (int x = Mathf.FloorToInt(i.pos.x - (rad + 1) + 20F); x <= Mathf.FloorToInt(i.pos.x + (rad + 1) + 20F); x++)
                        for (int y = Mathf.FloorToInt(i.pos.y - (rad + 1) + 10F); y <= Mathf.FloorToInt(i.pos.y + (rad + 1) + 10F); y++)
                        {
                            index = new Vector2Int(x, y);
                            if (InBoundary(index))
                                continue;
                            dist = (i.pos - BlockCenterPos(index)).magnitude;
                            SingleEffect(index, i.color, str * (rad - dist) / rad);
                        }
                    break;

            }
        }

        foreach (var i in DeleteList)
            Effect.Remove(i);
        for (int i = 0; i < 40; i++)
            for (int j = 0; j < 20; j++)
            {
                index = new Vector2Int(i, j);
                BlockSprite[index].color = BlockColor[index];
            }
    }
    public void AddEffect(BgrEffect be)
    {
        Effect.Add(be);
    }
    // Update is called once per frame
    void Update()
    {
        CalcColor();
    }
}
