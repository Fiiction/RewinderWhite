using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class IntroWave : MonoBehaviour
{
    float rMax = 0;
    float amp = 0, spatialFreq = 3F, timeFreq = 20F;
    float sigma = 1.5F;
    public float step = 0.01F;
    LineRenderer LR;
    float GaussianDistribution(float x)
    {
        return (0.398942F / sigma) * Mathf.Exp(-x * x / (2 * sigma * sigma));
    }

    IEnumerator WaveCoroutine()
    {
        yield return new WaitForSeconds(1);


        DOTween.To(() => rMax, x => rMax = x, 6, 0.5F);
        DOTween.To(() => amp, x => amp = x, 13, 0.5F);
        yield return new WaitForSeconds(0.8F);
        DOTween.To(() => amp, x => amp = x, 0, 0.5F);
        yield return new WaitForSeconds(0.2F);
        DOTween.To(() => rMax, x => rMax = x, 0, 0.2F);
        yield return new WaitForSeconds(0.8F);

        spatialFreq = 10F;

        DOTween.To(() => rMax, x => rMax = x, 8, 0.5F);
        DOTween.To(() => amp, x => amp = x, 13, 0.5F);
        yield return new WaitForSeconds(0.8F);
        DOTween.To(() => amp, x => amp = x, 0, 0.5F);
        yield return new WaitForSeconds(0.2F);
        DOTween.To(() => rMax, x => rMax = x, 0, 0.2F);
        yield return new WaitForSeconds(0.8F);

        spatialFreq = 20F;

        DOTween.To(() => rMax, x => rMax = x, 12, 0.5F);
        DOTween.To(() => amp, x => amp = x, 13, 0.5F);
        yield return new WaitForSeconds(0.8F);
        DOTween.To(() => amp, x => amp = x, 0, 0.5F);
        yield return new WaitForSeconds(0.2F);
        DOTween.To(() => rMax, x => rMax = x, 0, 0.2F);
        yield return new WaitForSeconds(0.8F);
        yield return new WaitForSeconds(2F);

        //SceneManager.LoadScene(0);
    }


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaveCoroutine());
        LR = GetComponent<LineRenderer>();
    }

    void UpdateLine()
    {
        int stepCnt = (int)((2F * rMax) / step);
        Vector3[] pos = new Vector3[stepCnt];
        float x, y;
        for (int i = 0;i<stepCnt;i++)
        {
            x = -rMax + i * step;
            y = amp * GaussianDistribution(x) * Mathf.Cos(spatialFreq * x + timeFreq * Time.time);
            pos[i] = new Vector3(x, y);
        }
        LR.positionCount = stepCnt;
        LR.SetPositions(pos);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLine();
    }
}
