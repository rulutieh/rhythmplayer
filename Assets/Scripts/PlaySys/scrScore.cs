using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class scrScore : MonoBehaviour
{
    TextMeshProUGUI tmp;
    int score, oldscore;
    // Start is called before the first frame update
    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        score = (int)Mathf.Round(FileReader.Score);
        if (score != oldscore)
        {
            tmp.text = score.ToString("0000000");
            oldscore = score;
        }
        
        
    }
}
