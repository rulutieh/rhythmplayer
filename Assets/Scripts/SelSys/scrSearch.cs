using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class scrSearch : MonoBehaviour
{
    public GameObject inputfield;
    GameObject selsys;
    InputField tmp;
    public string oldtext = "", text = "";
    // Start is called before the first frame update
    void Start()
    {
        tmp = inputfield.GetComponent<InputField>();
        selsys = GameObject.FindWithTag("SelSys");
        tmp.text = scrSetting.sortsearch;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.F2)) tmp.ActivateInputField();
        string inputext = tmp.text;
        if (string.Compare(inputext, oldtext) != 0)
        {
            text = inputext.ToUpper();
            oldtext = tmp.text;
            scrSetting.sortsearch = text;
            selsys.GetComponent<FileSelecter>().SortSearch(scrSetting.sortsearch);
        }
        if (tmp.text == "")
        {
            selsys.GetComponent<FileSelecter>().searching = false;
        }else selsys.GetComponent<FileSelecter>().searching = true;

    }

}
