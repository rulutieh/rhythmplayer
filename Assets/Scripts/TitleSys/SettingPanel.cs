using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingPanel : MonoBehaviour
{

    bool init;
    public Slider slvolume, slcolumn;
    public Toggle video, bpm, fullscreen;
    public Dropdown ddres, ddframe;
    public TextMeshProUGUI offset;
    //login
    public InputField ID;
    public InputField PW;

    public GameObject Loginpanel, Logoutpanel;

    public TextMeshProUGUI loginas;

    MusicHandler player;
    scrSetting st;
    GameObject w;

    public void Awake()
    {    
        w = GameObject.FindWithTag("world");
        player = w.GetComponent<MusicHandler>();
        st = w.GetComponent<scrSetting>();
        slcolumn.value = scrSetting.ColWidth;
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
        slcolumn.onValueChanged.AddListener(
            (float cw) =>
            {
                scrSetting.ColWidth = cw;
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

        if (scrSetting.playername == "Guest")
        {
            Loginpanel.SetActive(true);
            Logoutpanel.SetActive(false);
        }
        else
        {
            Loginpanel.SetActive(false);
            Logoutpanel.SetActive(true);
        }
        loginas.text = "Login as : " + scrSetting.playername;
    }
    public void setLogout()
    {
        scrSetting.playername = "Guest";
    }
    public void setLogin()
    {
        if (ID.text == "Guest" || ID.text == "" || PW.text == "") return;
        st.SaveLocalPlayerAccount(ID.text, PW.text);
        st.LoadLocalPlayerAccount();
        
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
