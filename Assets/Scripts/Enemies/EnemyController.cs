using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int strength;
    public Player player;
    public Vector2 vel = Vector2.zero;
    GameObject CurrentDrop;

    [Header("Drop")]
    public Color color;
    public float tailLength = 1.0F;
    public float lineWidth = 1.0F;
    public float lineLength = 1.0F;
    [Header("Drip Color")]
    public float dripFreq = 0.0F;
    public float dripRad = 1F;
    [Header("Death Wave")]
    public float waveS = 0.15F;
    public float waveL = 1.0F;
    public float waveR = 9.0F;

    public void Kill()
    {
        CurrentDrop.transform.SetParent(null);
        CurrentDrop.GetComponent<DropGraphics>().Fade(vel);

        var be = new BgrEffect(BgrEffect.Type.Ring, color, waveS, waveR, waveL,
            (Vector2)transform.position);
        FindObjectOfType<BackgroundSystem>().AddEffect(be);

        Destroy(gameObject);
    }

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }
    // Start is called before the first frame update
    void Start()
    {
        CurrentDrop = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/DropGraphics"), transform);
        var dg = CurrentDrop.GetComponent<DropGraphics>();
        dg.basicColor = color;
        dg.tailLength = tailLength;
        dg.lineWidth = lineWidth;
        dg.lineLength = lineLength;
    }

    void DripColor()
    {
        if (dripFreq <= 0F)
            return;
        if (Random.Range(0F, 1F) <= Time.deltaTime * dripFreq)
        {
            float angle = Random.Range(0F, 2F * Mathf.PI);
            Vector2 deltaPos = Random.Range(0.3F, 1F) * dripRad * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            var be = new BgrEffect(BgrEffect.Type.Single, color, Random.Range(0.3F,0.7F), 0F, 1F,
                (Vector2)transform.position + deltaPos);
            FindObjectOfType<BackgroundSystem>().AddEffect(be);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
            collision.gameObject.GetComponent<Player>().Kill();
        if (collision.gameObject.tag == "Enemy")
        {
            var c = collision.gameObject.GetComponent<EnemyController>();
            if (c.strength <= strength)
            {
                c.Kill();
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        //DripColor();
    }
}
