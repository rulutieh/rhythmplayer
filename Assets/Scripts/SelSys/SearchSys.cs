using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SearchSys : MonoBehaviour
{
    public GameObject inputfield;
    GameObject selsys;
    InputField tmp;
    FileLoader Loader;
    public string oldtext = "", text = "";
    // Start is called before the first frame update
    void Start()
    {
        Loader = GameObject.FindWithTag("FileSys").GetComponent<FileLoader>();
        tmp = inputfield.GetComponent<InputField>();
        selsys = GameObject.FindWithTag("SelSys");
        tmp.text = Manager.sortsearch;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.F2)) tmp.ActivateInputField();
        string inputext = tmp.text;
        if (string.Compare(inputext, oldtext) != 0)
        {
            Manager.decide = 0;
            Loader.searchbyHash(NowPlaying.PLAY.HASH); //���� �ֱ� ������ �� ã�� ������ �������
            text = inputext.ToUpper();
            oldtext = tmp.text;
            Manager.sortsearch = text;
            selsys.GetComponent<FileSelecter>().SortSearch(Manager.sortsearch);
        }
        if (tmp.text == "")
        {
            selsys.GetComponent<FileSelecter>().searching = false;
        }
        else selsys.GetComponent<FileSelecter>().searching = true;


    }

}
