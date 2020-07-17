using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchDebugger : MonoBehaviour
{
    Text text;
    string str;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        str = "";
        str = Input.touches.Length.ToString()+"\n";
        foreach(Touch i in Input.touches)
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(i.position);
            str += "(" + pos.x.ToString("0.#") + ", " + pos.y.ToString("0.#") + ")\n";
        }
        text.text = str;
    }
}
