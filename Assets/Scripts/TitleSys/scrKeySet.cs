using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class scrKeySet : MonoBehaviour
{
    public GameObject sets;
    public TextMeshProUGUI[] keynum;
    int key = -1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < keynum.Length; i++)
            keynum[i].text = KeySetting.keys[(KeyAction)i].ToString();
    }
    private void OnGUI()
    {
        Event keyEvent = Event.current;
        if (keyEvent.isKey)
        {
            if (keyEvent.keyCode != KeyCode.Escape)
                KeySetting.keys[(KeyAction)key] = keyEvent.keyCode;
            key = -1;
            sets.GetComponent<SettingPanel>().ClickSound(1);
            scrSetting.SaveControlKeybinds();
        }
    }
    public void ChangeKey(int num)
    {
        key = num;
    }
}
