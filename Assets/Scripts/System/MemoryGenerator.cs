using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryGenerator : MonoBehaviour
{

    public int orangeCnt, redCnt;
    GameObject Orange, Red;
    float startTime;
    GameSystem GS;
    public float power = 1F, growthPower = 1F;
    public Color orangeColor, redColor;
    public bool startWhenDamaged = true, startingGenerate = true, generateRed = true;

    public float growthRate
    {
        get
        {
            return 1F - GS.bossHealth / GS.maxBossHealth;
        }
    }

    float SpeedMultiplier()
    {
        return 0.9F + power * 0.1F + growthRate * growthPower * 0.1F;
    }

    void GenerateOrange()
    {
        float angle = Random.Range(1F, 5F) / 6F * Mathf.PI;
        Vector3 pos1 = new Vector3(28 * Mathf.Cos(angle), 17 * Mathf.Sin(angle)),
            pos2 = new Vector3(28 * Mathf.Cos(angle), -17 * Mathf.Sin(angle));
        var o1 = GameObject.Instantiate(Orange, pos1, Quaternion.identity);
        o1.GetComponent<Orange>().speed *= SpeedMultiplier();
        o1.GetComponent<Orange>().angSpeed *= SpeedMultiplier();
        o1.GetComponent<EnemyController>().color = orangeColor;
        o1 = GameObject.Instantiate(Orange, pos2, Quaternion.identity);
        o1.GetComponent<Orange>().speed *= SpeedMultiplier();
        o1.GetComponent<Orange>().angSpeed *= SpeedMultiplier();
        o1.GetComponent<EnemyController>().color = orangeColor;
    }

    void GenerateRed()
    {
        float angle = Random.Range(-2F, 2F) / 6F * Mathf.PI;
        Vector3 pos1 = new Vector3(28 * Mathf.Cos(angle), 17 * Mathf.Sin(angle)),
            pos2 = new Vector3(-28 * Mathf.Cos(angle), -17 * Mathf.Sin(angle));

        var r1 = GameObject.Instantiate(Red, pos1, Quaternion.identity);
        r1.GetComponent<Red>().speed *= SpeedMultiplier();
        r1.GetComponent<EnemyController>().color = redColor;
        if ((Generator.orangeCnt + Generator.redCnt) % 2 == 1)
            return;
        r1 = GameObject.Instantiate(Red, pos2, Quaternion.identity);
        r1.GetComponent<Red>().speed *= SpeedMultiplier();
        r1.GetComponent<EnemyController>().color = redColor;
    }
    int BasicMinCnt()
    {
        if (power + growthRate * growthPower <= 1F) return 2;
        if (power + growthRate * growthPower <= 1.5F) return 4;
        if (power + growthRate * growthPower <= 2.0F) return 6;
        if (power + growthRate * growthPower <= 2.5F) return 8;
        return 10;
    }
    float RedRate()
    {
        return 0.3F + growthRate * 0.2F;
        //return ((power + growthRate * growthPower) -0.5F) * 0.32F;
    }

    float nextGenerateTime = 0F;
    void Generate()
    {
        if (power + growthRate * growthPower <= 0F)
            return;
        if (GS.bossHealth == GS.maxBossHealth && startWhenDamaged)
            return;
        if (Generator.orangeCnt + Generator.redCnt < BasicMinCnt())
            nextGenerateTime -= Time.deltaTime;
        if (nextGenerateTime < 0F)
        {
            if (Random.Range(0F, 1F) <= RedRate() && generateRed)
                GenerateRed();
            else
                GenerateOrange();

            nextGenerateTime = 7F / (power + growthRate * growthPower);
        }
    }

    void StartingGenerate()
    {
        if (!startingGenerate)
            return;
        Vector3 pos1 = new Vector3(20F, 20F), pos2 = new Vector3(-20.15F, 20F);
        var o1 = GameObject.Instantiate(Orange, pos1, Quaternion.identity);
        o1.GetComponent<EnemyController>().color = orangeColor;
        o1 = GameObject.Instantiate(Orange, pos2, Quaternion.identity);
        o1.GetComponent<EnemyController>().color = orangeColor;
        o1 = GameObject.Instantiate(Orange, -pos1, Quaternion.identity);
        o1.GetComponent<EnemyController>().color = orangeColor;
        o1 = GameObject.Instantiate(Orange, -pos2, Quaternion.identity);
        o1.GetComponent<EnemyController>().color = orangeColor;
    }

    // Use this for initialization
    void Start()
    {
        GS = FindObjectOfType<GameSystem>();

        Orange = Resources.Load<GameObject>("Prefabs/Enemies/Orange");
        Red = Resources.Load<GameObject>("Prefabs/Enemies/Red");
        startTime = GS.gameTime;
        nextGenerateTime = startTime + 24F / (power + 1F);
        Generator.orangeCnt = Generator.redCnt= 0;
        StartingGenerate();
    }


    // Update is called once per frame
    void Update()
    {
        if (!GS.Gaming())
        {
            Destroy(gameObject, 5F);
            Destroy(this);
        }
        orangeCnt = Generator.orangeCnt;
        redCnt = Generator.redCnt;
        Generate();
    }
}