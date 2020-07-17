using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BgrEffect
{
    public enum Type { Single, Circle, Focus, Ring, OutSide, InnerRing, OuterRing, MemRing};
    public Type type;
    public Color color;
    public float strength, radius, life,startTime,phase;
    public Vector2 pos;
    public int memIndex;
    public BgrEffect(Type t, Color c, float s, float r, float l, Vector2 p,int mI=0)
    {
        c = StandardColors.Adjust(c);
        type = t; color = c;
        strength = s; radius = r;
        life = l; startTime = Time.time;
        pos = p;
        phase = 0;
        memIndex = mI;
    }
}

public class MeteorEffect
{
    float r;
    Vector2 pos,vec;
    HashSet<Vector2Int> Effected;

    public MeteorEffect()
    {
        r = Random.Range(0F, 8F);
        float theta = Random.Range(0F, 2F) * Mathf.PI;
        vec = new Vector2(Mathf.Cos(theta)*1.6F, Mathf.Sin(theta));
        pos = vec * r;
        Effected = new HashSet<Vector2Int>();
    }
    public bool Update(BackgroundSystem BS)
    {
        r += Time.deltaTime * 5F;
        pos = vec * r;
        Vector2Int index = BackgroundSystem.BlockIndex(pos);
        if (!BackgroundSystem.InScreen(index))
            return false;
        if(!Effected.Contains(index))
        {
            Vector2 p = BackgroundSystem.BlockCenterPos(index);
            float dist = Mathf.Abs(p.x - pos.x) + Mathf.Abs(p.y - pos.y);
            if (dist > 0.5F)
                return true;
            Effected.Add(index);
            Color c = BackgroundSystem.InBoundary(index) ? Color.black : StandardColors.COLORORANGE;
            float s = r * 0.03F*Random.Range(0.6F,1.4F);
            BgrEffect be = new BgrEffect(BgrEffect.Type.Single, c, s, 0F, 1F, p);
            BS.AddEffect(be);
        }
        return true;

    }
}


public class BackgroundSystem : MonoBehaviour
{
    static public Vector2 BlockCenterPos(Vector2Int i)
    {
        return new Vector2(i.x - 19.5F, i.y - 9.5F);
    }
    static public Vector2Int BlockIndex(Vector2 p)
    {
        int x = Mathf.FloorToInt(p.x + 20F), y = Mathf.FloorToInt(p.y + 10F);
        return new Vector2Int(x, y);
    }
    static public bool InBoundary(Vector2Int i)
    {
        return (i.x >= 8 && i.x <= 31 && i.y >= 2 && i.y <= 17);
    }
    static public bool InScreen(Vector2Int i)
    {
        return (i.x >= 0 && i.x <= 39 && i.y >= 0 && i.y <= 19);
    }
    static Vector2[] EPS = { new Vector2(-0.33F, -0.33F), new Vector2(-0.33F, 0F), new Vector2(-0.33F, 0.33F),
                new Vector2(0F, -0.33F), new Vector2(0F, 0F), new Vector2(0F, 0.33F),
                new Vector2(0.33F, -0.33F), new Vector2(0.33F, 0F), new Vector2(0.33F, 0.33F)};

    Dictionary<Vector2Int, SpriteRenderer> BlockSprite;
    Dictionary<Vector2Int, Color> BlockColor;
    List<BgrEffect> Effect, DeleteList;
    List<MeteorEffect> MeteorEffects;
    GameObject BlockObj;
    GameSystem GS;
    public Color bgrColor = Color.white;

    public IEnumerator SetBgrColorCoroutine(Color c)
    {
        yield return new WaitForSeconds(1F);
        DOTween.To(() => bgrColor, x => bgrColor = x, c, 1F);
        yield return new WaitForSeconds(1F);

    }

    void Generate()
    {
        for (int i = 0; i < 40; i++)
            for (int j = 0; j < 20; j++)
            {
                Vector2Int index = new Vector2Int(i, j);
                var obj = GameObject.Instantiate(BlockObj, (Vector3)BlockCenterPos(index), Quaternion.identity, transform);
                BlockSprite.Add(index, obj.GetComponent<SpriteRenderer>());
                BlockColor.Add(index, Color.white);
            }
    }

    // Start is called before the first frame update
    void Start()
    {
        GS = FindObjectOfType<GameSystem>();
        BlockObj = Resources.Load<GameObject>("Prefabs/BackgroundBlock");
        BlockSprite = new Dictionary<Vector2Int, SpriteRenderer>();
        BlockColor = new Dictionary<Vector2Int, Color>();
        Effect = new List<BgrEffect>();
        MeteorEffects = new List<MeteorEffect>();
        Generate();
        AddEffect(new BgrEffect(BgrEffect.Type.Ring, new Color(0.8F, 0.8F, 0.8F), 0.8F, 42F, 4F, Vector2.zero));
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
                be = new BgrEffect(BgrEffect.Type.OuterRing, StandardColors.COLORORANGE, str, 24F, 3F, pos);

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
        Color color = InBoundary(index) ? Color.black : StandardColors.Adjust(StandardColors.COLORORANGE);
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
                BlockColor[index] = bgrColor;
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
                case BgrEffect.Type.Focus:
                case BgrEffect.Type.InnerRing:
                case BgrEffect.Type.OuterRing:
                case BgrEffect.Type.MemRing:
                    int minX = 2 + i.memIndex * 4;
                    if( i.type == BgrEffect.Type.Focus)
                    {
                        str = i.strength * (i.phase) * (i.phase);
                        rad = i.radius * Mathf.Pow(1F - i.phase, 1.3F);
                    }
                    else
                    {
                        str = i.strength * (1F - i.phase) * (1F - i.phase);
                        rad = i.radius * Mathf.Pow(i.phase, 1.3F);
                    }
                    float halfWidth = rad * 0.3F + 1F;
                    for (int x = Mathf.FloorToInt(i.pos.x - (rad * 1.6F + 1) + 20F); x <= Mathf.FloorToInt(i.pos.x + (rad * 1.6F + 1) + 20F); x++)
                        for (int y = Mathf.FloorToInt(i.pos.y - (rad * 1.6F + 1) + 10F); y <= Mathf.FloorToInt(i.pos.y + (rad * 1.6F + 1) + 10F); y++)
                        {
                            index = new Vector2Int(x, y);
                            if (i.type == BgrEffect.Type.InnerRing && !InBoundary(index))
                                continue;
                            if (i.type == BgrEffect.Type.OuterRing && InBoundary(index))
                                continue;
                            if(i.type == BgrEffect.Type.MemRing)
                            {
                                if (x < minX || x > minX + 3)
                                    continue;
                                if (y <= 12 && y >= 7)
                                    continue;
                            }
                            strRate = 0F;
                            foreach (var vec in EPS)
                            {
                                dist = (i.pos - BlockCenterPos(index) - vec).magnitude;
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
        if (be.strength == 0 || be.life == 0)
            return;
        Effect.Add(be);
    }
    // Update is called once per frame
    void Update()
    {
        CalcColor();
        Camera.main.backgroundColor = bgrColor;

        if(Input.GetMouseButtonDown(0) && !GS.Gaming())
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(GS.state == GameSystem.State.Ending)
                AddEffect(new BgrEffect(BgrEffect.Type.Ring, new Color(1F, 1F, 1F), 0.8F, 24F, 2F, pos));
            else
                AddEffect( new BgrEffect(BgrEffect.Type.Ring, new Color(0F, 0F, 0F), 0.18F, 24F, 2F, pos));
        }
    }
}
