using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blue : MonoBehaviour
{
    public float gPower = 30F, speed = 3F, dist, totDist;
    float phase;
    AudioSource audioSource;
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
        //Destroy(gameObject, transform.position.magnitude * 2F / speed);
        string audioName = "";
        if (transform.position.x > 1f)
            audioName = "Audios/blackhole_right";
        if (transform.position.x < -1f)
            audioName = "Audios/blackhole_left";
        if (transform.position.y > 1f)
            audioName = "Audios/blackhole_up";
        if (transform.position.y < -1f)
            audioName = "Audios/blackhole_down";
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = 0f;
        audioSource.clip = Resources.Load<AudioClip>(audioName);
        audioSource.Play();
        phase = 0f;
        dist = transform.position.magnitude * 2F;
        totDist = dist;
        vel = -transform.position.normalized * speed;
        EC = GetComponent<EnemyController>();

        fakeBlock = Resources.Load<GameObject>("Prefabs/BlueFakeBlock");
        StartCoroutine(FakeBlockCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        Gravite();
        transform.position += vel * Time.deltaTime;
        dist -= speed * Time.deltaTime;
        phase = dist / totDist;
        audioSource.volume = Mathf.Clamp(1.5f - 3f * Mathf.Abs(phase - 0.5f), 0f, 0.7f);
        audioSource.panStereo = Mathf.Clamp(transform.position.x / 14f, -1f, 1f);
        if (dist <= 0F)
            EC.Kill();
    }
}
