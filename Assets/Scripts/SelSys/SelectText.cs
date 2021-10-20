using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectText : MonoBehaviour
{
    [SerializeField]
    GameObject srl;
    RectTransform srlRect;
    FileSelecter Select;
    FileLoader Loader;
    public TextMeshProUGUI TMPTITLE, TMPARTIST, TMPBPM, TMPDIFF, TMPDIFF2, TMPSPD, TMPMOD, TMPSORT, TMPCHART, TMPNOTE, TMPLN, TMPLENGTH, TMPFIXED, TMPSPC, TMPTUNES, TMPPNAME;
    int maxvalue;
    // Start is called before the first frame update
    void Start()
    {
        Select = GameObject.FindWithTag("SelSys").GetComponent<FileSelecter>();
        Loader = GameObject.FindWithTag("FileSys").GetComponent<FileLoader>();
        srlRect = srl.GetComponent<RectTransform>();
        TMPTUNES.text = Manager.keycount + "Key TUNES";
        TMPPNAME.text = "Player Name : " + Manager.playername;
    }

    // Update is called once per frame
    void Update()
    {
        maxvalue = Loader.list.Count - 1;
        TMPTITLE.text = Select._name;
        TMPARTIST.text = Select._artist;
        TMPBPM.text = $"bpm : {Select.medianBPM} ({Select.minBPM} ~ {Select.maxBPM})";
        TMPDIFF.text = TMPDIFF2.text = $"[{Select._diff}]";
        string str = "NONE";
        if (Manager.Random) str = "RANDOM";
        if (Manager.Mirror) str = "MIRROR";
        TMPMOD.text = str;
        TMPSORT.text = "TITLE";
        if (Manager.sortselection == 1) TMPSORT.text = "ARTIST";
        if (Manager.sortselection == 2) TMPSORT.text = "minLV";
        if (Manager.sortselection == 3) TMPSORT.text = "maxLV";
        if (Manager.sortselection == 4) TMPSORT.text = "NPS";
        TMPSPC.text = "NONE";
        if (Manager.AutoPlay) TMPSPC.text = "AUTOPLAY";

        TMPSPD.text = "Scroll Speed : " + Mathf.Round(Manager.scrollSpeed * 10f - 8f);

        TMPCHART.text = "Noter : " + Select._charter;
        TMPLENGTH.text = $"Length : {NowPlaying.LENGTH}";

        TMPNOTE.text = $"Note counts : {NowPlaying.NOTECOUNTS}";
        TMPLN.text = $"Longnote counts : {NowPlaying.LONGNOTECOUNTS}";

        TMPFIXED.text = "F3 / F4";
        float per = (float)Manager.decide / maxvalue;
        float v = 120f - maxvalue * 0.5f;
        if (v < 30f) v = 30f;
        srlRect.anchoredPosition = new Vector2(-44.2f, 213.7f - v / 2f - (387.7f - v) * per);

        srlRect.sizeDelta = new Vector2(8, v);
    }
    public void OnValueChanged()
    {
        //float value = srl.value;
        //int select = Mathf.RoundToInt(Loader.list.Count * value);
        //Debug.Log(select);
        //StartCoroutine(Selector(select));
    }
    IEnumerator Selector(int select)
    {
        if (Manager.decide > select)
        {
            while (Manager.decide > select)
            {
                Select.SongScroll();
                Manager.decide--;
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (Manager.decide < select)
            {
                Select.SongScroll();
                Manager.decide++;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
