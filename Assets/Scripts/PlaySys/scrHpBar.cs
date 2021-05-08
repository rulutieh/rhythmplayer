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
            transform.position = new Vector2(transform.position.x, 2.94f + 4.36f * FileReader.HP);
    }
}
