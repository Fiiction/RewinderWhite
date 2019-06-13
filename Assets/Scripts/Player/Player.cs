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
    float lastRewindTime;

    public Vector3 rewindPos;
    //public GameObject RewindMark;
    GameObject dropGraphics;
    GameObject CurrentGraphic;
    Queue<State> stateQ = new Queue<State>();
    Queue<TouchState> touchQ = new Queue<TouchState>();
    Vector2 touchSum = Vector2.zero;

    const float XMAX = 12F, YMAX = 9F;



    IEnumerator RewindEffectCoroutine()
    {
        State[] wavePos = stateQ.ToArray();
        float hue = Random.Range(0F,1F);
        for(int i = wavePos.Length-1;i>=0;i--)
        {
            if(i % 12 ==0)
            {
                hue += 0.1F;
                if (hue >= 1F)
                    hue -= 1F;
                //var be = new BgrEffect(BgrEffect.Type.Circle, Color.HSVToRGB(hue,1.0F,1.0F), 0.06F, 2.4F, 0.5F, wavePos[i].pos);
                var be = new BgrEffect(BgrEffect.Type.Ring, Color.black, 0.12F, 18F, 3F, wavePos[i].pos);
                FindObjectOfType<BackgroundSystem>().AddEffect(be);
                yield return 0;
            }
        }
    }

    public void Rewind()
    {
        if (Time.time < lastRewindTime + rewindTime)
            return;

        CurrentGraphic.GetComponent<DropGraphics>().Fade();
        transform.position = rewindPos;
        StartCoroutine(RewindEffectCoroutine());
        stateQ.Clear();
        lastRewindTime = Time.time;
        CurrentGraphic = GameObject.Instantiate(dropGraphics, transform);
        CurrentGraphic.transform.SetParent(transform);
    }
    float lastTouchBoundaryTime = -1F;
    void TouchBoundary()
    {
        if (Time.time <= lastTouchBoundaryTime + 1.2F)
            return;
        lastTouchBoundaryTime = Time.time;
        var be = new BgrEffect(BgrEffect.Type.OutSide, Color.black, 0.5F, 12F, 1.2F, (Vector2)(transform.position * 0.78F));
        Debug.Log("Be");
        FindObjectOfType<BackgroundSystem>().AddEffect(be);
    }
    void Move()
    {
        Vector2 deltaPos;

        deltaPos = Vector2.zero;

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
        lastRewindTime  = 0F;
        //GameObject.Instantiate(RewindMark, transform.position, Quaternion.identity);
        dropGraphics = Resources.Load<GameObject>("Prefabs/DropGraphics");
        CurrentGraphic = GameObject.Instantiate(dropGraphics, transform);
    }

    // Update is called once per frame
    void Update()
    {
        stateQ.Enqueue(new State(transform.position, Time.time));
        while (stateQ.Peek().tim < Time.time - rewindTime)
            stateQ.Dequeue();

        rewindPos = stateQ.Peek().pos;
        Move();
    }
}
