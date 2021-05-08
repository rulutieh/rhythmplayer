using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class scrButton : MonoBehaviour, IPointerClickHandler
{
    FileSelecter select;
    TextMeshProUGUI tmp, tmpa, tmpl;
    RectTransform rect;
    Image rend;
    public Transform Text, TextA, TextL;
    int decide, idx;
    // Start is called before the first frame update
    void Awake()
    {
        tmpa = TextA.gameObject.GetComponent<TextMeshProUGUI>();
        tmpl = TextL.gameObject.GetComponent<TextMeshProUGUI>();
        tmp = Text.gameObject.GetComponent<TextMeshProUGUI>();
        rend = gameObject.GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        GameObject sel = GameObject.FindWithTag("SelSys");
        if (!sel) Destroy(gameObject);
        select = sel.GetComponent<FileSelecter>();
    }

    // Update is called once per frame
    void Update()
    {
        decide = scrSetting.decide;
        float gap = decide - idx;
        
        Vector2 pos = new Vector2(-130f + Mathf.Abs(gap * 10), gap * 50f);
        rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, pos, Time.deltaTime * 30f);

        if (Mathf.Abs(gap) < 1f)
        {
            Color c = new Color(1f, 1f, 1f, 0.9f);
            rend.color = c;
        }
        else
        {
            Color c = new Color(0, 0, 0, 0.7f);
            rend.color = c;
        }

    }
    public void setInfo(int idx, string title, string artist, string level)
    {
        tmpa.alpha = 1f;
        tmpa.text = artist;
        tmp.text = title;
        tmpl.text = "Level : " +level;
        this.idx = idx;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (scrSetting.decide == idx)
        {
            select.songDecide();
        }
        else
        {
            scrSetting.decide = idx;
            select.SongScroll();
        }

    }
    public void selection()
    {
        if (scrSetting.decide == idx)
        {
            select.songDecide();
        }
        else
        {
            scrSetting.decide = idx;
            select.SongScroll();
        }
        
    }
}
