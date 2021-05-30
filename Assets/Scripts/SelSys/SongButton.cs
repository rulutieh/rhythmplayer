using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SongButton : MonoBehaviour, IPointerClickHandler
{
    FileSelecter select;
    TextMeshProUGUI tmp, tmpa, tmpl;
    RectTransform rect;

    Image rend;
    public Transform Text, TextA, TextL;
    int decide;
    bool crunning;
    public int idx;
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
    void OnEnable()
    {
        rect.anchoredPosition = new Vector2(1000f, 0);
        transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
    }
    // Update is called once per frame
    void Update()
    {
        decide = GlobalSettings.decide;
        float gap = decide - idx;
        // + Mathf.Abs(gap * 10)
        Vector2 pos = new Vector2(-100f, gap * 46f);
        rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, pos, Time.deltaTime * 30f);

        if (Mathf.Abs(gap) < 1f)
        {
            Color c = new Color(1f, 1f, 1f, 0.9f);
            rend.color = c;
            rect.SetSiblingIndex(10000);
        }
        else
        {
            Color c = new Color(0, 0, 0, 0.7f);
            rend.color = c;
            rect.SetSiblingIndex(idx);
        }


    }
    public void Pooling()
    {
        if (Mathf.Abs(GlobalSettings.decide - idx) > 10)
        {
            select.b_queue.Enqueue(gameObject);
            gameObject.SetActive(false);
        }
    }
    public void setInfo(int idx, string title, string artist, string level)
    {
        tmpa.alpha = 1f;
        tmpa.text = artist;
        tmp.text = title;
        tmpl.text = level;
        this.idx = idx;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GlobalSettings.decide == idx)
        {
            select.songDecide();
        }
        else
        {
            if (!crunning)
                StartCoroutine(Selector());
        }

    }

    IEnumerator Selector()
    {
        crunning = true;
        if (GlobalSettings.decide > idx)
        {
            while (GlobalSettings.decide > idx)
            {
                select.SongScroll();
                GlobalSettings.decide--;
                yield return new WaitForSeconds(0.05f);
            }
        }
        else
        {
            while (GlobalSettings.decide < idx)
            {
                select.SongScroll();
                GlobalSettings.decide++;
                yield return new WaitForSeconds(0.05f);
            }
        }
        crunning = false;
    }
}
