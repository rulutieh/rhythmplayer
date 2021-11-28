using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FPSManager : MonoBehaviour
{
    Scene curScene;
    Manager set;
    private void Awake()
    {
        set = GetComponent<Manager>();
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
            Application.targetFrameRate = 60;
        }
    }
}
