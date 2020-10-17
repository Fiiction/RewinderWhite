using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PosState
{
    public float tim;
    public Vector3 pos;
    public PosState(Vector3 p, float t)
    {
        tim = t;
        pos = p;
    }
};

public struct TouchState
{
    public float tim;
    public Vector2 pos;
    public TouchState(Vector2 p, float t)
    {
        tim = t;
        pos = p;
    }
}
public class Player : MonoBehaviour
{
    public float speed;
    public float rewindTime;
    public bool alive = true, canRewind = false;
    float lastRewindTime;

    public Vector3 rewindPos;
    GameObject dropGraphics;
    GameObject CurrentDrop;
    Queue<PosState> stateQ = new Queue<PosState>();
    Queue<TouchState> touchQ = new Queue<TouchState>();
    Vector2 touchSum = Vector2.zero;
    JoyStickController JSC;
    const float XMAX = 11.68F, YMAX = 7.68F;
    GameSystem GS;

    public void Kill()
    {
        if (!alive)
            return;
        alive = false;
        CurrentDrop.GetComponent<DropGraphics>().Fade();
        if(GS.state == GameSystem.State.EasyLevel || GS.state == GameSystem.State.HardLevel || 
            GS.state == GameSystem.State.MemoryLevel)
            GS.SetState(GameSystem.State.Ending);
        Destroy(gameObject);
    }
    IEnumerator RewindEffectCoroutine()
    {
        Vector2 rPos = transform.position;
        PosState[] wavePos = stateQ.ToArray();
        float hue = Random.Range(0F,1F);
        int f = (wavePos.Length - 1) / 6;
        if (f <= 0)
        {
            yield break;
        }
        int cnt = 0;
        float r,str;
        for(int i = wavePos.Length-1;i>=0;i--)
        {
            if(i % f == 0)
            {
                cnt++;
                r = Mathf.Abs(cnt - 4) * 3F + 3F;
                str = 0.24F - 0.01F * r;
                hue += 0.12F;
                if (hue >= 1F)
                    hue -= 1F;
                var be = new BgrEffect(BgrEffect.Type.Ring, Color.HSVToRGB(hue, 0.0F, 0.0F), 0.12F, r, 2F,
                    (Vector2)wavePos[i].pos + (Vector2)transform.position - rPos);
                FindObjectOfType<BackgroundSystem>().AddEffect(be);
                yield return new WaitForSeconds(0.06F);
            }
        }
    }
    void ResetDrop()
    {
        CurrentDrop = GameObject.Instantiate(dropGraphics, transform);
        CurrentDrop.GetComponent<DropGraphics>().basicColor = StandardColors.COLORPLAYER;
        CurrentDrop.GetComponent<DropGraphics>().tailLength = 0.91F;
    }
    public bool Rewind()
    {
        if (!alive)
            return false;
        if (!canRewind)
            return false;
        CurrentDrop.GetComponent<DropGraphics>().Fade();
        transform.position = rewindPos;
        StartCoroutine(RewindEffectCoroutine());
        stateQ.Clear();
        lastRewindTime = Time.time;
        ResetDrop();
        return true;
    }
    float lastTouchBoundaryTime = -1F, lastBoundaryWarningTime = -1F;

    void BoundaryWarning()
    {
        if (Time.time <= lastBoundaryWarningTime + 6F)
            return;
        if (Time.time <= lastTouchBoundaryTime + 5F)
            return;
        lastBoundaryWarningTime = Time.time;
        var be = new BgrEffect(BgrEffect.Type.Boundary, Color.black, 0.35F, 12F, 1.5F, (Vector2)(transform.position * 0.78F));
        FindObjectOfType<BackgroundSystem>().AddEffect(be);
    }

    void TouchBoundary()
    {
        if (Time.time <= lastBoundaryWarningTime + 2F)
            return;
        if (Time.time <= lastTouchBoundaryTime + 1.6F)
            return;
        lastTouchBoundaryTime = Time.time;
        var be = new BgrEffect(BgrEffect.Type.Boundary, Color.black, 0.5F, 12F, 2F, (Vector2)(transform.position * 0.78F));
        FindObjectOfType<BackgroundSystem>().AddEffect(be);
    }
    void Move()
    {
        if (!alive)
            return;
        Vector2 deltaPos = Vector2.zero;
        if (JSC)
            deltaPos = JSC.axis;
        if (Input.GetKey(KeyCode.UpArrow))
            deltaPos += Vector2.up * 20;
        if (Input.GetKey(KeyCode.DownArrow))
            deltaPos += Vector2.down * 20;
        if (Input.GetKey(KeyCode.LeftArrow))
            deltaPos += Vector2.left * 20;
        if (Input.GetKey(KeyCode.RightArrow))
            deltaPos += Vector2.right * 20;

        deltaPos /= Time.deltaTime;

        touchQ.Enqueue(new TouchState(deltaPos, Time.time));
        touchSum += deltaPos;
        while (touchQ.Peek().tim < Time.time - 0.16F)
        {
            touchSum -= touchQ.Peek().pos;
            touchQ.Dequeue();
        }

        deltaPos = Vector2.ClampMagnitude(touchSum, speed);

        if (Input.GetKey(KeyCode.Space))
            Rewind();


        if (transform.position.x > XMAX && deltaPos.x > 0)
        {
            deltaPos.x = 0;
            TouchBoundary();
        }
        if (transform.position.x < -XMAX && deltaPos.x < 0)
        {
            deltaPos.x = 0;
            TouchBoundary();
        }
        if (transform.position.y > YMAX && deltaPos.y > 0)
        {
            deltaPos.y = 0;
            TouchBoundary();
        }
        if (transform.position.y < -YMAX && deltaPos.y < 0)
        {
            deltaPos.y = 0;
            TouchBoundary();
        }
        if (Mathf.Abs(transform.position.y) > YMAX - 2.4F || Mathf.Abs(transform.position.x) > XMAX - 2.4F)
            BoundaryWarning();
        transform.position = transform.position + (Vector3)deltaPos * Time.deltaTime;
    }

    // Use this for initialization
    void Start()
    {
        GS = FindObjectOfType<GameSystem>();
        lastRewindTime  = Time.time;
        //GameObject.Instantiate(RewindMark, transform.position, Quaternion.identity);
        dropGraphics = Resources.Load<GameObject>("Prefabs/DropGraphics");
        JSC = FindObjectOfType<JoyStickController>();
        ResetDrop();
    }

    Vector3 prevP;
    float prevT;
    // Update is called once per frame
    void Update()
    {
        stateQ.Enqueue(new PosState(transform.position, Time.time));
        /*
        for(float i = 0.1F;i<=1F;i+=0.1F)
        {
            stateQ.Enqueue(new PosState(i*transform.position+(1-i)*prevP, i*Time.time+(1-i)*prevT));
        }
        */
        while (stateQ.Peek().tim < Time.time - rewindTime)
            stateQ.Dequeue();
        prevP = transform.position;
        prevT = Time.time;
        rewindPos = stateQ.Peek().pos;
        canRewind = (Time.time >= lastRewindTime + rewindTime);
    }
    private void LateUpdate()
    {
        Move();
    }
}
