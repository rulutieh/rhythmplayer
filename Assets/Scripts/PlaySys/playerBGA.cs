using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;


public class playerBGA : MonoBehaviour
{
    bool played;
    public RawImage mScreen = null;
    public VideoPlayer mVideoPlayer = null;
    RectTransform rect;
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        NotePlayer.isVideoLoaded = false;
        if (!string.IsNullOrEmpty(NowPlaying.BGAFILE) && Manager.isPlayVideo)
        {
            mVideoPlayer.url = NowPlaying.BGAFILE;
            StartCoroutine(PrepareVideo());
        }
        else
        {
            NotePlayer.isVideoLoaded = true;
            Destroy(gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (NotePlayer.isPlaying && !mVideoPlayer.isPlaying && mVideoPlayer.isPrepared)
        {
            if (!played)
            {
                mVideoPlayer.Play();
                played = true;
            }
            else
                Destroy(gameObject);
        }
    }
    IEnumerator PrepareVideo()
    {
        // 비디오 준비
        mVideoPlayer.Prepare();

        // 비디오가 준비되는 것을 기다림
        while (!mVideoPlayer.isPrepared)
        {
            yield return new WaitForSeconds(0.3f);
        }

        NotePlayer.isVideoLoaded = true;
        mScreen.texture = mVideoPlayer.texture;
    }
}
