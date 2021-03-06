using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JudgeText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI error, acc, auto;
    bool active;
    float ap = 0;
    string txt = "";
    ScoreManager manager;

    private void Start()
    {
        var g = GameObject.FindWithTag("NoteSys");
        manager = g.GetComponent<ScoreManager>();
    }

    void Update()
    {
        float j = Mathf.RoundToInt(ScoreManager.judgeerror);
        if (NotePlayer.Playback > 100) active = true;
        if (active)
        {
            if (ap < 0.7f)
                ap += Time.deltaTime / 2f;
            if (j < 0)
            {
                error.color = Color.red;
                txt = "";
            }
            else
            if (j > 0)
            {
                error.color = Color.blue;
                txt = "+";
            }
            else
            {
                error.color = Color.white;
                txt = "";
            }
            Color col = error.color;
            error.color = new Color(col.r, col.g, col.b, ap);
            acc.color = new Color(1, 1, 1, ap);
            auto.color = acc.color;
            float a = manager.score[manager.currentPlayer]._acc;
            if (!Manager.AutoPlay)
            {
                error.text = $"{txt}{j}";
                acc.text = string.Format("{0:p}", a);
            }
            else
            {
                auto.text = "AutoPlay";
            }
        }

    }
}
