using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasureBars : MonoBehaviour
{
    public float _TIME;
    NotePlayer rdr;
    private void Awake()
    {
        var sys = GameObject.FindWithTag("NoteSys");
        rdr = sys.GetComponent<NotePlayer>();
    }
    private void LateUpdate()
    {
        transform.localScale = new Vector2(1.1f * Manager.ColWidth, 0.70f);
        transform.position = new Vector2(transform.position.x, (float)
            (NotePlayer.judgeoffset + (_TIME - NotePlayer.PlaybackChanged) * NotePlayer.multiply));
        if (transform.position.y < -5f)
        {
            transform.position = new Vector2(0, 1000f);
            rdr.b_queue.Enqueue(this.gameObject);
            this.gameObject.SetActive(false);

        }
    }
}
