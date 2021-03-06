using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using Newtonsoft.Json;

public enum KeyAction { _0, _1, _2, _3, _4, _5, _6, KEYCOUNT };

public enum Resolution { _1366_768, _1400_900, _1600_900, _1600_1024, _1920_1080};


class PlayerIDPW
{
    public string id, pw;
}
class PlayerAccount
{
    public string id, pw, name;
}

class PlayerOnlineStatus
{
    public string nickname;
    public int uid;
}


[System.Serializable]
public static class KeySetting { public static Dictionary<KeyAction, KeyCode> keys = new Dictionary<KeyAction, KeyCode>(); }

public class Manager : MonoBehaviour
{


    KeyCode[] defaultKeys = new KeyCode[] { KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.Space, KeyCode.J, KeyCode.K, KeyCode.L };
    KeyCode[] newKeys = new KeyCode[] { KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.Space, KeyCode.J, KeyCode.K, KeyCode.L };


    

    public static Dictionary<string, Sprite> spriteMap;

    public static int[] judgems = { 0, 1, 2, 3 };

    public static float scrollSpeed = 2.4f, stageXPOS = 0, stageYPOS, GlobalOffset = 0f, Transparency = 1f;
    public static float ColWidth = 0.85f, ColumnWidth; //7키일시 컬럼 넓이, 다른 키카운트 컬럼넓이
    public static int decide, diffselection, sortselection, modselection, specialselection;
    public static string sortsearch = "", playername = "TEST", email = "";
    public static string FolderPath = Path.Combine(Application.streamingAssetsPath, "Songs");

    public static int UID = 2;

    public Sprite[] SquareNotes;
    public Sprite[] CircleNotes;

    public bool isCircleNote;

    public static bool isCutOff = false;
    public static bool isPlayVideo = true;
    public static bool isFullScreen = false;
    public static bool Mirror = false;
    public static bool Random = false;
    public static bool AutoPlay = false;

    public static float Volume = 1f;

    public int res, fps;

    static public float vol, tp, sync;

    public static int keycount = 4;

    string defpath = Path.Combine(Application.streamingAssetsPath, "Songs");

    

    //임시 로컬 회원가입
    List<PlayerAccount> accList = new List<PlayerAccount>();

    // Start is called before the first frame update 
    void Awake()
    {
        QualitySettings.vSyncCount = -1;
        decide = 0;
        //저장값 불러오기
        if (PlayerPrefs.HasKey("K0"))
        {
            newKeys[0] = (KeyCode)PlayerPrefs.GetInt("K0");
            newKeys[1] = (KeyCode)PlayerPrefs.GetInt("K1");
            newKeys[2] = (KeyCode)PlayerPrefs.GetInt("K2");
            newKeys[3] = (KeyCode)PlayerPrefs.GetInt("K3");
            newKeys[4] = (KeyCode)PlayerPrefs.GetInt("K4");
            newKeys[5] = (KeyCode)PlayerPrefs.GetInt("K5");
            newKeys[6] = (KeyCode)PlayerPrefs.GetInt("K6");

            for (int i = 0; i < (int)KeyAction.KEYCOUNT; i++)
            {
                KeySetting.keys.Add((KeyAction)i, newKeys[i]);
            }
        }
        else
        {
            //초기값
            for (int i = 0; i < (int)KeyAction.KEYCOUNT; i++)
            {
                KeySetting.keys.Add((KeyAction)i, defaultKeys[i]);
            }
        }
        fps = 3;
        res = (int)Resolution._1920_1080;
        LoadSettings();
        LoadSelection();
        //LoadPlayerAccounts();
        //LoadLocalPlayerAccount();

    }
    private void Update()
    {
        ColumnWidth = ColWidth * (7f / keycount);
    }

