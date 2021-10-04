using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Ookii.Dialogs;
using System.IO;
using UnityEngine.SceneManagement;

public class SettingPanel : MonoBehaviour
{
    [SerializeField]
    GameObject PANEL, KSETTING, w;
    public static bool isUIanimationFinished;
    public bool init, signup, active, kactive, escdisable;
    public Slider slvolume, slkeysound, slcolumn, slstof, sljudgeline, sltrans;
    public Toggle video, cutoff, fullscreen;
    public TMP_Dropdown ddres, ddframe;
    public TextMeshProUGUI offset, errormessage, songroot;
    public UnityEngine.UI.Button KeySetting;
    //login
    public InputField ID, newID, nickField;
    public InputField PW, newPW, checkPW;

    public GameObject Loginpanel, Logoutpanel, Signuppanel;

    public TextMeshProUGUI loginas;

    MusicHandler player;
    GlobalSettings st;

    VistaFolderBrowserDialog OpenDialog;
    Stream openStream = null;

    public void Awake()
    {
        player = w.GetComponent<MusicHandler>();
        st = w.GetComponent<GlobalSettings>();
        slcolumn.value = GlobalSettings.ColWidth;
        slstof.value = GlobalSettings.stageXPOS;
        sljudgeline.value = GlobalSettings.stageYPOS;
        sltrans.value = GlobalSettings.Transparency;
        cutoff.isOn = GlobalSettings.isCutOff;
        video.isOn = GlobalSettings.isPlayVideo;
        fullscreen.isOn = GlobalSettings.isFullScreen;

        PANEL.SetActive(false);
        KSETTING.SetActive(false);

        #region EventListners
        cutoff.onValueChanged.AddListener(
            (bool bOn) =>
            {
                bool val = bOn;
                GlobalSettings.isCutOff = val;
                st.SaveSettings();
            }
        );

        video.onValueChanged.AddListener(
            (bool bOn) =>
            {
                bool val = bOn;
                GlobalSettings.isPlayVideo = val;
                st.SaveSettings();
            }
        );
        fullscreen.onValueChanged.AddListener(
            (bool bOn) =>
            {
                bool val = bOn;
                UnityEngine.Screen.fullScreen = val;
                st.SaveSettings();
            }
        );
        slvolume.onValueChanged.AddListener(
            (float vl) =>
            {
                GlobalSettings.Volume = vl;
                player.SetVolume();
                st.SaveSettings();
            }
        );
        slcolumn.onValueChanged.AddListener(
            (float cw) =>
            {
                GlobalSettings.ColWidth = cw;
                st.SaveSettings();
            }
        );
        slstof.onValueChanged.AddListener(
            (float cw) =>
            {
                GlobalSettings.stageXPOS = cw;
                st.SaveSettings();
            }
        );
        sljudgeline.onValueChanged.AddListener(
            (float cw) =>
            {
                GlobalSettings.stageYPOS = cw;
                st.SaveSettings();
            }
        );
        sltrans.onValueChanged.AddListener(
            (float cw) =>
            {
                GlobalSettings.Transparency = cw;
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
            slvolume.value = GlobalSettings.Volume;
            init = true;
        }

        songroot.text = GlobalSettings.FolderPath;

        OpenDialog = new VistaFolderBrowserDialog();

    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.F10))
        {
            if (!active)
            {
                active = true;
                escdisable = true;
                PANEL.SetActive(true);
            }
            else
            {
                active = false;
                StartCoroutine(esc());
                PANEL.SetActive(false);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (kactive)
            {
                KSETTING.SetActive(false);
                kactive = false;
            }
            else if (active)
            {
                active = false;
                PANEL.SetActive(false);
                StartCoroutine(esc());
            }

        }
        if (active)
        {
            Scene scene = SceneManager.GetActiveScene();
            if (scene.name == "PlayMusic")
            {
                KSETTING.SetActive(false);
                kactive = false;
                active = false;
                PANEL.SetActive(false);
            }
        }


        float f = Mathf.Round(GlobalSettings.GlobalOffset * 100f) / 100f;
        offset.text = f.ToString();
        //계정 패널 관련
        /*
        if (uisys.activeUI == 3 && isUIanimationFinished)
        {
            if (signup)
            {
                Loginpanel.SetActive(false);
                Logoutpanel.SetActive(false);
                Signuppanel.SetActive(true);
            }
            else if (GlobalSettings.playername == "Guest")
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
        
        loginas.text = GlobalSettings.playername;
        */
    }
    IEnumerator esc()
    {
        yield return new WaitForSeconds(0.1f);
        Debug.Log("a");

        escdisable = false;
    }
    #region Account Settings
    public void setLogout()
    {
        GlobalSettings.playername = "Guest";
    }
    public void setLogin()
    {
        if (ID.text == "" || PW.text == "") return;
        //빈칸 리턴
        st.SaveLocalPlayerAccount(ID.text, PW.text);
        //ID PW 기억 및 로그인
        string result = st.LoadLocalPlayerAccount();
        errormessage.text = result;
        if (result == "Login Success") ClickSound(3);
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
        if (Regex.IsMatch(newPW.text, "^[a-zA-Z0-9]*$"))
        {
            errormessage.text = "include least one special chars";
            return;
        }
        errormessage.text = "Account Created";
        //계정 생성 후 리스트 저장
        st.SigninLocalPlayerAccount(id, newPW.text, nickField.text);
        //저장 후 불러오기
        st.LoadPlayerAccounts();
        signup = false;
        ClickSound(3);
        newID.text = ""; newPW.text = ""; nickField.text = "";
    }
    public void setBool()
    {
        errormessage.text = "";
        signup = !signup;
    }
    #endregion
    #region Game Play Settings
    public void SetKeySet()
    {
        if (!kactive)
        {
            KSETTING.SetActive(true);
            kactive = true;
        }
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
        if (init)
            player.PlaySFX(index);
    }
    public void SetGlobalOffset(float o)
    {
        GlobalSettings.GlobalOffset += o;
        st.SaveSettings();
        ClickSound(4);
    }
    public void SetFolderPath()
    {
        FileLoader fl = GameObject.FindWithTag("FileSys").GetComponent<FileLoader>();
        if (!fl.threading)
        if (OpenDialog.ShowDialog() == DialogResult.OK)
        {
            string path = OpenDialog.SelectedPath;
            if (path == null) return;
            GlobalSettings.FolderPath = path;
            PlayerPrefs.SetString("PATH", path);
            PlayerPrefs.Save();
            songroot.text = GlobalSettings.FolderPath;
            
            fl.ReLoad();
        }
    }
    #endregion
}
