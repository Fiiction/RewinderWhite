using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Green : MonoBehaviour
{
    public float speed = 5F;
    float speedRate = 1F;
    bool damping = false;
    Vector2 targetPos = Vector2.zero, vec = Vector2.zero;
    float burstTime;
    EnemyController EC;

    IEnumerator GreenCoroutine()
    {
        yield return new WaitForSeconds(burstTime);
        EC.dripFreq = 12F;
        EC.dripRad = 6F;
        damping = true;
        yield return new WaitForSeconds(1.5F);
        GameObject GB = Resources.Load<GameObject>("Prefabs/Enemies/Green/GreenBurst");
        GameObject gb = GameObject.Instantiate(GB, transform.position, Quaternion.identity);
        EC.Kill(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        EC = GetComponent<EnemyController>();
        if (EC.player)
            targetPos = EC.player.transform.position;
        vec = (targetPos - (Vector2)transform.position).normalized * speed;
        burstTime = (targetPos - (Vector2)transform.position).magnitude / speed - 0.5F;
        EC.vel = vec;
        StartCoroutine(GreenCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        if(damping)
        {
            speedRate -= Time.deltaTime / 1.5F;
            transform.position += (Vector3)vec * speedRate * Time.deltaTime;
            EC.vel = vec * speedRate;
        }
        else
            transform.position += (Vector3)vec * Time.deltaTime;
    }
}
