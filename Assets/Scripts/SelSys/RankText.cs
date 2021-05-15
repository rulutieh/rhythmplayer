using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RankText : MonoBehaviour
{
    public Sprite[] rankspr;
    public GameObject rk;
    RectTransform rect;
    GameObject panel;
    RankPanel rp;
    Image rend;
    int idx;
    public TextMeshProUGUI playertxt;
    public TextMeshProUGUI scoretxt;
    public TextMeshProUGUI combotxt;
    public TextMeshProUGUI datetxt;
    public TextMeshProUGUI sunwitxt;
    // Start is called before the first frame update
    public void Awake()
    {
        rect = GetComponent<RectTransform>();
        rend = rk.GetComponent<Image>();
    }
    private void Update()
    {
        rect.anchoredPosition = new Vector2(-0.1f, 60.46f - idx * 31f + rp.ypos);
    }
    public void SetText(int id, string pname, int score, int state, int maxcombo, string date, GameObject p)
    {
        idx = id;
        playertxt.text = pname;
        scoretxt.text = score.ToString("0000000");
        combotxt.text = $"max combo : {maxcombo}";
        datetxt.text = date;
        sunwitxt.text = $"#{id + 1}";
        panel = p;
        rp = p.GetComponent<RankPanel>();

        if (score > 950000f)
        {
            if (state == 1)
            {
                rend.sprite = rankspr[0];
            }
            else
            {
                rend.sprite = rankspr[1];
            }
        }
        else if (score > 900000f)
        {
            rend.sprite = rankspr[2];
        }
        else if (score > 800000f)
        {
            rend.sprite = rankspr[3];
        }
        else if (score > 600000f)
        {
            rend.sprite = rankspr[4];
        }
        else
        {
            rend.sprite = rankspr[5];
        }
    }
    //233.3, 46.2
}
