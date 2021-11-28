using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Serializable]
    public class SCORES
    {
        public int KOOL { get; set; }
        public int COOL { get; set; }
        public int GOOD { get; set; }
        public int BAD { get; set; }
        public int MISS { get; set; }
        public int TOTAL { get; set; }
        public int COMBO { get; set; }
        public int MAXCOMBO { get; set; }
        public int SCORE { get; set; }
        public int MAXNOTE { get; set; }
        public void CacScore()
        {
            Debug.Log(MISS + " " + TOTAL);
            if (MAXNOTE == 0) return;

            float s = (GOOD / (2f * MAXNOTE)) + (BAD / (6f * MAXNOTE));
            float sum = (KOOL / (float)MAXNOTE) + (COOL * 19f / (MAXNOTE * 20f)) + s;
            float sumforacc = ((KOOL + COOL) / (float)MAXNOTE) + s;
            SCORE = Mathf.RoundToInt(1000000f * sum);
            if (TOTAL != 0)
                _acc = sumforacc / (TOTAL / (float)MAXNOTE);
            if (COMBO > MAXCOMBO) MAXCOMBO = COMBO; //최대 콤보 체크
        }
        public float _acc { get; set; }


    }


    [SerializeField]
    GameObject gameover, judgeobj;
    RankSystem RankSys;
    public static float HP = 1f, judgeerror;
    public static bool isFailed;

    public SCORES[] score;

    public int currentPlayer; //멀티플레이구분

    List<float> errorList = new List<float>();
    // Start is called before the first frame update
    void Start()
    {
        score = new SCORES[1];
        score[0] = new SCORES();
        score[0].MAXNOTE = NowPlaying.PLAY.NOTECOUNTS + (NowPlaying.PLAY.LONGNOTECOUNTS * 2);

        GameObject w = GameObject.FindWithTag("world");
        RankSys = w.GetComponent<RankSystem>();
        isFailed = false;
        HP = 1f;
        judgeerror = 0;
    }
    

    // Update is called once per frame
    void Update()
    {
        if (HP <= 0) //게임오버
        {
            isFailed = true;
            gameover.SetActive(true);

        }
        if (errorList.Count > 30) errorList.RemoveAt(0);
    }
    //판정관련
    public void CacScore()
    {
        if (HP <= 0) //게임오버
        {
            isFailed = true;
            gameover.SetActive(true);
            
        }
        
        score[currentPlayer].CacScore();
        //패슬 평균
        if (errorList.Count > 30) errorList.RemoveAt(0);
    }
    public void SetJudge(int a)
    {
        score[currentPlayer].COMBO = 0;
        judgeobj.SetActive(true);
        if (a == 1) //롱노트 아애 안눌럿을시
            judgeobj.GetComponent<Judgement>().setInfo(5);
        else if (a == 2)
            judgeobj.GetComponent<Judgement>().setInfo(7);
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
        RankSys.SaveScore(
            NowPlaying.PLAY.HASH,
            Manager.playername,
            score[currentPlayer].KOOL,
            score[currentPlayer].COOL,
            score[currentPlayer].GOOD,
            score[currentPlayer].BAD,
            score[currentPlayer].MISS,
            score[currentPlayer].MAXCOMBO,
            DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")
            );
    }
}
