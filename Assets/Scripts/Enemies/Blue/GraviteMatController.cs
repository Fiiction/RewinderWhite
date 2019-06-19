using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraviteMatController : MonoBehaviour
{
    public float freq = 1.0F;
    float G = 1F;
    Material mat;
    bool flag = false;
    GameObject obj;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<SpriteRenderer>().material;
        obj = Resources.Load<GameObject>("Prefabs/BlueGraviteMat");
    }

    // Update is called once per frame
    void Update()
    {
        G += Time.deltaTime * freq;
        
        if(flag)
        {
            mat.SetFloat("_Alpha", (2.5F - G) / 0.5F);
            if (G >= 2.5)
                Destroy(gameObject);
        }
        else
        {
            mat.SetFloat("_G", Mathf.Sqrt(G));
            mat.SetFloat("_Alpha", 1);
            if (G >= 2)
            {
                flag = true;
                GameObject.Instantiate(obj, transform.parent);
            }
        }

    }
}
