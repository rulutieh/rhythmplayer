using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class showScore : MonoBehaviour
{
    TextMeshProUGUI tmp;
    public TextMeshProUGUI acc;
    int score, oldscore;
    ScoreManager manager;
    // Start is called before the first frame update
    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        var g = GameObject.FindWithTag("NoteSys");
        manager = g.GetComponent<ScoreManager>();
    }

    // Update is called once per frame
    void Update()
    {
        float a = manager.score[manager.currentPlayer]._acc;
        acc.text = string.Format("{0:p}", a);
        score = manager.score[manager.currentPlayer].SCORE;
        tmp.text = score.ToString("0000000");
        if (score != oldscore)
        {
            //tmp.text = score.ToString("0000000");
            oldscore = score;
        }
        
        
    }
}
