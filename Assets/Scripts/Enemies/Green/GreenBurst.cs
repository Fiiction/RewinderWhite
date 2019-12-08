using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenBurst : MonoBehaviour
{
    int N = 6;
    float rad = 5F;
    float angVel = 2F;
    float phaseVel = 5F;
    float deltaAngle;
    GameObject GreenSplitObj;
    GameSystem GS;
    IEnumerator GreenBurstCoroutine()
    {
        deltaAngle = Mathf.PI * 2F / (float)N;
        for(int i =0;i<N;i++)
        {
            var gs = GameObject.Instantiate(GreenSplitObj, transform.position, Quaternion.identity)
                .GetComponent<GreenSplit>();
            gs.centerPos = transform.position;
            gs.maxRad = rad / 3F;
            gs.phaseVel = phaseVel;
            gs.angle = i * deltaAngle;
            gs.angularVel = angVel / 3F;
        }

        var be = new BgrEffect(BgrEffect.Type.Ring, new Color(0.6800F,0.8113F,0.1415F), 0.6F, 6F, 0.9F,
            (Vector2)transform.position);
        FindObjectOfType<BackgroundSystem>().AddEffect(be);

        yield return new WaitForSeconds(Mathf.PI * 2F / phaseVel);

        deltaAngle = Mathf.PI  / (float)N;
        for (int i = 0; i < 2*N; i++)
        {
            var gs = GameObject.Instantiate(GreenSplitObj, transform.position, Quaternion.identity)
                .GetComponent<GreenSplit>();
            gs.centerPos = transform.position;
            gs.maxRad = rad / 2F;
            gs.phaseVel = phaseVel;
            gs.angle = i * deltaAngle;
            gs.angularVel = angVel / 2F;
        }
        be = new BgrEffect(BgrEffect.Type.Ring, new Color(0.8509F, 0.9764F, 0.3372F), 1.2F, 9F, 1.2F,
            (Vector2)transform.position);
        FindObjectOfType<BackgroundSystem>().AddEffect(be);
        yield return new WaitForSeconds(Mathf.PI * 2F / phaseVel);

        deltaAngle = Mathf.PI * 0.66667F / (float)N;
        for (int i = 0; i < 3 * N; i++)
        {
            var gs = GameObject.Instantiate(GreenSplitObj, transform.position, Quaternion.identity)
                .GetComponent<GreenSplit>();
            gs.centerPos = transform.position;
            gs.maxRad = rad;
            gs.phaseVel = phaseVel;
            gs.angle = i * deltaAngle;
            gs.angularVel = angVel;
        }
        be = new BgrEffect(BgrEffect.Type.Ring, new Color(0.8509F, 0.9764F, 0.3372F), 1.6F, 9F, 1.2F,
            (Vector2)transform.position);
        FindObjectOfType<BackgroundSystem>().AddEffect(be);
        
        yield return new WaitForSeconds(Mathf.PI * 2F / phaseVel);

        be = new BgrEffect(BgrEffect.Type.Ring, new Color(0.8509F, 0.9764F, 0.3372F), 0.8F, 8F, 1.0F,
            (Vector2)transform.position);
        FindObjectOfType<BackgroundSystem>().AddEffect(be);
        
        Destroy(gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        GS = FindObjectOfType<GameSystem>();
        GreenSplitObj = Resources.Load<GameObject>("Prefabs/Enemies/Green/GreenSplit");
        StartCoroutine(GreenBurstCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (!GS.Gaming())
           Destroy(gameObject);
    }
}
