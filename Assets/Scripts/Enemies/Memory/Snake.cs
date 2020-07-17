using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct Line
{
    public Vector3 stPos, edPos;
    public float Length()
    {
        return (edPos - stPos).magnitude;
    }
    public void SetEd(Vector3 pos)
    {
        //Debug.Log("!!!");
        edPos = pos;
    }
    public Line(Vector3 st,Vector3 ed)
    {
        stPos = st;
        edPos = ed;
    }
    public Vector3 GetPos(float len)
    {
        if (len > Length())
            return edPos;
        return stPos + (edPos - stPos).normalized * len;
    }
}

public class Snake : MonoBehaviour {

    public float speed = 2.5F,HeadSpacing = 0.37F;
    public int BasicHeadCount = 20;
    public List<Line> LineList = new List<Line>();
    public List<SnakeHead> HeadList = new List<SnakeHead>();
    GameObject SnakeHeadPrefab;
    Vector3 startPos;
    int curLine = -1;
    float curLength = 0F;
    GameSystem GS;

    public int HeadCnt()
    {
        return HeadList.Count;
    }
    public int LastHead()
    {
        return HeadList.Count-1;
    }

    public enum Direction { Up, Down, Left, Right };

    public Direction direction = Direction.Up;

    Vector3 ToVector(Direction d)
    {
        if (d == Direction.Up)
            return Vector3.up;
        if (d == Direction.Down)
            return Vector3.down;
        if (d == Direction.Left)
            return Vector3.left;
        if (d == Direction.Right)
            return Vector3.right;
        return Vector3.zero;

    }

    float lastBiteTime = 0F;

    public void Bite(int o)
    {
        if (GS.gameTime <= lastBiteTime + 0.5F)
            return;
        lastBiteTime = GS.gameTime;
        GS.bossHealth -= HeadCnt() - o + 15F;
        foreach (SnakeHead i in HeadList)
            if (i.index >= o)
                i.GetComponent<EnemyController>().Kill();
    }

    int lastAddLine = -1;
    void Add(float length, int line)
    {
        Vector3 pos;
        if (HeadCnt() == 0)
            pos = startPos;
        else
            pos = HeadList[LastHead()].transform.position;
        GameObject head = GameObject.Instantiate(SnakeHeadPrefab, pos, Quaternion.identity);
        head.transform.parent = transform;
        HeadList.Add(head.GetComponent<SnakeHead>());
        head.GetComponent<SnakeHead>().Init(LastHead());
        head.GetComponent<SnakeHead>().SetPos(length - HeadSpacing, line);
        //Debug.Break();
    }

    public void TryAdd(float length,int line)
    {
        if (length <= 0.6F||line<=lastAddLine||GS.gameTime<=lastBiteTime + 3F)
            return;
        if (HeadCnt() < BasicHeadCount * 2)
        {
            Add(length, line);
        }
        if (HeadCnt() < BasicHeadCount * 0.75 && length >= 2F*HeadSpacing)
        {
            Add(length - HeadSpacing, line);
        }
        lastAddLine = line;
    }

    void Turn()
    {
        if (!GS.Gaming())
            return;
        if (direction == Direction.Up || direction == Direction.Down)
        {
            if (transform.position.x > GS.PlayerPos().x)
                direction = Direction.Left;
            else
                direction = Direction.Right;
        }
        else
        {
            if (transform.position.y > GS.PlayerPos().y)
                direction = Direction.Down;
            else
                direction = Direction.Up;
        }

        if (curLine >= 0)
        {
            LineList[curLine] = new Line(LineList[curLine].stPos, transform.position);
        }
        LineList.Add(new Line(transform.position, transform.position + Random.Range(80F, 120F) * ToVector(direction)));
        if(curLine >= 0)
            curLength -= LineList[curLine].Length();
        curLine++;
    }

    IEnumerator GenerateHeadsCoroutine()
    {
        yield return new WaitForSeconds(0.2F);
        for (int i = 1; i <=BasicHeadCount; i++)
        {
            GameObject head = GameObject.Instantiate(SnakeHeadPrefab, startPos, Quaternion.identity);
            head.transform.parent = transform;
            HeadList.Add(head.GetComponent<SnakeHead>());
            head.GetComponent<SnakeHead>().Init(LastHead());
            yield return new WaitForSeconds(HeadSpacing / speed);
        }
    }

	// Use this for initialization
	void Start ()
    {
        GS = FindObjectOfType<GameSystem>();
        Turn();
        startPos = transform.position;
        StartCoroutine(GenerateHeadsCoroutine());
        SnakeHeadPrefab = Resources.Load<GameObject>("Prefabs/Enemies/Memory/SnakeHead");
	}

    void Move()
    {
        curLength += speed * Time.deltaTime;
        if (curLength >= 1.5F&&GS.Gaming())
        {
            if (direction == Direction.Left && transform.position.x < GS.PlayerPos().x)
                Turn();
            if (direction == Direction.Right && transform.position.x > GS.PlayerPos().x)
                Turn();
            if (direction == Direction.Down && transform.position.y < GS.PlayerPos().y)
                Turn();
            if (direction == Direction.Up && transform.position.y > GS.PlayerPos().y)
                Turn();
        }
        transform.position = LineList[curLine].GetPos(curLength);
    }

    // Update is called once per frame
    void Update ()
    {
        Move();
        if (GS.bossHealth <= 0F)
        {
            Bite(1);
            Destroy(gameObject, 0.8F);
        }
        if (!GS.Gaming())
        {
            Bite(1);
            Destroy(gameObject, 0.8F);
        }
    }
}
