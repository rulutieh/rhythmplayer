using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FPSManager : MonoBehaviour
{
    Scene curScene;
    GlobalSettings set;
    private void Awake()
    {
        set = GetComponent<GlobalSettings>();
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        curScene = scene;
        SetFrameRate(curScene);

    }
    void SetFrameRate(Scene scene)
    {
        if (scene.name == "PlayMusic")
        {
            set.SwitchFrameRate();
            Debug.Log("frame");
        }
        else
        {
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
        }
    }
    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            SetFrameRate(curScene);
        }
        else
        {
            Application.targetFrameRate = 30;
        }
    }
}
