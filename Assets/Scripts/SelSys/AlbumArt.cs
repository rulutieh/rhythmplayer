using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class AlbumArt : MonoBehaviour, IPointerClickHandler
{
    public GameObject panel;
    RectTransform rect;
    RankPanel p;
    Image rend;
    GameObject bg;
    Vector3 startpos, endpos;
    Color c;
    bool isloaded;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Image>();
        c = new Color(1, 1, 1, 1);
        p = panel.GetComponent<RankPanel>();
        rect = GetComponent<RectTransform>();
        bg = GameObject.FindWithTag("Background");
    }

    // Update is called once per frame
    void Update()
    {
        if (c.a < 1f && isloaded)
        {
            c.a += Time.deltaTime * 3f;
            rend.color = c;
        }


    }

    public void OnPointerClick(PointerEventData eventData)
    {
        p.onoff();
    }


    public void LoadAlbumArt()
    {
        StartCoroutine(LoadImage());
        isloaded = false;
    }
    IEnumerator LoadImage()
    {
        
        WWW www1 = new WWW(NowPlaying.PLAY.BGFILE);
        yield return www1;
        isloaded = true;
        c.a = 0;
        float width = www1.texture.width;
        float height = www1.texture.height;
        rend.sprite = Sprite.Create(www1.texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
        bg.GetComponent<Image>().sprite = rend.sprite;
        NowPlaying.PLAY.bg = rend.sprite;
        rend.color = new Color(1, 1, 1, 0);
    }
}
