using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrTitleMusic : MonoBehaviour
{
    AudioSource aud;
    AudioClip audClip;
    FileLoader files;
    bool loading;
    // Start is called before the first frame update
    void Start()
    {
        aud = GetComponent<AudioSource>();
        files = GameObject.FindWithTag("FileSys").GetComponent<FileLoader>();
    }

    // Update is called once per frame
    void Update()
    {
        aud.volume = scrSetting.Volume;
        if (!aud.isPlaying && !loading)
        {
            StartCoroutine(LoadMusic(files.listorigin[Mathf.FloorToInt(Random.Range(0, files.listorigin.Count - 1))].getAudio()));
        }
    }
    IEnumerator LoadMusic(string filepath)
    {
        loading = true;
        WWW www = new WWW(filepath);
        yield return www; //로드
        audClip = www.GetAudioClip();
        aud.clip = audClip;
        NowPlaying.AUD = aud.clip;
        aud.Play();
        loading = false;
    }
}
