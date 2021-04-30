using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingPanel : MonoBehaviour
{

    bool init;
    public Slider slvolume, sltrans;
    public Toggle video, bpm, fullscreen;
    public Dropdown ddres, ddframe;
    public TextMeshProUGUI offset;
    MusicHandler player;
    scrSetting st;
    GameObject w;
    AudioSource aud;
    public AudioClip[] clips;

    public void Awake()
    {
        w = GameObject.FindWithTag("world");
        player = w.GetComponent<MusicHandler>();
        st = w.GetComponent<scrSetting>();
        aud = w.GetComponent<AudioSource>();
        bpm.isOn = scrSetting.isFixedScroll;
        video.isOn = scrSetting.isPlayVideo;
        fullscreen.isOn = scrSetting.isFullScreen;
        bpm.onValueChanged.AddListener(
            (bool bOn) =>
            {
                bool val = bOn;
                scrSetting.isFixedScroll = val;
                st.SaveSettings();
            }
        );
        video.onValueChanged.AddListener(
            (bool bOn) =>
            {
                bool val = bOn;
                scrSetting.isPlayVideo = val;
                st.SaveSettings();
            }
        );
        fullscreen.onValueChanged.AddListener(
            (bool bOn) =>
            {
                bool val = bOn;
                scrSetting.isFixedScroll = val;
                Screen.fullScreen = val;
                st.SaveSettings();
            }
        );
        slvolume.onValueChanged.AddListener(
            (float vl) =>
            {
                scrSetting.Volume = vl;
                st.SaveSettings();
            }
        );
    }
    private void Start()
    {
        if (!init && ddres)
        {
            ddres.value = st.res;
            ddframe.value = st.fps;
            slvolume.value = scrSetting.Volume;
            init = true;
        }
    }
    private void Update()
    {
        float f = Mathf.Round(scrSetting.GlobalOffset * 100f) / 100f;
        offset.text = f.ToString();
    }
    public void SetResolution(int val)
    {
        if (init)
        {
            st.res = val;
            st.SaveSettings();
            st.SwitchResolution();
        }
    }
    public void SetFrameRate(int val)
    {
        if (init)
        {
            st.fps = val;
            st.SaveSettings();
            st.SwitchFrameRate();
        }
    }
    public void ClickSound(int index)
    {
        player.PlaySFX(index);
    }
    public void SetGlobalOffset(float o)
    {
        scrSetting.GlobalOffset += o;
        st.SaveSettings();
        ClickSound(1);
    }

}
