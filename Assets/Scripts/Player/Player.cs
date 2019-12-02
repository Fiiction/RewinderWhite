using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct State
{
    public float tim;
    public Vector3 pos;
    public State(Vector3 p, float t)
    {
        tim = t;
        pos = p;
    }
};

struct TouchState
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
    public bool alive = true;
    float lastRewindTime;

    public Vector3 rewindPos;
    //public GameObject RewindMark;
    GameObject dropGraphics;
    GameObject CurrentDrop;
    Queue<State> stateQ = new Queue<State>();
    Queue<TouchState> touchQ = new Queue<TouchState>();
    Vector2 touchSum = Vector2.zero;
    JoyStickController JSC;
    const float XMAX = 11.68F, YMAX = 7.68F;

    public void Kill()
    {
        if (!alive)
            return;
        alive = false;
        CurrentDrop.GetComponent<DropGraphics>().Fade();
        FindObjectOfType<GameSystem>().SetState(GameSystem.State.Ending);
        Destroy(gameObject);
    }
    IEnumerator RewindEffectCoroutine()
    {
        Vector2 rPos = transform.position;
        State[] wavePos = stateQ.ToArray();
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
    }
    public bool Rewind()
    {
        if (!alive)
            return false;
        if (Time.time < lastRewindTime + rewindTime)
            return false;

        CurrentDrop.GetComponent<DropGraphics>().Fade();
        transform.position = rewindPos;
        StartCoroutine(RewindEffectCoroutine());
        stateQ.Clear();
        lastRewindTime = Time.time;
        ResetDrop();
        return true;
    }
    float lastTouchBoundaryTime = -1F;
    void TouchBoundary()
    {
        if (Time.time <= lastTouchBoundaryTime + 1.2F)
            return;
        lastTouchBoundaryTime = Time.time;
        var be = new BgrEffect(BgrEffect.Type.OutSide, Color.black, 0.5F, 12F, 1.2F, (Vector2)(transform.position * 0.78F));
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
        transform.position = transform.position + (Vector3)deltaPos * Time.deltaTime;
    }

    // Use this for initialization
    void Start()
    {
        lastRewindTime  = Time.time;
        //GameObject.Instantiate(RewindMark, transform.position, Quaternion.identity);
        dropGraphics = Resources.Load<GameObject>("Prefabs/DropGraphics");
        JSC = FindObjectOfType<JoyStickController>();
        ResetDrop();
    }

    // Update is called once per frame
    void Update()
    {
        stateQ.Enqueue(new State(transform.position, Time.time));
        while (stateQ.Peek().tim < Time.time - rewindTime)
            stateQ.Dequeue();

        rewindPos = stateQ.Peek().pos;
    }
    private void LateUpdate()
    {
        Move();
    }
}
