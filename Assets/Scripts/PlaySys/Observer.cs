using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
    public delegate void event_handler(int idx);
    public event_handler eventhandler;

    public void OnNotify(int idx)
    {
        eventhandler(idx);
    }
}