using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int strength;
    public bool killEqual = true;
    public float scoreRate;
    public Player player;
    public float timeAlive = 0F;
    GameObject CurrentDrop, HollowDrop;
    DropGraphics dg, hdg;
    GameSystem GS;

    [Header("Drop")]
    public Color color;
    public float _tailLength = 1.0F;
    public float tailLength
    {
        get { return _tailLength; }
        set
        {
            Debug.Log(gameObject.name);
            _tailLength = value;
            if (dg)
                dg.tailLength = value;
            if (hdg)
                hdg.tailLength = value;
        }
    }
    
    public float lineWidth = 1.0F;
    public float lineLength = 1.0F;
    public bool isBossPart = false;
    public bool hasNoDrop = false;
    [Header("Drip Color")]
    public float dripFreq = 0.0F;
    public float dripRad = 1F;
    [Header("Death Wave")]
    public float waveS = 0.15F;
    public float waveL = 1.0F;
    public float waveR = 9.0F;
    [Header("Audio When Killing Others")]
    public string killAudioName = "";
    public bool isKillAudioMainLoop = false;
    public Vector2 lastFramePos,vec;
    public void Kill(bool scoring = true)
    {
        CurrentDrop.transform.SetParent(null);
        CurrentDrop.GetComponent<DropGraphics>().Fade(vec);
        if(isBossPart)
        {
            HollowDrop.transform.SetParent(null);
            HollowDrop.GetComponent<DropGraphics>().Fade(vec);
        }
        var be = new BgrEffect(BgrEffect.Type.Ring, color, waveS, waveR, waveL,
            (Vector2)transform.position);
        FindObjectOfType<BackgroundSystem>().AddEffect(be);
        if(scoring)
            FindObjectOfType<GameSystem>().AddScore(scoreRate);
        Destroy(gameObject);
    }

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }
    // Start is called before the first frame update
    void Start()
    {
        GS = FindObjectOfType<GameSystem>();
        CurrentDrop = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/DropGraphics"), transform);
        dg = CurrentDrop.GetComponent<DropGraphics>();
        dg.basicColor = color;
        dg.tailLength = tailLength;
        
        dg.lineWidth = lineWidth;
        dg.lineLength = lineLength;

        if(isBossPart)
        {
            HollowDrop = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/DropGraphics"), transform);
            hdg = HollowDrop.GetComponent<DropGraphics>();
            hdg.basicColor = Color.white;
            hdg.tailLength = tailLength;
            hdg.lineWidth = 0F;
            hdg.lineLength = 0F;
            hdg.GetComponent<SpriteRenderer>().sortingOrder += 2;
            hdg.transform.Find("Tail").GetComponent<TrailRenderer>().sortingOrder += 2;
            HollowDrop.transform.localScale = Vector3.zero;
        }

        lastFramePos = transform.position;
    }

    void DripColor()
    {
        if (dripFreq <= 0F)
            return;
        if (Random.Range(0F, 1F) <= Time.deltaTime * dripFreq)
        {
            float angle = Random.Range(0F, 2F * Mathf.PI);
            Vector2 deltaPos = Random.Range(0.3F, 1F) * dripRad * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            var be = new BgrEffect(BgrEffect.Type.Single, color, Random.Range(0.3F,0.5F), 0F, 1F,
                (Vector2)transform.position + vec * 0.3F + deltaPos);
            FindObjectOfType<BackgroundSystem>().AddEffect(be);
        }
    }
    void PlayKillAudio()
    {
        if (killAudioName != "")
        {
            if (isKillAudioMainLoop)
                FindObjectOfType<AudioSystem>().PlayMainLoop(killAudioName);
            else
                FindObjectOfType<AudioSystem>().PlayAudio(killAudioName, AudioSystem.LoopType.Random);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (timeAlive < 0.5F)
            return;
        if (collision.gameObject.tag == "Player")
        {
            FindObjectOfType<GameSystem>().killColor = color;
            collision.gameObject.GetComponent<Player>().Kill();
            PlayKillAudio();
        }
        if (collision.gameObject.tag == "Enemy")
        {
            var c = collision.gameObject.GetComponent<EnemyController>();
            if (!c)
                return;
            if(c.timeAlive < 0.5F)
                return;
            if (c.strength < strength || (c.strength == strength && killEqual))
            {
                c.Kill();
                PlayKillAudio();
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        OnTriggerEnter2D(collision);
    }
    float hollowRate = 0F;
    // Update is called once per frame
    void Update()
    {
        DripColor();
        if (!GS.Gaming())
            Kill();
        if(isBossPart)
        {
            float r = (1 - GS.bossHealth / GS.maxBossHealth)*0.9F;
            hollowRate += (r - hollowRate) * 2F * Time.deltaTime;
            HollowDrop.transform.localScale = new Vector3(hollowRate,hollowRate,1F);
        }
        vec = ((Vector2)transform.position - lastFramePos) / Time.deltaTime;
        lastFramePos = transform.position;
        timeAlive += Time.deltaTime;
    }
}
