using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class JoyStickController : MonoBehaviour
{
    public GameObject Plate, Stick;
    public Vector3 defaultPos = new Vector3(-12F, -6F,0F);
    public float radius = 2F;
    public Vector2 axis = Vector2.zero;

    float sensi = 2F;
    bool touching = false;
    Touch curTouch;
    Vector3 platePos,curVec;
    GameSystem GS;
    Color c;
    float alpha = 0F;

    // Start is called before the first frame update
    void Start()
    {
        touching = false;
        curVec = Vector3.zero;
        platePos = defaultPos;
        GS = FindObjectOfType<GameSystem>();
        c = Plate.GetComponent<SpriteRenderer>().color;
        c.a = alpha;
        Plate.GetComponent<SpriteRenderer>().color = c;
        Stick.GetComponent<SpriteRenderer>().color = c;
    }

    void EndTouch()
    {
        touching = false;
        curVec = Vector3.zero;
        platePos = defaultPos;
    }
    void StartTouch(Touch t)
    {
        touching = true;
        curTouch = t;
        platePos = Camera.main.ScreenToWorldPoint(t.position);
        platePos.z = 0F;
        curVec = Vector3.zero;
    }
    // Update is called once per frame
    void Update()
    {
        touching = false;

        foreach (var i in Input.touches)
        {
            if (i.position.x <= Screen.width * 0.5F)
            {
                if(i.phase == TouchPhase.Began)
                {
                    touching = true;
                    platePos = Camera.main.ScreenToWorldPoint(i.position);
                    platePos.z = 0F;
                    curVec = Vector3.zero;
                    Debug.Log(platePos);
                }
                if (i.phase == TouchPhase.Moved || i.phase == TouchPhase.Stationary)
                {
                    touching = true;
                    curVec += sensi * (Vector3)i.deltaPosition * 20F / Screen.height;
                }
            }
        }

        if(!touching)
        {
            curVec = Vector3.zero;
            platePos = defaultPos;
        }

        if(GS.Gaming())
        {
            if(alpha <1F)
            {
                alpha += Time.deltaTime;
                c.a = alpha;
                Plate.GetComponent<SpriteRenderer>().color = c;
                Stick.GetComponent<SpriteRenderer>().color = c;
            }
        }
        else
        {
            alpha -= Time.deltaTime;
            c.a = alpha;
            Plate.GetComponent<SpriteRenderer>().color = c;
            Stick.GetComponent<SpriteRenderer>().color = c;
        }

        Plate.transform.position = platePos;
        Stick.transform.position = platePos + Vector3.ClampMagnitude(curVec, radius);
        axis = (Vector2)Vector3.ClampMagnitude(curVec, radius) / radius;
    }
}
