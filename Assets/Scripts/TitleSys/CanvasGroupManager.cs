using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CanvasGroupManager : MonoBehaviour
{
    public int id;
    CanvasGroup cv;
    GameObject sys;
    private void Start()
    {
        cv = GetComponent<CanvasGroup>();
        sys = GameObject.FindWithTag("TitleSys");
    }
    private void Update()
    {
        int idx = sys.GetComponent<scrUISystem>().activeUI;
        if (idx == id) cv.interactable = true;
        if (idx != id) cv.interactable = false;
    }
}
