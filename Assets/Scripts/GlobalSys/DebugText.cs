using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugText : MonoBehaviour
{
    public TextMeshProUGUI FPS, DELAY;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float ms = Mathf.Round(Time.smoothDeltaTime * 100000f) / 100f;
        DELAY.text = string.Format("{0:F2}", ms) + "ms";

        FPS.text = $"FPS:{ (int)(1f / Time.unscaledDeltaTime)}";
    }
}
