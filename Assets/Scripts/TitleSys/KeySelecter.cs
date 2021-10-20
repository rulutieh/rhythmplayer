using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeySelecter : MonoBehaviour, IPointerClickHandler
{
    public int keycount;
    public GameObject errScreen;
    public void OnPointerClick(PointerEventData eventData)
    {
        Manager.keycount = keycount;
        NextRoom();
    }
    public void NextRoom()
    {
        FileLoader fl = GameObject.FindWithTag("FileSys").GetComponent<FileLoader>();
        if (fl.listorigin.Count == 0 || fl.threading)
        {
            errScreen.SetActive(true);
        }
        else
        {
            fl.SortByKeycounts();

        }
        if (fl.listkeysort.Count == 0)
        {
            errScreen.SetActive(true);
        }
        else
        {
            RoomChanger.roomchanger.goRoom("SelectMusic");
        }
    }
}
