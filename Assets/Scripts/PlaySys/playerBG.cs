using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerBG : MonoBehaviour
{
    FileSelecter select;
    Image rend;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadImage());
    }

    IEnumerator LoadImage()
    {
        WWW www1 = new WWW(NowPlaying.BGFILE);
        yield return www1;
        float width = www1.texture.width;
        float height = www1.texture.height;
        rend = gameObject.GetComponent<Image>();
        rend.sprite = Sprite.Create(www1.texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
        //select.Album = rend.sprite;
    }
}
