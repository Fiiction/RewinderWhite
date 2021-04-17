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
    int basicStrength;

    bool setted = false;
    public void Set()
    {
        if (setted)
            return;
        setted = true;
        Vector3 tar = Vector3.zero;
        tar.x = transform.position.y;
        tar.y = -transform.position.x;
        tar.Normalize();
        tar *= 2f;
        vel = (tar - transform.position).normalized;
        inScreen = false;
        basicStrength = EC.strength;
        EC.strength = basicStrength + 2;
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
        if((transform.position - GS.PlayerPos()).magnitude <= 5F)
        {
            expelled = true;
            return;
        }
        vel = (GS.PlayerPos() - transform.position).normalized;
        if(inScreen)
        {
            var be = new BgrEffect(BgrEffect.Type.BoundaryCircle, EC.color, 0.7F, 10F, 0.6F, (Vector2)(transform.position * 0.83F));
            FindObjectOfType<BackgroundSystem>().AddEffect(be);
        }
        FindObjectOfType<AudioSystem>().PlayMainLoop("w");
    }

    void Move()
    {
        transform.position += speed * (Vector3)vel * Time.deltaTime;
        if (vel == Vector2.zero && EC.player)
            Set();
    }

    // Update is called once per frame
    void Update()
    {
        if (!inScreen)
        {
            if (!OutOfBound())
                inScreen = true;
        }
        else if (OutOfBound())
            {
                if(EC.strength > basicStrength)
                    EC.strength --;
                Rebound();
            }

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