    public static void SaveControlKeybinds()
    {
        int k;
        k = (int)KeySetting.keys[KeyAction._0]; PlayerPrefs.SetInt("K0", k);
        k = (int)KeySetting.keys[KeyAction._1]; PlayerPrefs.SetInt("K1", k);
        k = (int)KeySetting.keys[KeyAction._2]; PlayerPrefs.SetInt("K2", k);
        k = (int)KeySetting.keys[KeyAction._3]; PlayerPrefs.SetInt("K3", k);
        k = (int)KeySetting.keys[KeyAction._4]; PlayerPrefs.SetInt("K4", k);
        k = (int)KeySetting.keys[KeyAction._5]; PlayerPrefs.SetInt("K5", k);
        k = (int)KeySetting.keys[KeyAction._6]; PlayerPrefs.SetInt("K6", k);

        PlayerPrefs.Save();
    }
    public void SetNoteSprites()
    {
        if (!isCircleNote)
        {
            spriteMap.Add("sfjl.N", SquareNotes[0]);
            spriteMap.Add("sfjl.L", SquareNotes[1]);
            spriteMap.Add("dk.N", SquareNotes[2]);
            spriteMap.Add("dk.L", SquareNotes[3]);
            spriteMap.Add("sp.N", SquareNotes[4]);
            spriteMap.Add("sp.L", SquareNotes[5]);
        }
        else
        {
            spriteMap.Add("sfjl.N", CircleNotes[0]);
            spriteMap.Add("sfjl.L", CircleNotes[1]);
            spriteMap.Add("dk.N", CircleNotes[2]);
            spriteMap.Add("dk.L", CircleNotes[3]);
            spriteMap.Add("sp.N", CircleNotes[4]);
            spriteMap.Add("sp.L", CircleNotes[5]);
        }
    }
    public static Sprite GetSprite(string key)
    {
        return spriteMap[key];
    }
    public void SaveSelection()
    {
        PlayerPrefs.SetFloat("SPEED", scrollSpeed);
        PlayerPrefs.SetInt("MOD", modselection);
}
    public void SaveSettings()
    {
        PlayerPrefs.SetInt("CUT", boolToInt(isCutOff));
        PlayerPrefs.SetInt("VIDEO", boolToInt(isPlayVideo));
        PlayerPrefs.SetInt("FSCREEN", boolToInt(isFullScreen));
        PlayerPrefs.SetInt("RESOLUTION", res);
        PlayerPrefs.SetInt("FPS", fps);
        PlayerPrefs.SetFloat("VOL", Volume);
        PlayerPrefs.SetFloat("GOFFSET", GlobalOffset);
        PlayerPrefs.SetFloat("CW", ColWidth);
        PlayerPrefs.SetFloat("XX", stageXPOS);
        PlayerPrefs.SetFloat("YY", stageYPOS);
        PlayerPrefs.SetFloat("TR", Transparency);
        PlayerPrefs.SetString("PATH", FolderPath);

        PlayerPrefs.Save();
    }
    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey("FPS")) {
            isCutOff = intToBool(PlayerPrefs.GetInt("CUT", 0));
            isPlayVideo = intToBool(PlayerPrefs.GetInt("VIDEO"));
            isFullScreen = intToBool(PlayerPrefs.GetInt("FSCREEN"));
            res = PlayerPrefs.GetInt("RESOLUTION");
            fps = PlayerPrefs.GetInt("FPS");
            Volume = PlayerPrefs.GetFloat("VOL");
            GlobalOffset = PlayerPrefs.GetFloat("GOFFSET", 0);
            ColWidth = PlayerPrefs.GetFloat("CW", 0.85f);
            Transparency = PlayerPrefs.GetFloat("TR", 1);
            stageXPOS = PlayerPrefs.GetFloat("XX", 0);
            stageYPOS = PlayerPrefs.GetFloat("YY", 0);
            FolderPath = PlayerPrefs.GetString("PATH", defpath);
        }

        SwitchResolution();

        SwitchFrameRate();

        if (isFullScreen)
        {
            Screen.fullScreen = true;
        }
        else
            Screen.fullScreen = false;
    }
    public void LoadSelection()
    {
        if (PlayerPrefs.HasKey("SPEED"))
        {
            scrollSpeed = PlayerPrefs.GetFloat("SPEED");
            modselection = PlayerPrefs.GetInt("MOD");
        }
    }


    public void SwitchResolution()
    {
        switch(res)
        {
            case (int)Resolution._1366_768:
                Screen.SetResolution(1366, 768, isFullScreen);
                break;
            case (int)Resolution._1400_900:
                Screen.SetResolution(1400, 900, isFullScreen);
                break;
            case (int)Resolution._1600_900:
                Screen.SetResolution(1600, 900, isFullScreen);
                break;
            case (int)Resolution._1600_1024:
                Screen.SetResolution(1600, 1024, isFullScreen);
                break;
            case (int)Resolution._1920_1080:
                Screen.SetResolution(1920, 1080, isFullScreen);
                break;
        }
    }
    public void SwitchFrameRate()
    {
        switch(fps)
        {
            case 0:
                Application.targetFrameRate = 60;
                break;
            case 1:
                Application.targetFrameRate = 144;
                break;
            case 2:
                Application.targetFrameRate = 400;
                break;
            case 3:
                Application.targetFrameRate = 960;
                break;

        }


    }
    public void SigninLocalPlayerAccount(string id, string pw, string nick)
    {
        PlayerAccount a = new PlayerAccount();
        a.id = id; a.pw = pw; a.name = nick;
        accList.Add(a); //회원가입 (임시로 json저장, mysql대체예정)
        string json = JsonConvert.SerializeObject(accList, Formatting.Indented);
        json = CryptoManager.AESEncrypt128(json);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "accounts.json"), json);
    }

    public void LoadPlayerAccounts()
    {
        string json = "";
        if (File.Exists(Path.Combine(Application.persistentDataPath, "accounts.json")))
        {
            json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "accounts.json"));
            json = CryptoManager.AESDecrypt128(json);
            accList = JsonConvert.DeserializeObject<List<PlayerAccount>>(json);
        }
    }

    public void SaveLocalPlayerAccount(string id, string pw)
    {
        PlayerIDPW p = new PlayerIDPW();
        p.id = id; p.pw = pw;

        string json = JsonConvert.SerializeObject(p, Formatting.Indented);
        json = CryptoManager.AESEncrypt128(json);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "player.json"), json);
    }

    public string LoadLocalPlayerAccount()
    {
        string json = "";
        playername = "Guest";
        if (File.Exists(Path.Combine(Application.persistentDataPath, "player.json")))
        {
            json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "player.json"));
            json = CryptoManager.AESDecrypt128(json);
            PlayerIDPW pid = JsonConvert.DeserializeObject<PlayerIDPW>(json);
            for (int i = 0; i < accList.Count; i++)
            {
                if (pid.id == accList[i].id && pid.pw == accList[i].pw)
                {
                    playername = accList[i].name;
                    
                    return "Login Success";
                    
                }
            }
        }
        return "Login Failed";
    }





    int boolToInt(bool val)
    {
        if (val)
            return 1;
        else
            return 0;
    }

    bool intToBool(int val)
    {
        if (val != 0)
            return true;
        else
            return false;
    }

}

