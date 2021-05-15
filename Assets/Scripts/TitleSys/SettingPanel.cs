using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingPanel : MonoBehaviour
{
    public static bool isUIanimationFinished;
    public bool init, signup;
    public scrUISystem uisys;
    public Slider slvolume, slcolumn;
    public Toggle video, bpm, fullscreen;
    public Dropdown ddres, ddframe;
    public TextMeshProUGUI offset, errormessage;
    //login
    public InputField ID, newID, nickField;
    public InputField PW, newPW, checkPW;

    public GameObject Loginpanel, Logoutpanel, Signuppanel;

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

        #region EventListners
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
        #endregion
    }
    private void Start()
    {

        //UI 표시용 설정값 불러오기
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
        //계정 패널 관련
        if (uisys.activeUI == 3 && isUIanimationFinished)
        {
            if (signup)
            {
                Loginpanel.SetActive(false);
                Logoutpanel.SetActive(false);
                Signuppanel.SetActive(true);
            }
            else if (scrSetting.playername == "Guest")
            {
                Loginpanel.SetActive(true);
                Logoutpanel.SetActive(false);
                Signuppanel.SetActive(false);
            }
            else
            {
                Loginpanel.SetActive(false);
                Logoutpanel.SetActive(true);
                Signuppanel.SetActive(false);
            }
        }
        loginas.text = scrSetting.playername;
    }
    #region Account Settings
    public void setLogout()
    {
        scrSetting.playername = "Guest";
    }
    public void setLogin()
    {
        if (ID.text == "" || PW.text == "") return;
        //빈칸 리턴
        st.SaveLocalPlayerAccount(ID.text, PW.text);
        //ID PW 기억 및 로그인
        string result = st.LoadLocalPlayerAccount();
        errormessage.text = result;
        //불러오기, 로그인 결과 출력
    }
    public void setSignUp()
    {
        //로그인
        //email 형식 체크
        string id = newID.text;
        if (!id.Contains("@"))
        {
            errormessage.text = "wrong Email 1 ";
            return;
        }
        string[] idsplit = id.Split('@');
        if (idsplit.Length != 2 || idsplit[0] == "")
        {
            errormessage.text = "wrong Email 2";
            return;
        }
        if (!idsplit[1].Contains("."))
        {
            errormessage.text = "wrong domain";
            return;
        }
        //닉네임 형식 체크
        if (nickField.text == "Guest" || nickField.text.Length < 4)
        {
            errormessage.text = "wrong Nickname";
            return;
        }
        //암호 형식, 확인 일치 체크
        if (newPW.text != checkPW.text)
        {
            errormessage.text = "incorrect PW";
            return;
        }
        if (newPW.text.Length < 8)
        {
            errormessage.text = "must be greater than 7";
            return;
        }
        //계정 생성 후 리스트 저장
        st.SigninLocalPlayerAccount(id, newPW.text, nickField.text);
        //저장 후 불러오기
        st.LoadPlayerAccounts();
        signup = false;
        errormessage.text = "Account Created";
        newID.text = ""; newPW.text = ""; nickField.text = "";
    }
    public void setBool()
    {
        errormessage.text = "";
        signup = !signup;
    }
    #endregion
    #region Game Play Settings
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
        if (init)
            player.PlaySFX(index);
    }
    public void SetGlobalOffset(float o)
    {
        scrSetting.GlobalOffset += o;
        st.SaveSettings();
        ClickSound(1);
    }
    #endregion
}
