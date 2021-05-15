using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrMeasureBar : MonoBehaviour
{
    public float _TIME;
    private void LateUpdate()
    {
        transform.localScale = new Vector2(1.2f * scrSetting.ColWidth, 0.74848f);
        transform.position = new Vector2(transform.position.x, (float)
            (FileReader.judgeoffset + (_TIME - FileReader.PlaybackChanged) * FileReader.multiply));
        if (transform.position.y < -5f)
            Destroy(gameObject);
        
    }
}
