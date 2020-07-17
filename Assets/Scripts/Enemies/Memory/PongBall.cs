using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongBall : MonoBehaviour
{
    public float speed;
    const float XMAX = 12F, YMAX = 7.68F;
    public Vector2 vel = Vector2.zero;
    public PongManager manager;
    bool starting = true;
    EnemyController EC;
    BackgroundSystem BS;

    IEnumerator StartReboundCoroutine()
    {
        yield return new WaitForSeconds(0.6F);
        starting = false;
        HorizontalRebound();
    }

    float lastVRTime = -1F;
    void VerticleRebound()
    {
        if (Time.time <= lastVRTime + 0.2F)
            return;
        lastVRTime = Time.time;
        vel.y = -vel.y;
        var be = new BgrEffect(BgrEffect.Type.OutSide, EC.color, 0.5F, 8F, 0.6F, (Vector2)(transform.position * 0.85F));
        BS.AddEffect(be);
    }

    int reboundCnt;
    public void HorizontalRebound()
    {
        if (starting)
            return;
        reboundCnt++;
        if (reboundCnt > 8 && speed <= 14F)
            speed *= 1.1F;
        Vector2 targetPos = EC.player.transform.position;
        if (targetPos.x > 10.6F)
            targetPos.x = 10.6F;
        if (targetPos.x < -10.6F)
            targetPos.x = -10.6F;
        vel = (targetPos - (Vector2)transform.position).normalized;
        var be = new BgrEffect(BgrEffect.Type.Focus, EC.color, 0.7F, 5F, 0.6F, (Vector2)EC.player.transform.position);
        BS.AddEffect(be);
    }


    // Start is called before the first frame update
    void Start()
    {
        EC = GetComponent<EnemyController>();
        BS = FindObjectOfType<BackgroundSystem>();
        StartCoroutine(StartReboundCoroutine());
    }

    void Move()
    {
        if (starting)
            return;
        transform.position += speed * (Vector3)vel * Time.deltaTime;
        if (vel == Vector2.zero && EC.player)
            vel = (EC.player.transform.position - transform.position).normalized;
    }
    // Update is called once per frame
    void Update()
    {
        Move();
        if (Mathf.Abs(transform.position.x) <= XMAX && Mathf.Abs(transform.position.y) >= YMAX)
            VerticleRebound();
    }

    public void Kill()
    {
        EC.Kill();
        Destroy(gameObject, 1F);
    }
}
