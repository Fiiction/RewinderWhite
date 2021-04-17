using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindMark : MonoBehaviour
{
    public float alpha = 0F;
    public Player PL;
    public Violet Vi;
    public Color color;
    public DropGraphics DG;
    public float tailLength = 0.91F;
    bool prevCan = false;
    GameObject HollowDrop;
    GameSystem GS;
    DropGraphics hdg;
    // Start is called before the first frame update
    void Start()
    {
        GS = FindObjectOfType<GameSystem>();
        PL = GetComponentInParent<Player>();
        Vi = GetComponentInParent<Violet>();
        DG = GetComponentInChildren<DropGraphics>();
        transform.SetParent(null);
        DG.tailLength = tailLength;

        HollowDrop = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/DropGraphics"), transform);
        hdg = HollowDrop.GetComponent<DropGraphics>();
        hdg.basicColor = new Color(1F, 1F, 1F, 0F);
        hdg.tailLength = tailLength;
        hdg.lineWidth = 0F;
        hdg.lineLength = 0F;
        hdg.GetComponent<SpriteRenderer>().sortingOrder += 2;
        hdg.transform.Find("Tail").GetComponent<TrailRenderer>().sortingOrder += 2;
        HollowDrop.transform.localScale = new Vector3(0.7F,0.7F,1F);
    }

    // Update is called once per frame
    void Update()
    {
        if (PL)
        {
            if (PL.canRewind)
            {
                transform.position = PL.rewindPos;
                if (!prevCan)
                {
                    alpha = 0.6F;
                    DG.tailLength = 1F;
                    prevCan = true;
                    Debug.Log("Refresh!");
                }
            }
            if(!PL.canRewind)
            {
                alpha = -0.1F;
                if (prevCan)
                {
                    transform.position = GS.PlayerPos();
                }
            }
            prevCan = PL.canRewind;
        }
        if(Vi)
        {
            if(Vi.canRewind)
            {
                transform.position = Vi.rewindPos;
            }
            if (Vi.canRewind && !prevCan)
            {
                alpha = 0.6F;
                prevCan = true;
                Debug.Log("Refresh!");
            }
            if (!Vi.canRewind)
            {
                alpha = -0.1F;
            }
            prevCan = Vi.canRewind;
        }
        if (!PL && !Vi && alpha <= 0F)
            Destroy(gameObject);
        Color c = color;
        c.a = Mathf.Clamp(alpha,0F,0.3F) * 0.7F;
        alpha -= Time.deltaTime * 0.4F;
        /*
        if (alpha <= 0F)
            DG.tailLength = 0F;
        else
            DG.tailLength = tailLength;
            */
        DG.basicColor = c;
        hdg.basicColor = new Color(1F, 1F, 1F, Mathf.Clamp(alpha / 0.21F, 0F, 1F));
    }
}
