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
        rend = gameObject.GetComponent<Image>();
        rend.sprite = NowPlaying.PLAY.bg;
    }
}
