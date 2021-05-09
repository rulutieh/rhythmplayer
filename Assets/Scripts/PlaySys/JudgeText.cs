using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JudgeText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI error, acc;
    bool active;
    float ap = 0;
    string txt = "";
    void Update()
    {
        float j = Mathf.RoundToInt(ScoreManager.judgeerror);
        if (FileReader.Playback > 100) active = true;
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
            float a = ScoreManager.acc;
            a = Mathf.Floor(a * 100) / 100f;
            error.text = $"{txt}{j}";
            acc.text = $"{a}%";
        }

    }
}
