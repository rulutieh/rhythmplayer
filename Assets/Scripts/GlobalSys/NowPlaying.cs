using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NowPlaying : MonoBehaviour
{

    public string MUSICFILE, FILE, BGFILE, BGAFILE, TITLE, ARTIST, HASH, FOLDER;
    public string LEVEL, LENGTH;
    public float OFFSET;
    public  double MEDIAN;
    public int TIMINGCOUNTS, NOTECOUNTS, LONGNOTECOUNTS;
    public bool isBGA, isVirtual;
    public int nt, ln;
    public int LengthMS;
    MusicHandler player;
    public Sprite bg;
    private static NowPlaying play = null;

    private void Awake()
    {
        play = this;
    }
    public static NowPlaying PLAY
    {
        get
        {
            if (null == play)
            {
                return null;
            }
            return play;
        }
    }
    // Start is called before the first frame update

    void Start()
    {
        player = GameObject.FindWithTag("world").GetComponent<MusicHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (nt != NOTECOUNTS || ln != LONGNOTECOUNTS)
        {
            nt = NOTECOUNTS;
            ln = LONGNOTECOUNTS;
            string s;
            float f = LengthMS / 1000f;
            s = $" {Mathf.FloorToInt(f / 60f)} : ";
            s += (f % 60f).ToString("00");
            LENGTH = s;
        }
    }
}
