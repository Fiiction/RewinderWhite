using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHead : MonoBehaviour {

    public Snake snake;
    int curLine = 0;
    float curLength = 0F;
    public bool destroyed = false;
    float destoryTime = 100000F;
    public int index;
    GameSystem GS;
    EnemyController EC;
    // Use this for initialization
    void Start ()
    {
        snake = FindObjectOfType<Snake>();
        GS = FindObjectOfType<GameSystem>();
        EC = GetComponent<EnemyController>();
    }

    float angle=0F;
       
    public void Init(int _index)
    {
        index = _index;
        angle =(float)index*0.3F*Mathf.PI;
    }

    public void SetPos(float Length, int Line)
    {
        curLength = Length;
        curLine = Line;
    }

    void Move()
    {
        angle = index * Mathf.PI * 0.15F + snake.speed * GS.gameTime;

        curLength += snake.speed * Time.deltaTime;
        if (curLength >= snake.LineList[curLine].Length())
        {
            if (index == snake.LastHead())
                snake.TryAdd(curLength, curLine);
            curLength -= snake.LineList[curLine].Length();
            curLine++;
        }
        transform.position = snake.LineList[curLine].GetPos(curLength);
        transform.position += new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 0.20F;
    }
    // Update is called once per frame
    void Update () {
        Move();
	}

    private void OnDestroy()
    {
        snake.HeadList.Remove(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var head = collision.gameObject.GetComponent<SnakeHead>();
        if (index == 0 && head)
        {
            if (head.index >= 5)
                snake.Bite(head.index);
        }
    }

}
