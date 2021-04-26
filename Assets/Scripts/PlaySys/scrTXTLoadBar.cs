using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class scrTXTLoadBar : MonoBehaviour
{
    public SpriteRenderer r;
    GameObject o;
    public GameObject t;
    FileReader s;
    TextMeshPro tt;
    float ap = 1;
    // Start is called before the first frame update
    void Start()
    {
        o = GameObject.FindWithTag("NoteSys");
        s = o.GetComponent<FileReader>();
         tt = t.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        float ld = (s.progress) / (NowPlaying.NOTECOUNTS + NowPlaying.TIMINGCOUNTS);
        if (ld > 1f) ld = 1f;
        transform.localScale = new Vector2(ld / 2, 4f);
        if (ld > 0.99f)
        {
            ap -= Time.deltaTime * 1.3f;
        }
        r.color = new Color(1, 1, 1, ap);
        //tt.faceColor = r.color;
        if (ap <= 0) Destroy(gameObject);
    }
}
