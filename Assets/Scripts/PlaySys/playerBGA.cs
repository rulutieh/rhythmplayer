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
        FileReader.isVideoLoaded = false;
        if (NowPlaying.isBGA && GlobalSettings.isPlayVideo)
        {
            mVideoPlayer.url = NowPlaying.BGAFILE;
            StartCoroutine(PrepareVideo());
        }
        else
        {
            FileReader.isVideoLoaded = true;
            Destroy(gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (FileReader.isPlaying && !mVideoPlayer.isPlaying && mVideoPlayer.isPrepared)
        {
            if (!played)
            {
                mVideoPlayer.Play();
                /*
                float tmp =  700f / mVideoPlayer.width;
                rect.sizeDelta = new Vector2(1280 * tmp, 720 * tmp);
                played = true;
                */
            }
            else
                Destroy(gameObject);
        }
    }
    IEnumerator PrepareVideo()
    {
        // ���� �غ�
        mVideoPlayer.Prepare();

        // ������ �غ�Ǵ� ���� ��ٸ�
        while (!mVideoPlayer.isPrepared)
        {
            yield return new WaitForSeconds(0.3f);
        }

        FileReader.isVideoLoaded = true;
        mScreen.texture = mVideoPlayer.texture;
    }
}
