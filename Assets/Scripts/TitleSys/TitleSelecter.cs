using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class TitleSelecter : MonoBehaviour, IPointerClickHandler
{
    Vector2 myPos;
    RectTransform rect;
    FileLoader fl;
    public bool menuActive = false;
    public GameObject Key4, Key7;
    public GameObject errScreen;
    public int activeUI = 0;
    // Start is called before the first frame update
    void Start()
    {
        fl = GameObject.FindWithTag("FileSys").GetComponent<FileLoader>();
        Manager.decide = 0;
        rect = GetComponent<RectTransform>();
        myPos = rect.anchoredPosition;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        NextMenu();
        fl.list.Clear();
        fl.listkeysort.Clear();

    }
    void Update()
    {
        
        Vector2 v = new Vector2(-90.5f, rect.anchoredPosition.y);
        if (menuActive)
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, v, 6f * Time.deltaTime);
        else
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, myPos, 6f * Time.deltaTime);
    }
    public void NextMenu()
    {

        if (menuActive)
        {
            menuActive = false;
            Key4.SetActive(false);
            Key7.SetActive(false);
        }
        else
        {
            if (fl.listorigin.Count == 0 || fl.threading)
            {
                errScreen.SetActive(true);
                StartCoroutine(HideError());
            }
            else
            {
                fl.SortByKeycounts();

                if (fl.listkeysort.Count == 0)
                {
                    errScreen.SetActive(true);
                    StartCoroutine(HideError());
                }
                else
                {
                    menuActive = true;
                    StartCoroutine(ShowMenu());
                }
            }

        }
    }

    IEnumerator ShowMenu()
    {
        
        yield return new WaitForSeconds(0.5f);
        Key4.SetActive(true);
        Key7.SetActive(true);
    }
    IEnumerator HideError()
    {
        yield return new WaitForSeconds(4f);
        errScreen.SetActive(false);
    }
}
