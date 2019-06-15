using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{

    public static int orangeCnt, redCnt, blueCnt;
    public static string text;
    public GameObject Orange, Red, Blue, Inflater, Green, debugText, scoreText;
    float nextBlue, nextInflater;
    public static float score;
    float startTime,gameTime;


    void GenerateOrange()
    {
        float angle = Random.Range(1F,5F)/6F*Mathf.PI;
        Vector3 pos1 = new Vector3(28 * Mathf.Cos(angle), 17 * Mathf.Sin(angle)),
            pos2 = new Vector3(28 * Mathf.Cos(angle), -17 * Mathf.Sin(angle));
        Debug.Log("Pos1: " + pos1.ToString() + "\nPos2: " + pos2.ToString());
        GameObject.Instantiate(Orange, pos1, Quaternion.identity);
        if ((orangeCnt + redCnt) % 2 == 1)
            return;
        GameObject.Instantiate(Orange, pos2, Quaternion.identity);

    }

    void GenerateRed()
    {
        float angle = Random.Range(-2F, 2F) / 6F * Mathf.PI;
        Vector3 pos1 = new Vector3(28 * Mathf.Cos(angle), 17 * Mathf.Sin(angle)),
            pos2 = new Vector3(-28 * Mathf.Cos(angle), 17 * Mathf.Sin(angle));

        GameObject.Instantiate(Red, pos1, Quaternion.identity);

        if ((orangeCnt + redCnt) % 2 == 1)
            return;
        GameObject.Instantiate(Red, pos2, Quaternion.identity);
    }

    void GenerateBlue()
    {
        float angle1 = Random.Range(0, 2 * Mathf.PI);
        //GameObject.Instantiate(Blue, new Vector3(7,7), Quaternion.identity);
        GameObject.Instantiate(Blue, new Vector3(13 * Mathf.Cos(angle1), 9 * Mathf.Sin(angle1)), Quaternion.identity);
        blueCnt++;

        //Calc the time of next generation
        if (gameTime <= 60)
            nextBlue = gameTime + Random.Range(20F, 30F);
        else nextBlue = gameTime + Random.Range(12F, 24F);
    }

    int BasicMinCnt()
    {
        if (gameTime <= 15) return 4;
        if (gameTime <= 30) return 6;
        if (gameTime <= 50) return 8;
        if (gameTime <= 75) return 10;
        if (gameTime <= 105) return 12;
        return 14;

    }
    float RedRate()
    {
        if (gameTime <= 20) return 0;
        else return 0.16F;
    }

    void GenerateBasics()
    {
    }

    void Generate()
    {
        if (orangeCnt + redCnt < BasicMinCnt())
        {
            if (Random.Range(0F, 1F) <= RedRate())
                GenerateRed();
            else
                GenerateOrange();
        }
        /*
        if (gameTime >= nextBlue)
            GenerateBlue();
            */
    }

    void StartingGenerate()
    {
        Vector3 pos1 = new Vector3(20F, 20F), pos2 = new Vector3(-20.15F, 20F);
        GameObject.Instantiate(Orange, pos1, Quaternion.identity);
        GameObject.Instantiate(Orange, pos2, Quaternion.identity);
        GameObject.Instantiate(Orange, -pos1, Quaternion.identity);
        GameObject.Instantiate(Orange, -pos2, Quaternion.identity);

    }

    // Use this for initialization
    void Start()
    {
        Orange = Resources.Load<GameObject>("Prefabs/Enemies/Orange");
        Red = Resources.Load<GameObject>("Prefabs/Enemies/Red");

        score = 0F;
        nextBlue = 35F;
        startTime = Time.time;
        orangeCnt = redCnt = blueCnt = 0;
        StartingGenerate();
    }


    // Update is called once per frame
    void Update()
    {
        gameTime = Time.time - startTime;
        //debugText.GetComponent<Text>().text = text;
        Generate();
    }
}
