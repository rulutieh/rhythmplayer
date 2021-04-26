using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class showBPM : MonoBehaviour
{
    // Start is called before the first frame update
    TextMeshProUGUI tmp;
    float bpm, oldbpm;
    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        bpm = Mathf.Round((1f / FileReader.bpm * 1000f * 60f) * 100f) / 100f;
        if (bpm != oldbpm)
        {
            tmp.text = "BPM : " + bpm.ToString();
            oldbpm = bpm;
        }
    }
}
