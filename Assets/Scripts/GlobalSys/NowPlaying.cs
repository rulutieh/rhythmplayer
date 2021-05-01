using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NowPlaying : MonoBehaviour
{

    public static string MUSICFILE, FILE, BGFILE, BGAFILE, TITLE, ARTIST, HASH;
    public static string LEVEL, LENGTH;
    public static float OFFSET;
    public static double MEDIAN;
    public static int TIMINGCOUNTS, NOTECOUNTS, LONGNOTECOUNTS;
    public static AudioClip AUD;
    public static bool isBGA;
    public int nt, ln;
    float f;
    MusicHandler player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("world").GetComponent<MusicHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        nt = NOTECOUNTS;
        ln = LONGNOTECOUNTS;
        string s;
        f = player.GetLength() / 1000f;
        s = $" {Mathf.FloorToInt(f / 60f)} : ";
        s += (f % 60f).ToString("00");
        LENGTH = s;
    }
}
