using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class DiffSelBT : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    int value;
    FileSelecter Select;
    // Start is called before the first frame update
    void Start()
    {
        Select = GameObject.FindWithTag("SelSys").GetComponent<FileSelecter>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (value > 0)
            Select.diffPlus = true;
        else if (value < 0)
            Select.diffMinus = true;
        else
            Select.songDecide();
    }
}
