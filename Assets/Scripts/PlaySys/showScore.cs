using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class showScore : MonoBehaviour
{
    TextMeshProUGUI tmp;
    public TextMeshProUGUI acc;
    int score, oldscore;
    // Start is called before the first frame update
    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        float a = ScoreManager.acc;
        acc.text = string.Format("{0:p}", a);
        score = ScoreManager.Score;
        tmp.text = score.ToString("0000000");
        if (score != oldscore)
        {
            //tmp.text = score.ToString("0000000");
            oldscore = score;
        }
        
        
    }
}
