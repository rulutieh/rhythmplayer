using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasureBars : MonoBehaviour
{
    public float _TIME;
    FileReader rdr;
    private void Awake()
    {
        var sys = GameObject.FindWithTag("NoteSys");
        rdr = sys.GetComponent<FileReader>();
    }
    private void LateUpdate()
    {
        transform.localScale = new Vector2(1.1f * GlobalSettings.ColWidth, 0.70f);
        transform.position = new Vector2(transform.position.x, (float)
            (FileReader.judgeoffset + (_TIME - FileReader.PlaybackChanged) * FileReader.multiply));
        if (transform.position.y < -5f)
        {
            transform.position = new Vector2(0, 1000f);
            rdr.b_queue.Enqueue(this.gameObject);
            this.gameObject.SetActive(false);

        }
    }
}
