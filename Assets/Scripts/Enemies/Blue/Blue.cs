using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blue : MonoBehaviour
{
    public float gPower = 30F, speed = 3F;
    Vector3 vel;
    EnemyController EC;
    GameObject fakeBlock;
    void GEffect(GameObject o)
    {
        if (!o)
            return;
        float dist = (transform.position - o.transform.position).magnitude;
        Vector3 vec = (transform.position - o.transform.position).normalized;
        o.transform.position = o.transform.position + vec * gPower * Time.deltaTime / Mathf.Pow(dist,1.3F);
    }
    void Gravite()
    {
        Red[] reds = FindObjectsOfType<Red>();
        Orange[] oranges = FindObjectsOfType<Orange>();
        foreach (var i in reds)
            GEffect(i.gameObject);
        foreach (var i in oranges)
            GEffect(i.gameObject);
        var o = FindObjectOfType<Player>();
        if(o)
            GEffect(o.gameObject);

    }

    IEnumerator FakeBlockCoroutine()
    {
        float angle = 0F, dist = 0F;
        Vector3 pos;
        GameObject obj;
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(0.1F, 0.5F));
            angle +=Random.Range(0.5F, 1.5F) * Mathf.PI;
            dist = Random.Range(1F, 4F);
            pos = transform.position + vel*0.7F + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;
            pos = new Vector3(Mathf.Floor(pos.x) + 0.5F, Mathf.Floor(pos.y) + 0.5F);
            obj = GameObject.Instantiate(fakeBlock, pos, Quaternion.identity);
            obj.GetComponent<BlueFakeBlock>().mother = gameObject;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, transform.position.magnitude * 2F / speed);
        vel = -transform.position.normalized * speed;
        EC = GetComponent<EnemyController>();
        EC.vel = vel;

        fakeBlock = Resources.Load<GameObject>("Prefabs/BlueFakeBlock");
        StartCoroutine(FakeBlockCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        Gravite();
        transform.position += vel * Time.deltaTime;
    }
}
