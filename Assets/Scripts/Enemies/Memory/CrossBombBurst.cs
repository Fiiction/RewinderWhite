using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossBombBurst : MonoBehaviour
{
    GameObject CrossBombParticlePrefab;
    public Color color;
    void ShootParticle(Vector2 vec, int i)
	{
        Vector2 vD = new Vector2(vec.y, -vec.x).normalized * 0.5f;
        if (i % 2 == 0)
            vD *= ((((i / 2) -1) % 5) * 0.5f - 1.5f);
        else
            vD *= Random.Range(-1f, 1f);
        var obj = GameObject.Instantiate(CrossBombParticlePrefab, transform.position + (Vector3)vD, Quaternion.identity);
        obj.GetComponent<CrossBombParticle>().vec = vec;
        obj.GetComponent<EnemyController>().color = color;
    }

    IEnumerator BurstCoroutine()
	{
        Vector2[] vecs = { Vector2.up, Vector2.left, Vector2.right, Vector2.down };
        for(int i = 1; i <= 30; i++)
		{
            yield return new WaitForSeconds(0.02f);
            foreach(var v in vecs)
			{
                ShootParticle(v, i);
			}
		}
        Destroy(gameObject);
	}

    // Start is called before the first frame update
    void Start()
    {
        CrossBombParticlePrefab = Resources.Load<GameObject>("Prefabs/Enemies/Memory/CrossBombParticle");
        StartCoroutine(BurstCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
