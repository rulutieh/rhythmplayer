using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class RankPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isOn = true, online = false;
    public float ypos, ypostemp;
    int counts;
    string h;
    public GameObject o, isnull, onlinemyrank;
    [SerializeField]
    TextMeshProUGUI tmp;
    RectTransform rect;
    RankSystem sys;
    Vector2 startpos;
    RankText myRank;
    private void Awake()
    {
        isnull.SetActive(true);
        sys = GameObject.FindWithTag("world").GetComponent<RankSystem>();
        rect = GetComponent<RectTransform>();
        startpos = rect.anchoredPosition;
        rect.DOAnchorPosX(67.93f, 0.4f, true);
        myRank = onlinemyrank.GetComponent<RankText>();
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
        h = hash;
        ypos = 0;
        DestroySubjects();
        if (!online)
            isnull.SetActive(true);

        sys.SelectSong(hash);        
        StartCoroutine(LoadSubjects());

    }
    void DestroySubjects()
    {
        var gameObjects = GameObject.FindGameObjectsWithTag("ranks");
        for (int i = 0; i < gameObjects.Length; i++)
        {
            Destroy(gameObjects[i]);
        }
    }
    IEnumerator LoadSubjects()
    {
        onlinemyrank.SetActive(false);
        while (online && sys.AsyncLoading)
            yield return null;

        counts = sys.GetScoreCounts(online);
        if (counts == 0) isnull.SetActive(true);
        string pname, date;
        int score, state, maxcombo;
        for (int i = 0; i < counts; i++)
        {
            isnull.SetActive(false);
            var r = Instantiate(o, transform.GetChild(0));
            r.transform.position = new Vector2(1000,1000);
            sys.GetInfo(online, i, out pname, out score, out float acc, out state, out maxcombo, out date);
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
        while (sys.AsyncLoading)
            yield return null;
        sys.GetMyScore(out pname, out score, out float acc2, out state, out maxcombo, out date, out int orank);
        if (!string.IsNullOrEmpty(pname))
        {
            onlinemyrank.SetActive(true);
            myRank.SetText(
                    -1,
                    pname,
                    score,
                    acc2,
                    state,
                    maxcombo,
                    date,
                    this.gameObject
                    );
            myRank.onlinerank = orank;
        }
    }
    public void SwitchOnline()
    {
        StopCoroutine(LoadSubjects());
        online = !online;
        if (online) tmp.text = "Online Ranking"; else tmp.text = "Local Ranking";
        DestroySubjects();
        StartCoroutine(LoadSubjects());
        //LoadRanks(h);
    }
}
