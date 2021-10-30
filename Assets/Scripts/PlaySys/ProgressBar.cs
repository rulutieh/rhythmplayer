using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    RectTransform rect;
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        float c = 437f * NotePlayer.Playback / NowPlaying.PLAY.LengthMS;
        if (c > 437f) c = 437f;
        rect.anchoredPosition = new Vector2(-437f + c, rect.anchoredPosition.y);
    }
}
