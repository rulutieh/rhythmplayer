using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scrUIButton : MonoBehaviour
{
    public int id;
    Button BT;
    GameObject sys;
    private void Start()
    {
        BT = GetComponent<Button>();
        sys = GameObject.FindWithTag("TitleSys");
    }
    private void Update()
    {
        /*
        int idx = sys.GetComponent<scrUISystem>().activeUI;
        if (idx == id) BT.interactable = true;
        if (idx != id) BT.interactable = false;
        */
    }
}
