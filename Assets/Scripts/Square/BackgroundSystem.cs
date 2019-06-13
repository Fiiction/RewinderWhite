using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgrEffect
{
    public enum Type { Single, Circle, Ring, OutSide};
    public Type type;
    public Color color;
    public float strength, radius, life,startTime,phase;
    public Vector2 pos;

    public BgrEffect(Type t, Color c, float s, float r, float l, Vector2 p)
    {
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
        return (i.x >= 8 && i.x <= 31 && i.y >= 1 && i.y <= 18);
    }

    Dictionary<Vector2Int, SpriteRenderer> BlockSprite;
    Dictionary<Vector2Int, Color> BlockColor;
    List<BgrEffect> Effect, DeleteList;
    GameObject BlockObj;
    static public Color COLORORANGE = new Color(0.522F, 0.690F, 0.592F), COLORPINK = new Color(1.000F, 0.235F, 0.235F),
        COLORBLUE = new Color(0.176F, 0.365F, 0.376F),COLORGREEN = new Color(0.851F,0.976F,0.337F);

    Vector2[] EPS = { new Vector2(-0.33F, -0.33F), new Vector2(-0.33F, 0F), new Vector2(-0.33F, 0.33F),
                new Vector2(0F, -0.33F), new Vector2(0F, 0F), new Vector2(0F, 0.33F),
                new Vector2(0.33F, -0.33F), new Vector2(0.33F, 0F), new Vector2(0.33F, 0.33F)};
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
                //StartCoroutine( StartingDiamoundCoroutine(index));
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
        StartCoroutine(StartingWaveCoroutine());
    }
    IEnumerator StartingWaveCoroutine()
    {
        yield return new WaitForSeconds(1F);

        var be = new BgrEffect(BgrEffect.Type.Ring, COLORORANGE, 0.5F, 24F, 4F, new Vector2(-8F,6F));
        AddEffect(be);

        yield return new WaitForSeconds(1F);

        be = new BgrEffect(BgrEffect.Type.Ring, COLORBLUE, 0.5F, 36F, 4F, new Vector2(8F, -6F));
        AddEffect(be);

        yield return new WaitForSeconds(0.5F);

        be = new BgrEffect(BgrEffect.Type.Ring, COLORGREEN, 0.5F, 18F, 4F, new Vector2(-2F, -6F));
        AddEffect(be);
        yield return new WaitForSeconds(0.5F);

        be = new BgrEffect(BgrEffect.Type.Ring, new Color(0.8F,0.8F,0.8F), 0.5F, 24F, 4F, new Vector2(2F, 6F));
        AddEffect(be);
    }
    IEnumerator StartingDiamoundCoroutine(Vector2Int index)
    {
        Vector2 pos = BlockCenterPos(index);
        //float str = Mathf.PerlinNoise(12F * pos.x, 0.1F * pos.y);
        float str = (Mathf.Cos(12F * pos.x + 0.5F * pos.y)+1) * 0.5F;
        float waitTime = 0.06F * Mathf.Abs(pos.y) + 0.12F * Mathf.Abs(pos.x)+Random.Range(0F,0.4F);
        Color color = InBoundary(index) ? Color.black : new Color(0.522F, 0.690F, 0.592F);
        float life = Random.Range(0.7F, 1.1F);
        if (InBoundary(index))
            str *= 0.6F;
        else
            str *= 1.2F;

        yield return new WaitForSeconds(waitTime* 0.2F);
        AddEffect(new BgrEffect(BgrEffect.Type.Single, color, str*0.2F, 0, life*0.5F, pos));
        yield return new WaitForSeconds(waitTime* 0.7F);
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
            if(i.phase > 1)
            {
                DeleteList.Add(i);
                continue;
            }
            float str,rad;
            float dist, strRate;
            switch (i.type)
            {
                case BgrEffect.Type.Single:
                    index = BlockIndex(i.pos);
                    str = i.strength * Mathf.Min(1F, 2F * (1F - i.phase));
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
                    str = i.strength * (1F - i.phase) * (1F - i.phase);
                    rad = i.radius * Mathf.Pow(i.phase,1.3F);
                    for (int x = Mathf.FloorToInt(i.pos.x - (rad*1.6F + 1) + 20F); x <= Mathf.FloorToInt(i.pos.x + (rad * 1.6F + 1) + 20F); x++)
                        for (int y = Mathf.FloorToInt(i.pos.y - (rad * 1.6F + 1) + 10F); y <= Mathf.FloorToInt(i.pos.y + (rad * 1.6F + 1) + 10F); y++)
                        {
                            index = new Vector2Int(x, y);
                            strRate = 0F;
                            foreach (var v in EPS)
                            {
                                dist = (i.pos - BlockCenterPos(index) - v).magnitude / rad;
                                if (dist < 1.6F && dist >= 1F)
                                    strRate += (1.6F - dist) / 0.6F;
                                if (dist < 1F && dist > 0.4F)
                                    strRate += (dist - 0.4F) / 0.6F;
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

        foreach(var i in DeleteList)
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
        /*
        if(Random.Range(0F,1F)<Time.deltaTime)
        {
            BgrEffect e = new BgrEffect(BgrEffect.Type.Single, Color.HSVToRGB(Random.Range(0F, 1F), 1F, 1F),
                0.10F, 4F, 2F, new Vector2(Random.Range(-18F, 18F), Random.Range(-9F, 9F)));
            Effect.Add(e);
        }
        if (Random.Range(0F, 2F) < Time.deltaTime)
        {
            BgrEffect e = new BgrEffect(BgrEffect.Type.Circle, Color.HSVToRGB(Random.Range(0F, 1F), 1F, 1F),
                0.10F, 4F, 2F, new Vector2(Random.Range(-18F, 18F), Random.Range(-9F, 9F)));
            Effect.Add(e);
        }
        if (Random.Range(0F, 2F) < Time.deltaTime)
        {
            BgrEffect e = new BgrEffect(BgrEffect.Type.OutSide, Color.HSVToRGB(Random.Range(0F, 1F), 1F, 1F),
                0.10F, 4F, 2F, new Vector2(Random.Range(-18F, 18F), Random.Range(-9F, 9F)));
            Effect.Add(e);
        }
        */
    }
}
