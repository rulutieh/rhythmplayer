using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class RankPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isOn = true;
    public float ypos, ypostemp;
    int counts;
    public GameObject o, isnull;
    RectTransform rect;
    RankSystem sys;
    Vector2 startpos;
    private void Awake()
    {
        isnull.SetActive(true);
        sys = GameObject.FindWithTag("world").GetComponent<RankSystem>();
        rect = GetComponent<RectTransform>();
        startpos = rect.transform.position;
        rect.DOAnchorPosX(67.93f, 0.4f, true);
    }

    public void onoff()
    {
        isOn = !isOn;
        if (isOn)
        {
            rect.DOAnchorPosX(67.93f, 0.4f, true);
        }
        else
            rect.DOAnchorPosX(-92.5f, 0.4f, true);
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (isOver)
        {
            if (scroll > 0) { ypostemp-= 20f; }
            if (scroll < 0) { ypostemp+= 20f; }
        }
        if (ypostemp < 0) ypostemp = 0;
        if (ypostemp > counts * 31.5f) ypostemp = counts * 31.5f;

        ypos = Mathf.Lerp(ypos, ypostemp, 0.2f);

        if (!isOn) isOver = false;
    }

    public bool isOver = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOver = false;
    }

    public void LoadRanks(string hash)
    {
        ypos = 0;
        var gameObjects = GameObject.FindGameObjectsWithTag("ranks");
        for (int i = 0; i < gameObjects.Length; i++)
        {
            Destroy(gameObjects[i]);
        }
        isnull.SetActive(true);
        sys.SelectSong(hash);
        counts = sys.GetScoreCounts();
        for (int i = 0; i < counts; i++)
        {
            isnull.SetActive(false);
            var r = Instantiate(o, transform.GetChild(0));
            string pname, date;
            int score, state, maxcombo;
            sys.GetInfo(i, out pname, out score, out float acc, out state, out maxcombo, out date);
            r.GetComponent<RankText>().SetText(
                i,
                pname,
                score,
                acc,
                state,
                maxcombo,
                date,
                this.gameObject
                );
        }

    }
}
