using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrHpBar : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (FileReader.HP >= -0.1f)
            transform.localPosition = new Vector2(transform.localPosition.x, 7.74f - 4.42f * (1f - FileReader.HP));
    }
}
