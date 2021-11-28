using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pixelplacement;

public class ResultScreen : MonoBehaviour
{
    public Image gutter;
    public Sprite[] rankspr;
    public GameObject Rank;
    public GameObject[] elements;
    Vector2 gutterStartSize;
    Vector2 gutterEndSize;
    MusicHandler player;
    bool played;
    Image rend;
    ScoreManager manager;
    public TextMeshProUGUI tmptitle, tmpscore, tmpkool, tmpcool, tmpgood, tmpmiss, tmpbad, tmpcombo, tmpresult, tmplevel, tmpacc;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("world").GetComponent<MusicHandler>();
        rend = Rank.GetComponent<Image>();
        gutterStartSize = new Vector2(gutter.rectTransform.sizeDelta.x, 0);
        gutterEndSize = gutter.rectTransform.sizeDelta;
        gutter.rectTransform.sizeDelta = new Vector2(0, 0);

        var g = GameObject.FindWithTag("NoteSys");
        manager = g.GetComponent<ScoreManager>();

        Tween.Size(gutter.rectTransform, gutterStartSize, gutterEndSize, .5f, 0, Tween.EaseInOutStrong, Tween.LoopType.None, null, () => showChilds());
    }
    // Update is called once per frame
    void Update()
    {

        tmptitle.text = $"{NowPlaying.PLAY.ARTIST} - {NowPlaying.PLAY.TITLE}";
        tmplevel.text = NowPlaying.PLAY.LEVEL;
        
        tmpcombo.text = "Combo : " + manager.score[manager.currentPlayer].MAXCOMBO.ToString();
    }
    void showChilds()
    {
        for (int i = 0; i < elements.Length; i++)
            elements[i].SetActive(true);
        StartCoroutine(LoadScores());
    }
    IEnumerator LoadScores()
    {
        tmpkool.text = manager.score[manager.currentPlayer].KOOL.ToString("0000");
        yield return new WaitForSeconds(0.1f);
        tmpcool.text = manager.score[manager.currentPlayer].COOL.ToString("0000");
        yield return new WaitForSeconds(0.1f);
        tmpgood.text = manager.score[manager.currentPlayer].GOOD.ToString("0000");
        yield return new WaitForSeconds(0.1f);
        tmpbad.text = manager.score[manager.currentPlayer].BAD.ToString("0000");
        yield return new WaitForSeconds(0.1f);
        tmpmiss.text = manager.score[manager.currentPlayer].MISS.ToString("0000");
        yield return new WaitForSeconds(0.2f);
        tmpacc.text = string.Format("{0:p}", manager.score[manager.currentPlayer]._acc);
        yield return new WaitForSeconds(0.2f);
        tmpscore.text = Mathf.Round(manager.score[manager.currentPlayer].SCORE).ToString("0000000");
        yield return new WaitForSeconds(0.2f);
        if (!played)
        {
            if (ScoreManager.isFailed)
            {
                tmpresult.text = "Failed";
                player.PlaySFX(8);
            }
            else if (Manager.AutoPlay)
            {
                tmpresult.text = "AutoPlay";
                player.PlaySFX(7);
            }
            else
            {
                tmpresult.text = "Cleared";
                player.PlaySFX(7);
            }
            played = true;
        }
        yield return new WaitForSeconds(0.2f);
        if (manager.score[manager.currentPlayer].MAXCOMBO == NotePlayer.NoteCountLongnote) tmpcombo.text = "FULL COMBO";
        if (Mathf.Round(manager.score[manager.currentPlayer].SCORE) == 1000000) tmpcombo.text = "PERFECT";
        yield return new WaitForSeconds(0.3f);
        Rank.SetActive(true);
        if (manager.score[manager.currentPlayer]._acc >= 0.95f)
        {
            if (manager.score[manager.currentPlayer].MISS == 0 && manager.score[manager.currentPlayer].BAD == 0)
            {
                rend.sprite = rankspr[0];
            }
            else
            {
                rend.sprite = rankspr[1];
            }
        }
        else if (manager.score[manager.currentPlayer]._acc >= 0.9f)
        {
            rend.sprite = rankspr[2];
        }
        else if (manager.score[manager.currentPlayer]._acc >= 0.8f)
        {
            rend.sprite = rankspr[3];
        }
        else if (manager.score[manager.currentPlayer]._acc >= 0.6f)
        {
            rend.sprite = rankspr[4];
        }
        else
        {
            rend.sprite = rankspr[5];
        }

    }
}
