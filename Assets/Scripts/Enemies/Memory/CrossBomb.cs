using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CrossBomb : MonoBehaviour
{
    EnemyController EC;
    GameSystem GS;
    GameObject CrossBombBurstPrefab;
    public Color color;
    IEnumerator CrossBombCoroutine()
    {
        startToBurst = true;
        var be = new BgrEffect(BgrEffect.Type.CrossBomb, EC.color, 0.32F, 10F, 3F, transform.position);
        FindObjectOfType<BackgroundSystem>().AddEffect(be);
        //yield return new WaitForSeconds(1.2f);
        transform.DOShakePosition(0.24f, new Vector3(1.2f, 1.2f, 0f), 200);
        yield return new WaitForSeconds(0.9f);
        transform.DOShakePosition(0.24f, new Vector3(2f, 2f, 0f), 200);
        yield return new WaitForSeconds(0.9f);
        var obj = GameObject.Instantiate(CrossBombBurstPrefab, transform.position, Quaternion.identity);
        obj.GetComponent<CrossBombBurst>().color = color;
        EC.Kill(false);
    }

	private void Awake()
    {
        GS = FindObjectOfType<GameSystem>();
        EC = GetComponent<EnemyController>();
        EC.color = color;
    }
	// Start is called before the first frame update
	void Start()
    {
        CrossBombBurstPrefab = Resources.Load<GameObject>("Prefabs/Enemies/Memory/CrossBombBurst");
        //StartCoroutine(CrossBombCoroutine());
        var be = new BgrEffect(BgrEffect.Type.CrossCircle, EC.color, 0.4F, 10F, 1F, transform.position);
        FindObjectOfType<BackgroundSystem>().AddEffect(be);
    }

    bool startToBurst = false;

    // Update is called once per frame
    void Update()
    {
        var pp = GS.PlayerPos();
        if(Mathf.Abs(pp.x-transform.position.x) < 0.6f || Mathf.Abs(pp.y - transform.position.y) < 0.6f)
		{
            if (!startToBurst)
                StartCoroutine(CrossBombCoroutine());
		}

    }
}
