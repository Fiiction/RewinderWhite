using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Red : MonoBehaviour {
    
    public float speed;
    const float XMAX = 11.68F, YMAX = 7.68F;
    public Vector2 vel = Vector2.zero;
    EnemyController EC;
    GameSystem GS;
    float lifeTime = 0F;
    bool inScreen = false, expelled = false;
    float startTime;

    public void Set(Vector3 tar)
    {
        vel = speed * (tar - transform.position).normalized;
        inScreen = false;
    }

    // Use this for initialization
    void Start ()
    {
        GS = FindObjectOfType<GameSystem>();
        EC = GetComponent<EnemyController>();
        Generator.redCnt++;
        startTime = Time.time;
    }

    bool OutOfBound()
    {
        return transform.position.x > XMAX || transform.position.x < -XMAX
            || transform.position.y > YMAX || transform.position.y < -YMAX;
    }
    

    void Rebound()
    {
        if (!EC.player || expelled)
            return;
        if((transform.position - EC.player.transform.position).magnitude <= 5F)
        {
            expelled = true;
            return;
        }
        vel = (EC.player.transform.position - transform.position).normalized;
        if(inScreen)
        {
            var be = new BgrEffect(BgrEffect.Type.OutSide, EC.color, 0.5F, 8F, 0.6F, (Vector2)(transform.position * 0.85F));
            FindObjectOfType<BackgroundSystem>().AddEffect(be);
        }
    }

    void Move()
    {
        transform.position += speed * (Vector3)vel * Time.deltaTime;
        if (vel == Vector2.zero && EC.player)
            vel = (EC.player.transform.position - transform.position).normalized;
    }

    // Update is called once per frame
    void Update ()
    {
        if (!inScreen)
        {
            if (!OutOfBound())
                inScreen = true;
        }
        else
            if (OutOfBound())
                Rebound();

        Move();
        lifeTime += Time.deltaTime;
        if (lifeTime >= 10F && GS.autoKillEnemy)
            expelled = true;
        if (transform.position.magnitude >= 30F)
            Destroy(gameObject);
    }


    private void OnDestroy()
    {
        Generator.redCnt--;
    }
}
