using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;


public enum KeyAction { _0, _1, _2, _3, _4, _5, _6, KEYCOUNT };

public enum Resolution { _1366_768, _1400_900, _1600_900, _1600_1024, _1920_1080};

[System.Serializable]
public static class KeySetting { public static Dictionary<KeyAction, KeyCode> keys = new Dictionary<KeyAction, KeyCode>(); }

public class scrSetting : MonoBehaviour
{
    KeyCode[] defaultKeys = new KeyCode[] { KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.Space, KeyCode.J, KeyCode.K, KeyCode.L };
    KeyCode[] newKeys = new KeyCode[] { KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.Space, KeyCode.J, KeyCode.K, KeyCode.L };

    public static Dictionary<string, Sprite> spriteMap;

    public static int[] judgems = { 0, 1, 2, 3 };

    public static float scrollSpeed = 2.4f, stageXPOS = 0, stageYPOS, ColWidth = 0.85f, GlobalOffset = 0f;
    public static int decide, diffselection, sortselection, modselection;
    public static string sortsearch = "", playername = "Tewi Inaba";


    public Sprite[] SquareNotes;
    public Sprite[] CircleNotes;

    public bool isCircleNote;
    
    public static bool isFixedScroll = true;
    public static bool isPlayVideo = true;
    public static bool isFullScreen = false;
    public static bool Mirror = false;
    public static bool Random = false;

    public static float Volume = 1f;

    public int res, fps;

    static public float vol, tp, sync, hprecover, hprecover2, baddamage, missdamage;

    // Start is called before the first frame update 
    void Awake()
    {

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

        hprecover = 0.003f; //퍼펙트 회복량

        hprecover2 = 0.0015f; //굿 회복량

        baddamage = 0.0001f;

        missdamage = 0.04f;
    }
    void Start()
    {

    }

    // Update is called once per frame 
    void Update()
    {

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
        PlayerPrefs.SetInt("DECIDE", decide);
        PlayerPrefs.SetInt("SORT", sortselection);
        PlayerPrefs.SetInt("MOD", modselection);
        PlayerPrefs.SetString("SEARCH",sortsearch);
}
    public void SaveSettings()
    {
        PlayerPrefs.SetInt("SCROLL", boolToInt(isFixedScroll));
        PlayerPrefs.SetInt("VIDEO", boolToInt(isPlayVideo));
        PlayerPrefs.SetInt("FSCREEN", boolToInt(isFullScreen));
        PlayerPrefs.SetInt("RESOLUTION", res);
        PlayerPrefs.SetInt("FPS", fps);
        PlayerPrefs.SetFloat("VOL", Volume);
        PlayerPrefs.SetFloat("GOFFSET", GlobalOffset);
    }
    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey("SCROLL")) {
            isFixedScroll = intToBool(PlayerPrefs.GetInt("SCROLL"));
            isPlayVideo = intToBool(PlayerPrefs.GetInt("VIDEO"));
            isFullScreen = intToBool(PlayerPrefs.GetInt("FSCREEN"));
            res = PlayerPrefs.GetInt("RESOLUTION");
            fps = PlayerPrefs.GetInt("FPS");
            Volume = PlayerPrefs.GetFloat("VOL");
            GlobalOffset = PlayerPrefs.GetFloat("GOFFSET", 0);
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
            decide = PlayerPrefs.GetInt("DECIDE");
            sortselection = PlayerPrefs.GetInt("SORT");
            modselection = PlayerPrefs.GetInt("MOD");
            sortsearch = PlayerPrefs.GetString("SEARCH");
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
                Application.targetFrameRate = 1000;
                break;

        }


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

