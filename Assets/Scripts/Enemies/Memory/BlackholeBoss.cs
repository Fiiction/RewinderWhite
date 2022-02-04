using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class BlackholeBoss : MonoBehaviour
{
    public float scale = 1f;
    int curTargetPosIndex = -1;
    Vector2 moveVec;
    float distToGo = -10f;
    Vector2[] targetPoses =
        { new Vector2 (12f, 0f),new Vector2 (-12f, 8f),new Vector2 (0f, -8f),new Vector2 (12f, 8f),
        new Vector2 (-12f, 0f),new Vector2 (12f, -8f),new Vector2 (0f, 8f),new Vector2 (-12f, -8f)};
    public float speed = 3F, acceleration = 1f;
    public float basicGPower = 16f;
    Vector3 basicScale;
    AudioSource audioSource;
    EnemyController EC;
    GameSystem GS;
    GameObject fakeBlock;
    GameObject misslePrefab;
    public List<BlackholeMissle> blackholeMissles = new List<BlackholeMissle>();
    void GEffect(GameObject o)
    {
        if (!o)
            return;
        float dist = (transform.position - o.transform.position).magnitude;
        Vector3 vec = (transform.position - o.transform.position).normalized;
        float gScale = 1f;
        if (scale >= 1f)
            gScale = Mathf.Pow(scale, 1.5f);
        else
            gScale = scale;
        o.transform.position = o.transform.position + vec * basicGPower * gScale * Time.deltaTime / Mathf.Pow(dist, 1.3F);
    }
    void Gravite()
    {
        Red[] reds = FindObjectsOfType<Red>();
        Orange[] oranges = FindObjectsOfType<Orange>();
        foreach (var i in reds)
            GEffect(i.gameObject);
        foreach (var i in oranges)
            GEffect(i.gameObject);
        foreach (var i in blackholeMissles)
            GEffect(i.gameObject);

        var o = FindObjectOfType<Player>();
        if (o)
            GEffect(o.gameObject);

    }

    IEnumerator FakeBlockCoroutine()
    {
        float angle = 0F, dist = 0F;
        Vector3 pos;
        GameObject obj;
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.05F, 0.3F) / scale);
            angle += Random.Range(0.5F, 1.5F) * Mathf.PI;
            dist = Random.Range(1F, 5F) * scale;
            pos = transform.position + (Vector3)curSpeedVec * speed * 0.7F + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;
            pos = new Vector3(Mathf.Floor(pos.x) + 0.5F, Mathf.Floor(pos.y) + 0.5F);
            obj = GameObject.Instantiate(fakeBlock, pos, Quaternion.identity);
            var bfb = obj.GetComponent<BlueFakeBlock>();
            bfb.mother = gameObject;
            bfb.SetColor(EC.color);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GS = FindObjectOfType<GameSystem>();
        string audioName = "Audios/blackhole_right";
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = 0f;
        audioSource.clip = Resources.Load<AudioClip>(audioName);
        audioSource.Play();
        audioSource.loop = true;
        basicScale = transform.localScale;
        EC = GetComponent<EnemyController>();
        EC.OnKillingOther.AddListener(OnKilling);
        fakeBlock = Resources.Load<GameObject>("Prefabs/BlueFakeBlock");
        misslePrefab = Resources.Load<GameObject>("Prefabs/Enemies/Memory/BlackholeMissle");
        StartCoroutine(FakeBlockCoroutine());
    }

    public float missleCD = 5f;
    public int desiredMissleCnt = 1;

    float lastHitTime = 0f;

    void ChangeScale(float _scale)
	{
        Vector3 curScale = scale * basicScale;
        Vector3 midScale = (2f * _scale - 1f * scale) * basicScale;
        Vector3 toScale = _scale * basicScale;
        scale = _scale;
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(midScale, 0.15f));
        seq.Append(transform.DOScale(toScale, 0.15f));
        seq.Play();
    }
    void HitByMissle()
	{
        if (Time.time < lastHitTime + 0.1f)
            return;
        lastHitTime = Time.time;
        ChangeScale(scale * 0.75f);
	}

    void OnKilling(EnemyController otherEC)
	{
        if (otherEC.gameObject.name.Contains("Missle"))
            HitByMissle();
        else
            ChangeScale(scale + 0.2f);
	}

    void GenerateMissle()
    {
        if (GS.bossHealth <= 90f)
            desiredMissleCnt = 2;
        if (GS.bossHealth <= 60f)
            desiredMissleCnt = 3;
        float angle = Random.Range(-2F, 2F) / 6F * Mathf.PI;
        Vector3 pos = new Vector3(28 * Mathf.Cos(angle), 17 * Mathf.Sin(angle));
        if (transform.position.x > 0f)
            pos.x = -pos.x;
        var obj = GameObject.Instantiate(misslePrefab, pos, Quaternion.identity);
        var mis = obj.GetComponent<BlackholeMissle>();
        mis.boss = this;
        blackholeMissles.Add(mis);

    }

    void CheckGenerateMissle()
    {
        if (desiredMissleCnt > blackholeMissles.Count)
            missleCD -= Time.deltaTime;
        if (missleCD <= 0f)
        {
            GenerateMissle();
            missleCD = 6f - 0.03f * GS.score;
        }
    }

    Vector2 curSpeedVec = Vector2.zero;
    void SetTargetPos()
    {
        curTargetPosIndex++;
        if (curTargetPosIndex >= targetPoses.Length)
            curTargetPosIndex %= targetPoses.Length;

        curSpeedVec = (targetPoses[curTargetPosIndex] - (Vector2)transform.position).normalized;
        distToGo = (targetPoses[curTargetPosIndex] - (Vector2)transform.position).magnitude;
    }

	void Move()
	{
        if (distToGo <= 0f)
            SetTargetPos();
        transform.position += speed * Time.deltaTime * (Vector3)curSpeedVec;
        distToGo -= speed * Time.deltaTime;
    }


    void CheckAudio()
	{
        float t = audioSource.time;
        float maxT = audioSource.clip.length;
        if (t < 2f)
            audioSource.volume = 0.6f * t / 2f;
        else if (maxT - t < 2f)
            audioSource.volume = 0.6f * (maxT - t) / 2f;
        else
            audioSource.volume = 0.6f;

    }

	//   void Move()
	//{
	//       Vector2 vec = (GS.PlayerPos() - transform.position).normalized;
	//       curSpeedVec = Vector2.Lerp(curSpeedVec, vec, Time.deltaTime * acceleration);
	//       if (curSpeedVec.sqrMagnitude > 1f)
	//           curSpeedVec.Normalize();
	//       transform.position += speed * Time.deltaTime * (Vector3)curSpeedVec;
	//}

	// Update is called once per frame
	void Update()
    {
        Move();
        CheckGenerateMissle();
        Gravite();
        CheckAudio();
        audioSource.panStereo = Mathf.Clamp(transform.position.x / 14f, -1f, 1f);
    }
}
