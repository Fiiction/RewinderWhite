using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public static int orangeCnt, redCnt;
    public static string text;
    public GameObject Orange, Red, Blue, Inflater, Green, debugText, scoreText;
    float nextBlue, nextInflater, nextGreen;
    float startTime;
    GameSystem GS;

    float SpeedMultiplier()
    {
        switch(GS.state)
        {
            case GameSystem.State.Easy:
                return 0.9F + (GS.gameTime / (GS.gameTime + 60F)) * 0.24F;
            case GameSystem.State.Hard:
                return 0.95F + (GS.gameTime / (GS.gameTime + 60F)) * 0.2F;
        }
        return 1.0F;
    }

    void GenerateOrange()
    {
        float angle = Random.Range(1F,5F)/6F*Mathf.PI;
        Vector3 pos1 = new Vector3(28 * Mathf.Cos(angle), 17 * Mathf.Sin(angle)),
            pos2 = new Vector3(28 * Mathf.Cos(angle), -17 * Mathf.Sin(angle));
        var o1 = GameObject.Instantiate(Orange, pos1, Quaternion.identity);
        o1.GetComponent<Orange>().speed *= SpeedMultiplier();
        o1.GetComponent<Orange>().angSpeed*= SpeedMultiplier();
        if ((orangeCnt + redCnt) % 2 == 1)
            return;
        o1 = GameObject.Instantiate(Orange, pos2, Quaternion.identity);
        o1.GetComponent<Orange>().speed *= SpeedMultiplier();
        o1.GetComponent<Orange>().angSpeed *= SpeedMultiplier();

    }

    void GenerateRed()
    {
        float angle = Random.Range(-2F, 2F) / 6F * Mathf.PI;
        Vector3 pos1 = new Vector3(28 * Mathf.Cos(angle), 17 * Mathf.Sin(angle)),
            pos2 = new Vector3(-28 * Mathf.Cos(angle), 17 * Mathf.Sin(angle));

        var r1 = GameObject.Instantiate(Red, pos1, Quaternion.identity);
        r1.GetComponent<Red>().speed *= SpeedMultiplier();
        if ((orangeCnt + redCnt) % 2 == 1)
            return;
        r1 = GameObject.Instantiate(Red, pos2, Quaternion.identity);
        r1.GetComponent<Red>().speed *= SpeedMultiplier();
    }

    int blueCnt = 0;
    Vector3[] bluePos = { Vector3.left * 24F, Vector3.up * 16F, Vector3.right * 24F, Vector3.down * 16F };
    void GenerateBlue()
    {
        var b = GameObject.Instantiate(Blue, bluePos[blueCnt % 4], Quaternion.identity);
        b.GetComponent<Blue>().speed *= SpeedMultiplier();
        //Calc the time of next generation
        if (GS.gameTime <= 60)
            nextBlue = GS.gameTime + Random.Range(24F, 30F);
        else nextBlue = GS.gameTime + Random.Range(18F, 24F);
        blueCnt++;
    }

    int greenCnt = 0;
    void GenerateGreen()
    {
        var g = GameObject.Instantiate(Green, bluePos[greenCnt % 4], Quaternion.identity);
        g.GetComponent<Green>().speed *= SpeedMultiplier();
        //Calc the time of next generation
        if (GS.gameTime <= 60)
            nextGreen = GS.gameTime + Random.Range(24F, 30F);
        else nextGreen = GS.gameTime + Random.Range(18F, 24F);
        greenCnt++;
    }

    int BasicMinCnt()
    {
        switch(GS.state)
        {
            case GameSystem.State.Hard:
                if (GS.gameTime <= 15) return 4;
                if (GS.gameTime <= 30) return 6;
                if (GS.gameTime <= 50) return 8;
                if (GS.gameTime <= 75) return 10;
                if (GS.gameTime <= 105) return 12;
                return 14;
            case GameSystem.State.Easy:
                if (GS.gameTime <= 12) return 2;
                if (GS.gameTime <= 24) return 4;
                if (GS.gameTime <= 40) return 6;
                if (GS.gameTime <= 60) return 8;
                if (GS.gameTime <= 85) return 10;
                return 12;
        }
        return -2;

    }
    float RedRate()
    {
        switch (GS.state)
        {
            case GameSystem.State.Hard:
                return 0.16F;
            case GameSystem.State.Easy:
                if (GS.gameTime <= 20) return 0;
                else if (GS.gameTime <= 40) return 0.12F;
                else return 0.16F;
        }
        return 0F;
    }

    float nextGenerateTime = 0F;
    void Generate()
    {
        if (orangeCnt + redCnt < BasicMinCnt() && GS.gameTime >= nextGenerateTime)
        {
            if (Random.Range(0F, 1F) <= RedRate())
                GenerateRed();
            else
                GenerateOrange();

            switch (GS.state)
            {
                case GameSystem.State.Hard:
                    nextGenerateTime = GS.gameTime + 1.6F * 80F / (80F + GS.gameTime);
                    break;
                case GameSystem.State.Easy:
                    nextGenerateTime = GS.gameTime + 3F * 60F / (60F + GS.gameTime);
                    break;
                default:
                    nextGenerateTime = 99999F;
                    break;
            }
        }
        
        if (GS.gameTime >= nextBlue)
            GenerateBlue();

        if (GS.gameTime >= nextGreen)
            GenerateGreen();
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
        Green = Resources.Load<GameObject>("Prefabs/Enemies/Green/Green");
        switch (GS.state)
        {
            case GameSystem.State.Hard:
                nextBlue = 5F;
                nextGreen = 20F;
                break;
            case GameSystem.State.Easy:
                nextBlue = 35F;
                nextGreen = 55F;
                break;
            default:
                nextBlue = 99999F;
                nextGreen = 99999F;
                break;
        }
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
