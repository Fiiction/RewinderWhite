using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InflElement : MonoBehaviour
{
    public Inflator mother;
    public int index = 1;
    public float basicAngle, curAngle, delta;

    public void Kill()
    {
        Debug.DrawLine(transform.position, mother.transform.position);
        GetComponent<EnemyController>().Kill();
    }
    public Vector3 InitPos()
    {
        basicAngle = 2F * Mathf.PI * index / mother.elemCnt + mother.theta;
        basicAngle -= Mathf.Floor(basicAngle / (2F * Mathf.PI) + 0.5F) * (2F * Mathf.PI);
        curAngle = basicAngle;
        return 0.2F * mother.curRad * new Vector2(Mathf.Cos(curAngle), Mathf.Sin(curAngle));
    }
    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = InitPos();
    }

    // Update is called once per frame
    void Update()
    {
        basicAngle = 2F * Mathf.PI * index / mother.elemCnt + mother.theta;
        basicAngle -= Mathf.Floor(basicAngle / (2F * Mathf.PI) + 0.5F) * (2F * Mathf.PI);
        delta = basicAngle - curAngle;
        delta -= Mathf.Floor(delta / (2F * Mathf.PI) + 0.5F) * (2F * Mathf.PI);
        curAngle += delta * Time.deltaTime * 2F;
        transform.localPosition = 0.2F * mother.curRad *
            new Vector2(Mathf.Cos(curAngle), Mathf.Sin(curAngle));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            var c = collision.gameObject.GetComponent<EnemyController>();
            if (c.timeAlive < 0.5F)
                return;
            mother.Damage();

            if (mother.targetSize>=8F)
                mother.targetSize -= 3F;
            else if (mother.targetSize >= 4F)
                mother.targetSize -= 1.2F;
        }
    }
}
