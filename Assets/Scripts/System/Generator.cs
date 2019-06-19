using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{

    public static int orangeCnt, redCnt;
    public static string text;
    public GameObject Orange, Red, Blue, Inflater, Green, debugText, scoreText;
    float nextBlue, nextInflater;
    float startTime;
    GameSystem GS;

    void GenerateOrange()
    {
        float angle = Random.Range(1F,5F)/6F*Mathf.PI;
        Vector3 pos1 = new Vector3(28 * Mathf.Cos(angle), 17 * Mathf.Sin(angle)),
            pos2 = new Vector3(28 * Mathf.Cos(angle), -17 * Mathf.Sin(angle));
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

    int blueCnt = 0;
    Vector3[] bluePos = { Vector3.left * 24F, Vector3.up * 16F, Vector3.right * 24F, Vector3.down * 16F };
    void GenerateBlue()
    {
        GameObject.Instantiate(Blue, bluePos[blueCnt % 4], Quaternion.identity);
        //Calc the time of next generation
        if (GS.gameTime <= 60)
            nextBlue = GS.gameTime + Random.Range(24F, 30F);
        else nextBlue = GS.gameTime + Random.Range(18F, 24F);
        blueCnt++;
    }

    int BasicMinCnt()
    {
        if (GS.gameTime <= 15) return 4;
        if (GS.gameTime <= 30) return 6;
        if (GS.gameTime <= 50) return 8;
        if (GS.gameTime <= 75) return 10;
        if (GS.gameTime <= 105) return 12;
        return 14;

    }
    float RedRate()
    {
        if (GS.gameTime <= 20) return 0;
        else return 0.16F;
    }

    float nextGenerateTime = 0F;
    void Generate()
    {
        if (orangeCnt + redCnt < BasicMinCnt() && Time.time >= nextGenerateTime)
        {
            if (Random.Range(0F, 1F) <= RedRate())
                GenerateRed();
            else
                GenerateOrange();
            nextGenerateTime = Time.time + 2F * 60F/ (60F+GS.gameTime);
        }
        
        if (GS.gameTime >= nextBlue)
            GenerateBlue();
            
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
        GS = FindObjectOfType<GameSystem>();

        Orange = Resources.Load<GameObject>("Prefabs/Enemies/Orange");
        Red = Resources.Load<GameObject>("Prefabs/Enemies/Red");
        Blue = Resources.Load<GameObject>("Prefabs/Enemies/Blue");

        nextBlue = 35F;
        startTime = Time.time;
        orangeCnt = redCnt = blueCnt = 0;
        StartingGenerate();
    }


    // Update is called once per frame
    void Update()
    {
        if (!GS.Gaming())
            Destroy(gameObject);
        Generate();
    }
}
