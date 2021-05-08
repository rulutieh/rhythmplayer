using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class RankPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isOn = true;
    public float ypos;
    int counts;
    public GameObject o, isnull;
    RectTransform rect;
    RankSystem sys;
    private void Awake()
    {
        isnull.SetActive(true);
        sys = GameObject.FindWithTag("world").GetComponent<RankSystem>();
        rect = GetComponent<RectTransform>();
        rect.DOAnchorPosX(67.93f, 0.5f, true);
    }

    public void onoff()
    {
        isOn = !isOn;
        if (isOn)
        {
            rect.DOAnchorPosX(67.93f, 0.5f, true);
        }
        else
            rect.DOAnchorPosX(-92.5f, 0.5f, true);
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (isOver)
        {
            if (scroll > 0) { ypos-= 4f; }
            if (scroll < 0) { ypos+= 4f; }
        }
        if (ypos < 0) ypos = 0;
        if (ypos > counts * 31.5f) ypos = counts * 31.5f;
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
            r.GetComponent<RankText>().SetText(
                i,
                sys.GetPlayerName(i),
                sys.GetScores(i),
                sys.GetState(i),
                sys.GetCombo(i),
                sys.GetDate(i),
                this.gameObject
                );
        }

    }
}
