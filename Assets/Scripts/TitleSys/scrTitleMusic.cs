using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrTitleMusic : MonoBehaviour
{
    AudioSource aud;
    AudioClip audClip;
    FileLoader files;
    bool loading;
    // Start is called before the first frame update
    void Start()
    {
        aud = GetComponent<AudioSource>();
        files = GameObject.FindWithTag("FileSys").GetComponent<FileLoader>();
    }

    // Update is called once per frame
    void Update()
    {

    }

}
