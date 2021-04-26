using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pixelplacement;

public class scrResult : MonoBehaviour
{
    public Image gutter;
    public Sprite[] rankspr;
    public GameObject Rank;
    public GameObject[] elements;
    public AudioClip[] sfxs;
    AudioSource aud;
    Transform[] allChildren;
    Vector2 gutterStartSize;
    Vector2 gutterEndSize;
    bool played;
    Image rend;
    public TextMeshProUGUI tmptitle, tmpscore, tmpcool, tmpgreat, tmpgood, tmpmiss, tmpbad, tmpcombo, tmpresult, tmplevel;
    // Start is called before the first frame update
    void Start()
    {
        aud = GetComponent<AudioSource>();
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
        
        tmpcombo.text = "Combo : " + FileReader.maxcombo.ToString();
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
        tmpcool.text = FileReader.COOL.ToString("0000");
        yield return new WaitForSeconds(0.1f);
        tmpgreat.text = FileReader.GREAT.ToString("0000");
        yield return new WaitForSeconds(0.1f);
        tmpgood.text = FileReader.GOOD.ToString("0000");
        yield return new WaitForSeconds(0.1f);
        tmpbad.text = FileReader.BAD.ToString("0000");
        yield return new WaitForSeconds(0.1f);
        tmpmiss.text = FileReader.MISS.ToString("0000");
        yield return new WaitForSeconds(0.2f);
        tmpscore.text = Mathf.Round(FileReader.Score).ToString("0000000");
        yield return new WaitForSeconds(0.2f);
        if (!played)
        {
            if (FileReader.isFailed)
            {
                tmpresult.text = "Failed";
                aud.clip = sfxs[0]; aud.Play();
            }
            else
            {
                tmpresult.text = "Cleared";
                aud.clip = sfxs[1]; aud.Play();
            }
            played = true;
        }
        yield return new WaitForSeconds(0.2f);
        if (FileReader.maxcombo == FileReader.NoteCountLongnote) tmpcombo.text = "FULL COMBO";
        if (Mathf.Round(FileReader.Score) == 1000000) tmpcombo.text = "PERFECT";
        yield return new WaitForSeconds(0.3f);
        Rank.SetActive(true);
        if (FileReader.Score > 950000f)
        {
            if (FileReader.MISS == 0 && FileReader.BAD == 0)
            {
                rend.sprite = rankspr[0];
            }
            else
            {
                rend.sprite = rankspr[1];
            }
        }
        else if (FileReader.Score > 900000f)
        {
            rend.sprite = rankspr[2];
        }
        else if (FileReader.Score > 800000f)
        {
            rend.sprite = rankspr[3];
        }
        else if (FileReader.Score > 600000f)
        {
            rend.sprite = rankspr[4];
        }
        else
        {
            rend.sprite = rankspr[5];
        }

    }
}
