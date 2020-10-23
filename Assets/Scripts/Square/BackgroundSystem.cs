using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BgrEffect
{
    public enum Type { Single, Circle, Focus, Ring, OutSide, InnerRing, OuterRing, MemRing, Boundary, BoundaryCircle};
    public Type type;
    public Color color;
    public float strength, radius, life,startTime,phase;
    public Vector2 pos;
    public int memIndex;
    public BgrEffect(Type t, Color c, float s, float r, float l, Vector2 p,int mI=0)
    {
        //Debug.Log(t);
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
    static public bool OnBoundary(Vector2Int i)
	{
        return (!InBoundary(i)) && i.x>= 7 && i.x <= 32 && i.y>=1 && i.y<=18;
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

    void SingleEffect(Vector2Int index, Color color, float stre)
    { 
        if (!BlockColor.ContainsKey(index))
            return;
        BlockColor[index] = Color.Lerp(BlockColor[index], color, stre);
    }
    
    float Noise(float x, float y)
    {
        x *= 64f;y *= 64f;
        float z = Time.time * 0.9f;
        float ret = Mathf.PerlinNoise(x, x - z) + Mathf.PerlinNoise(y + z, x);
        ret = ret * 2.5f - 2f;
        return Mathf.Clamp01(ret);
    }

    void FadingLine(Vector2Int startPos, Vector2Int dir,Color c, float startStrength = 0.6f, float multiplier = 0.93f)
	{
        float stre = startStrength;
        for(Vector2Int pos = startPos; InScreen(pos); pos += dir)
		{
            SingleEffect(pos, c, stre);
            stre *= multiplier;
		}
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
            float stre, rad;
            float dist, streRate, bStre;
            float halfWidth;
            switch (i.type)
            {
                case BgrEffect.Type.Single:
                    index = BlockIndex(i.pos);
                    stre = i.strength * Mathf.Min(1F, 2F * (1F - i.phase), 5F * i.phase);
                    SingleEffect(index, i.color, stre);
                    break;
                case BgrEffect.Type.Circle:
                    stre = i.strength * Mathf.Min(1F, 2F * (1F - i.phase));
                    rad = i.radius * Mathf.Min(1F, 3F * (i.phase));
                    for (int x = Mathf.FloorToInt(i.pos.x - (rad + 1) + 20F); x <= Mathf.FloorToInt(i.pos.x + (rad + 1) + 20F); x++)
                        for (int y = Mathf.FloorToInt(i.pos.y - (rad + 1) + 10F); y <= Mathf.FloorToInt(i.pos.y + (rad + 1) + 10F); y++)
                        {
                            index = new Vector2Int(x, y);
                            dist = (i.pos - BlockCenterPos(index)).magnitude;
                            SingleEffect(index, i.color, stre * (rad - dist) / rad);
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
                        stre = i.strength * (i.phase) * (i.phase);
                        rad = i.radius * Mathf.Pow(1F - i.phase, 1.3F);
                    }
                    else
                    {
                        stre = i.strength * (1F - i.phase) * (1F - i.phase);
                        rad = i.radius * Mathf.Pow(i.phase, 1.3F);
                    }
                    halfWidth = rad * 0.3F + 1F;
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
                            streRate = 0F;
                            foreach (var vec in EPS)
                            {
                                dist = (i.pos - BlockCenterPos(index) - vec).magnitude;
                                if (dist < rad + halfWidth && dist >= rad)
                                    streRate += (rad + halfWidth - dist) / halfWidth;
                                if (dist < rad && dist > rad - halfWidth)
                                    streRate += (dist - rad + halfWidth) / halfWidth;
                            }
                            streRate /= EPS.Length;
                            SingleEffect(index, i.color, stre * streRate);
                        }
                    break;
                case BgrEffect.Type.OutSide:
                case BgrEffect.Type.BoundaryCircle:
                    stre = i.strength * Mathf.Min(1F, 3F * (1F - i.phase));
                    rad = i.radius * Mathf.Min(1F, 6F * (i.phase));
                    for (int x = Mathf.FloorToInt(i.pos.x - (rad + 1) + 20F); x <= Mathf.FloorToInt(i.pos.x + (rad + 1) + 20F); x++)
                        for (int y = Mathf.FloorToInt(i.pos.y - (rad + 1) + 10F); y <= Mathf.FloorToInt(i.pos.y + (rad + 1) + 10F); y++)
                        {
                            index = new Vector2Int(x, y);
                            if (InBoundary(index))
                                continue;
                            if (i.type == BgrEffect.Type.BoundaryCircle && !OnBoundary(index))
                                continue;
                            dist = (i.pos - BlockCenterPos(index)).magnitude;
                            float clamp = Mathf.Clamp01((rad - dist) / rad);
                            SingleEffect(index, i.color, stre * clamp);
                        }
                    break;
                case BgrEffect.Type.Boundary:
                    bStre = Mathf.Clamp01(i.phase * 8f);
                    stre = i.strength * Mathf.Clamp01(2f * (1F - i.phase));
                    rad = 20f * Mathf.Pow(i.phase, 1.3F) + 0f;
                    halfWidth = rad * 0.5F + 4F;
                    for (int x = 0; x < 40; x++)
                        for (int y = 0; y < 20; y++)
                        {
                            index = new Vector2Int(x, y);
                            if (OnBoundary(index))
							{
                                dist = Mathf.Abs((float)x - 19.5f) + Mathf.Abs((float)y - 9.5f);
                                streRate = 0f;
                                if (dist < rad + halfWidth && dist >= rad)
                                    streRate = (rad + halfWidth - dist) / halfWidth;
                                if (dist < rad && dist > rad - halfWidth)
                                    streRate = (dist - rad + halfWidth) / halfWidth;
                                streRate *= 0.7f;
                                streRate = 1.1f * bStre * (1f - 1.5f * streRate);
                                SingleEffect(index, i.color, stre * streRate);
                            }
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
