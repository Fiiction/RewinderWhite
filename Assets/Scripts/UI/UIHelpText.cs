using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIHelpText : MonoBehaviour
{
    GameSystem GS;
    Image im;
    float alpha = 0;
    // Start is called before the first frame update
    void Start()
    {
        GS = FindObjectOfType<GameSystem>();
        im = GetComponent<Image>();
        im.color = new Color(1, 1, 1, 0);
        im.raycastTarget = false;
    }

    public void SetHelp()
	{
        Sequence sq = DOTween.Sequence();
        sq.AppendInterval(0.5f);
        sq.Append(im.DOFade(1f, 1f));
        sq.AppendInterval(4.5f);
        sq.Append(im.DOFade(0f, 1f));
	}

    // Update is called once per frame
    void Update()
    {

    }
}
