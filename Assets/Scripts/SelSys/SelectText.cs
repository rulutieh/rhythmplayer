using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectText : MonoBehaviour
{
    FileSelecter Select;
    public TextMeshProUGUI TMPTITLE, TMPARTIST, TMPBPM, TMPDIFF, TMPDIFF2, TMPSPD, TMPMOD, TMPSORT, TMPCHART, TMPNOTE, TMPLN, TMPLENGTH, TMPFIXED, TMPSPC;
    
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
        TMPDIFF.text = TMPDIFF2.text = "Level : " + Select._diff;
        string str = "NONE";
        if (GlobalSettings.Random) str = "RANDOM";
        if (GlobalSettings.Mirror) str = "MIRROR";
        TMPMOD.text = str;
        TMPSORT.text = "NAME";
        if (GlobalSettings.sortselection == 1) TMPSORT.text = "ARTIST";
        if (GlobalSettings.sortselection == 2) TMPSORT.text = "LEVEL";
        TMPSPC.text = "NONE";
        if (GlobalSettings.AutoPlay) TMPSPC.text = "AUTOPLAY";

        TMPSPD.text = "Scroll Speed : " + Mathf.Round(GlobalSettings.scrollSpeed * 10f - 8f);

        TMPCHART.text = "Noter : " + Select._charter;
        TMPLENGTH.text = $"Length : {NowPlaying.LENGTH}";

        TMPNOTE.text = $"Note counts : {NowPlaying.NOTECOUNTS}";
        TMPLN.text = $"Longnote counts : {NowPlaying.LONGNOTECOUNTS}";
       

        TMPFIXED.text = "F3 / F4";
        if (!GlobalSettings.isFixedScroll) TMPFIXED.text = "F3 / F4        [FIXED]";
    }
}
