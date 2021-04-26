using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scrAlbumArt : MonoBehaviour
{
    Image rend;
    public Image bg;
    Color c;
    bool isloaded;
    // Start is called before the first frame update
    void Start()
    {
        rend = this.GetComponent<Image>();
        c = new Color(1, 1, 1, 1);
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
    public void LoadAlbumArt()
    {
        StartCoroutine(LoadImage());
        isloaded = false;
    }
    IEnumerator LoadImage()
    {
        
        WWW www1 = new WWW(NowPlaying.BGFILE);
        yield return www1;
        isloaded = true;
        c.a = 0;
        float width = www1.texture.width;
        float height = www1.texture.height;
        rend.sprite = Sprite.Create(www1.texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
        bg.sprite = rend.sprite;
        rend.color = new Color(1, 1, 1, 0);
    }
}
