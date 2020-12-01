using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : MonoBehaviour
{
    public Dictionary<string, int> Index;
    public Dictionary<string, int> Length;

    public enum LoopType { Loop, Random, None};

    string[] mainLoopTags = { "r", "g", "w" };
    string bufferedTag = "";
    int bufferedTagIndex = -1;
    int mainLoopIndex = 0;

    GameSystem GS;
    GameObject[] TensionAudios = { null, null };
    float tensionRate = 0f;
    public void PlayMainLoop(string tag)
	{
        int curIndex = -1;
        for(int i = 0;i<mainLoopTags.Length;i++)
            if(mainLoopTags[i] == tag)
                curIndex = i;
        if (curIndex > bufferedTagIndex)
        {
            bufferedTag = tag;
            bufferedTagIndex = curIndex;
        }
	}

    void HandleMainLoopBuffer()
	{
        if (bufferedTagIndex == -1)
            return;
        PlayAudio("collision",bufferedTag,LoopType.Loop);
        bufferedTagIndex = -1;
	}

    string Path(string loopName, string branchName = "", int index = 0)
    {
        string path;
        path = "Audios/" + loopName;
        if (branchName != "")
            path += "_" + branchName;
        if (index > 0)
            path += "_" + index.ToString();
        Debug.Log("Try: " + path);
        return path;
    }
    public void PlayAudio(string loopName, LoopType loopType = LoopType.None, float volume = 1f)
    {
        PlayAudio(loopName, "", loopType, volume);
    }
    public void PlayAudio(string loopName, string branchName,  LoopType loopType = LoopType.None, float volume = 1f)
	{
        AudioClip ac = null;
        string path;
        switch(loopType)
		{
            case LoopType.Loop:
                Index[loopName] = (Index[loopName] %Length[loopName]) +1;
                path = Path(loopName, branchName, Index[loopName]);
                ac = Resources.Load<AudioClip>(path);
                break;
            case LoopType.Random:
                int rIndex = Mathf.CeilToInt(Random.Range(0f, (float)Length[loopName]));
                while(rIndex == Index[loopName])
                    rIndex = Mathf.CeilToInt(Random.Range(0f, (float)Length[loopName]));
                Index[loopName] = rIndex;
                path = Path(loopName, branchName, Index[loopName]);
                ac = Resources.Load<AudioClip>(path);
                break;
            case LoopType.None:
                path = Path(loopName, branchName, 0);
                ac = Resources.Load<AudioClip>(path);
                break;
            default:
                ac = null;
                break;
        }
        InstantiateAudio(ac, volume);
	}

    public void InstantiateAudio(AudioClip ac, float volume = 1f)
	{
        if (!ac)
            return;
        if (volume <= 0f)
            return;
        Debug.Log("Play: "+ac.name);
        GameObject obj = new GameObject(ac.name);
        obj.transform.SetParent(transform);
        AudioSource audioSource = obj.AddComponent<AudioSource>();
        audioSource.clip = ac;
        audioSource.Play();
        audioSource.volume = volume;
        Destroy(obj, ac.length);
	}

    IEnumerator ThemeCoroutine()
	{
        float[] delay = { 0f, 8.35f, 11.87f, 12.17f, 12.54f };

        while (true)
		{
            if(GS.Gaming())
			{
                float volume = 1f - 0.4f * tensionRate;
                PlayAudio("piano", LoopType.Random, volume);
                yield return new WaitForSeconds(delay[Index["piano"]]);
			}
            else
                yield return new WaitForSeconds(0.2f);
        }
	}
    IEnumerator TensionCoroutine()
    {
        float delay = 6.84f;
        int cnt = 0;
        while (true)
        {
            if (GS.Gaming())
            {
                GameObject obj = new GameObject("tension");
                obj.transform.SetParent(transform);
                TensionAudios[cnt % 2] = obj;
                obj.transform.SetParent(transform);
                AudioSource audioSource = obj.AddComponent<AudioSource>();
                var ac = Resources.Load<AudioClip>("Audios/tension");
                audioSource.clip = ac;
                audioSource.Play();
                audioSource.volume = tensionRate;
                Destroy(obj, ac.length);
                yield return new WaitForSeconds(delay);
            }
            else
                yield return new WaitForSeconds(0.2f);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        Index = new Dictionary<string, int>();
        Length = new Dictionary<string, int>();

        GS = FindObjectOfType<GameSystem>();
        foreach (var i in mainLoopTags)
            Index[i] = 0;
        Length["absorb"] = 4;
        Length["piano"] = 4;
        Length["collision"] = 2;

        foreach(var i in Length)
            Index[i.Key] = 0;

        StartCoroutine(TensionCoroutine());
        StartCoroutine(ThemeCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        HandleMainLoopBuffer();
        tensionRate += (GS.tension - tensionRate) * 2f * Time.deltaTime;
        foreach(var i in TensionAudios)
		{
            if(i && i.GetComponent<AudioSource>())
                i.GetComponent<AudioSource>().volume = Mathf.Clamp01(tensionRate - 0.3f);
		}
    }
}
