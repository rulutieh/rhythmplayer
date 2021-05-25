﻿using System.Collections;
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
    public AudioClip[] sfxs;
    Vector2 gutterStartSize;
    Vector2 gutterEndSize;
    MusicHandler player;
    bool played;
    Image rend;
    public TextMeshProUGUI tmptitle, tmpscore, tmpcool, tmpgreat, tmpgood, tmpmiss, tmpbad, tmpcombo, tmpresult, tmplevel;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("world").GetComponent<MusicHandler>();
        rend = Rank.GetComponent<Image>();
        gutterStartSize = new Vector2(gutter.rectTransform.sizeDelta.x, 0);
        gutterEndSize = gutter.rectTransform.sizeDelta;
        gutter.rectTransform.sizeDelta = new Vector2(0, 0);



        Tween.Size(gutter.rectTransform, gutterStartSize, gutterEndSize, .5f, 0, Tween.EaseInOutStrong, Tween.LoopType.None, null, () => showChilds());
    }
    // Update is called once per frame
    void Update()
    {

        tmptitle.text = $"{NowPlaying.ARTIST} - {NowPlaying.TITLE}";
        tmplevel.text = NowPlaying.LEVEL;
        
        tmpcombo.text = "Combo : " + ScoreManager.maxcombo.ToString();
    }
    void showChilds()
    {
        for (int i = 0; i < elements.Length; i++)
            elements[i].SetActive(true);
        StartCoroutine(LoadScores());
    }
    IEnumerator LoadScores()
    {
        yield return new WaitForSeconds(0.1f);
        tmpcool.text = ScoreManager.COOL.ToString("0000");
        yield return new WaitForSeconds(0.1f);
        tmpgreat.text = ScoreManager.GREAT.ToString("0000");
        yield return new WaitForSeconds(0.1f);
        tmpgood.text = ScoreManager.GOOD.ToString("0000");
        yield return new WaitForSeconds(0.1f);
        tmpbad.text = ScoreManager.BAD.ToString("0000");
        yield return new WaitForSeconds(0.1f);
        tmpmiss.text = ScoreManager.MISS.ToString("0000");
        yield return new WaitForSeconds(0.2f);
        tmpscore.text = Mathf.Round(ScoreManager.Score).ToString("0000000");
        yield return new WaitForSeconds(0.2f);
        if (!played)
        {
            if (ScoreManager.isFailed)
            {
                tmpresult.text = "Failed";
                player.PlaySFX(8);
            }
            else if (GlobalSettings.AutoPlay)
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
        if (ScoreManager.maxcombo == FileReader.NoteCountLongnote) tmpcombo.text = "FULL COMBO";
        if (Mathf.Round(ScoreManager.Score) == 1000000) tmpcombo.text = "PERFECT";
        yield return new WaitForSeconds(0.3f);
        Rank.SetActive(true);
        if (ScoreManager.Score > 950000f)
        {
            if (ScoreManager.MISS == 0 && ScoreManager.BAD == 0)
            {
                rend.sprite = rankspr[0];
            }
            else
            {
                rend.sprite = rankspr[1];
            }
        }
        else if (ScoreManager.Score > 900000f)
        {
            rend.sprite = rankspr[2];
        }
        else if (ScoreManager.Score > 800000f)
        {
            rend.sprite = rankspr[3];
        }
        else if (ScoreManager.Score > 600000f)
        {
            rend.sprite = rankspr[4];
        }
        else
        {
            rend.sprite = rankspr[5];
        }

    }
}