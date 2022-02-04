using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MemoryLevelButtonEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Color basicColor;
    public int index;
    public bool mouseDown = false;
    bool working = false;
    bool touching = false;
    public float power = 1F;
    GameObject MemoriesMeteorPrefab;
    GameSystem GS;

    IEnumerator GenerateMeteorCoroutine()
    {
        Color color;
        float fade;
        float strength,x;
        float phase = 0F;
        yield return new WaitForSeconds(index * 0.25F);

        while (GS.state == GameSystem.State.Memory)
        {
            fade = Random.Range(0F, 1F);

            float fadePower;
            if (power > 0.7F)
                fadePower = 0.2F;
            else if (power > 0.4F)
                fadePower = 0.2F + 30 * (0.7F - power);
            else
                fadePower = 6F;

            fade = Mathf.Pow(fade, fadePower);

            if (fade < 0.5F)
                fade /= 4F;
            else
                fade = 1F - (1F - fade) / 4F;


            color = basicColor * fade + Color.black * (1 - fade);
            strength = Random.Range(0.2F, 0.4F) * Mathf.Sqrt(power);
            x = (index - 4) * 4F + Random.Range(-2F, 2F);

            var obj1 = GameObject.Instantiate(MemoriesMeteorPrefab);
            obj1.GetComponent<MemoryMeteor>().Set(color, strength, x, -1, 2F + power * 3F, this);
            var obj2 = GameObject.Instantiate(MemoriesMeteorPrefab);
            obj2.GetComponent<MemoryMeteor>().Set(color, strength, x, 1, 2F + power * 3F, this);


            power *= 0.96F;

            if (touching)
                power = 1.2F;
            //yield return new WaitForSeconds(0.16F / Mathf.Clamp(power, 1F, 2F));
            while(phase <1F)
            {
                phase += power * power * Time.deltaTime * 5F;
                yield return 0;
            }
            phase = 0F;
        }
    }

    IEnumerator GenerateRingCoroutine()
    {
        Color color;
        float fade;
        float strength, rad,life;
        Vector2 basicPos = Vector2.right * 4F * (index - 4F),pos;
        //Debug.Log(index);
        working = true;
        float phase = 0F;

        yield return new WaitForSeconds((index - 1F) * 0.35F);
        power = 1F;
        while (GS.state == GameSystem.State.Memory)
        {

            if (touching)
                power = 1.4F;

            fade = Random.Range(0F, 1F);

            float fadePower;
            if (power > 0.6F)
                fadePower = 0.2F;
            else if (power > 0.3F)
                fadePower = 0.2F + 30 * (0.6F - power);
            else
                fadePower = 6F;

            fade = Mathf.Pow(fade, fadePower);

            if (fade < 0.5F)
                fade /= 4F;
            else
                fade = 1F - (1F - fade) / 4F;


            color = basicColor * fade + Color.black * (1 - fade);
            strength = Random.Range(0.2F, 0.4F) * Mathf.Sqrt(power);
            rad = 9F + strength * 12F;
            life = 4F;
            pos = basicPos + Random.Range(-7F, 7F) * Vector2.left + Random.Range(-7F, 7F) * Vector2.up;

            if (Random.Range(0F, 1F) < 2F *power * power)
            {
                FindObjectOfType<BackgroundSystem>().AddEffect(
                new BgrEffect(BgrEffect.Type.MemRing, color, strength, rad, life, pos, index));
            }

            power *= 0.96F;

            while (phase < 1F)
            {
                phase += power * power * Time.deltaTime * 5F;
                if (touching && power <= 1F)
                    break;
                yield return 0;
            }
            phase = 0F;
        }
        working = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        MLBManager.MLBDict.Add(index, this);
        MemoriesMeteorPrefab = Resources.Load<GameObject>("Prefabs/UI/MemoriesMeteor");
        GS = FindObjectOfType<GameSystem>();
    }

    void CheckTouch()
    {
        bool t = false;
        Touch[] touches = Input.touches;
        foreach(Touch i in touches)
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(i.position);
            if (pos.x > -18F + 4F * index && pos.x < -14F + 4F * index)
                t = true;
        }
        touching = t || mouseDown;
    }
    // Update is called once per frame
    void Update()
    {
        if (GS.state == GameSystem.State.Memory && !working && GS.GetMemoryFastestFinish(index) < 9999f)
            StartCoroutine(GenerateRingCoroutine());
        CheckTouch();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        mouseDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        mouseDown = false;
    }
}
