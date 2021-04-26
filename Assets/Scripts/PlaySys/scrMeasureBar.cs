using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrMeasureBar : MonoBehaviour
{
    public float _TIME;
    private void LateUpdate()
    {
        transform.position = new Vector2(transform.position.x, (float)
            (FileReader.judgeoffset + (_TIME - FileReader.PlaybackChanged) * FileReader.multiply));
        if (transform.position.y < -5f)
            Destroy(gameObject);
        
    }
}
