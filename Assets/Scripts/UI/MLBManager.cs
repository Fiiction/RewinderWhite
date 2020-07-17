using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLBManager : MonoBehaviour
{
    public static Dictionary<int, MemoryLevelButtonEffect> MLBDict = new Dictionary<int, MemoryLevelButtonEffect>();
    GameSystem GS;
    bool working = false;
    IEnumerator WaveCoroutine()
    {
        working = true;
        yield return new WaitForSeconds(3F);
        int prev = 1, i = 1;
        while(GS.state == GameSystem.State.Memory)
        {
            while (i == prev)
            {
                i = Mathf.FloorToInt(Random.Range(1F, 7.999F));
            }
            //Debug.Log(i);
            //MLBDict[i].power = 1F;
            yield return new WaitForSeconds(Random.Range(0.5F, 2F) + Random.Range(0.5F, 2F));
            prev = i;
        }
        working = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        GS = FindObjectOfType<GameSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GS.state == GameSystem.State.Memory && !working)
            StartCoroutine(WaveCoroutine());
    }
}
