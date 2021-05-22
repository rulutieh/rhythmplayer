using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(transform.position.x, 2.94f + 4.36f * ScoreManager.HP);
        if (ScoreManager.HP < 0f)
            transform.position = new Vector2(transform.position.x, 2.94f);
    }
}
