using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class scrSetText : MonoBehaviour
{
    FileSelecter Select;
    public TextMeshProUGUI TMPTITLE, TMPARTIST, TMPBPM, TMPDIFF, TMPSPD, TMPMOD, TMPSORT, TMPCHART, TMPNOTE, TMPLN, TMPLENGTH, TMPFIXED, TMPSPC;
    
    // Start is called before the first frame update
    void Start()
    {
        Select = GameObject.FindWithTag("SelSys").GetComponent<FileSelecter>();
    }

    // Update is called once per frame
    void Update()
    {
        TMPTITLE.text = Select._name;
        TMPARTIST.text = Select._artist;
        TMPBPM.text = $"bpm : {Select.medianBPM} ({Select.minBPM} ~ {Select.maxBPM})";
        TMPDIFF.text = "Level : " + Select._diff;
        string str = "NONE";
        if (scrSetting.Random) str = "RANDOM";
        if (scrSetting.Mirror) str = "MIRROR";
        TMPMOD.text = str;
        TMPSORT.text = "by NAME";
        if (scrSetting.sortselection == 1) TMPSORT.text = "by ARTIST";
        if (scrSetting.sortselection == 2) TMPSORT.text = "by LEVEL";
        TMPSPC.text = "NONE";
        if (scrSetting.AutoPlay) TMPSPC.text = "AUTOPLAY";

        TMPSPD.text = "Scroll Speed : " + Mathf.Round(scrSetting.scrollSpeed * 10f - 8f);

        TMPCHART.text = "Noter : " + Select._charter;
        TMPLENGTH.text = $"Length : {NowPlaying.LENGTH}";

        TMPNOTE.text = $"Note counts : {NowPlaying.NOTECOUNTS}";
        TMPLN.text = $"Longnote counts : {NowPlaying.LONGNOTECOUNTS}";
       

        TMPFIXED.text = "F3 / F4";
        if (!scrSetting.isFixedScroll) TMPFIXED.text = "F3 / F4        [FIXED]";
    }
}
