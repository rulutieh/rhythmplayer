using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    GameObject gameover, judgeobj;
    RankSystem RankSys;
    public static int KOOL, COOL, GOOD, MISS, BAD, TOTAL, combo, maxcombo, Score;
    public static float acc = 100f, HP = 1f, judgeerror;
    public static bool isFailed;

    List<float> errorList = new List<float>();
    // Start is called before the first frame update
    void Start()
    {
        GameObject w = GameObject.FindWithTag("world");
        RankSys = w.GetComponent<RankSystem>();
        isFailed = false;
        HP = 1f;
        COOL = KOOL = GOOD = MISS = BAD = TOTAL = 0;
        acc = 1f;
        combo = maxcombo = 0;
        judgeerror = 0;
    }
    

    // Update is called once per frame
    void Update()
    {

    }
    //판정관련
    public void CacScore()
    {
        float c = NowPlaying.NOTECOUNTS + (NowPlaying.LONGNOTECOUNTS * 2);
        float s = (GOOD / (2 * c)) + (BAD / (6 * c));
        float sum = (KOOL / c) + (COOL * 19f / (c * 20f)) + s;
        float sumforacc = ((KOOL + COOL) / c) + s;
        Score = Mathf.RoundToInt(1000000f * sum);
        if (TOTAL != 0)
            acc = sumforacc / (TOTAL / c);


        if (combo > maxcombo) maxcombo = combo; //최대 콤보 체크

        if (HP <= 0) //게임오버
        {
            isFailed = true;
            gameover.SetActive(true);
        }

        //패슬 평균
        if (errorList.Count > 30) errorList.RemoveAt(0);

    }
    public void SetJudge(int a)
    {
        combo = 0;
        judgeobj.SetActive(true);
        if (a == 1) //롱노트 아애 안눌럿을시
            judgeobj.GetComponent<Judgement>().setInfo(5);
        else
            judgeobj.GetComponent<Judgement>().setInfo(4);
    }
    //패슬입력
    public void AddError(float error)
    {
        float sum = 0f;
        errorList.Add(error);
        for (int i = 0; i < errorList.Count; i++)
        {
            sum += errorList[i];
            judgeerror = sum / errorList.Count;
        }
    }
    public void SaveScores()
    {
        //COOL = KOOL = GOOD = MISS = BAD = TOTAL = 0;
        RankSys.SaveScore(
            NowPlaying.HASH,
            GlobalSettings.playername,
            KOOL,
            COOL,
            GOOD,
            BAD,
            MISS,
            maxcombo,
            DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")
            );
    }
}
