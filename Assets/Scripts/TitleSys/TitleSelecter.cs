using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class TitleSelecter : MonoBehaviour, IPointerClickHandler
{
    public GameObject[] UIs;
    public GameObject errScreen;
    public int activeUI = 0;
    // Start is called before the first frame update
    void Start()
    {
        GlobalSettings.decide = 0;
    }

    // Update is called once per frame
    /*
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            if (activeUI == 3)
            {
                Disable2();
            }
        else
            if (activeUI > 0)
            {
                Disable();
            }
    }
    
    public void Enable()
    {
        activeUI++;
        //UIs[activeUI].GetComponent<CanvasGroup>().interactable = true;
        UIs[activeUI].GetComponent<scrUI>().Active();
    }
    public void Enable2()
    {
        activeUI+=2;
        //UIs[activeUI].GetComponent<CanvasGroup>().interactable = true;
        UIs[activeUI].GetComponent<scrUI>().Active();
    }
    public void Disable()
    {
        //UIs[activeUI].GetComponent<CanvasGroup>().interactable = false;
        UIs[activeUI].GetComponent<scrUI>().inActive();
        activeUI--;
    }
    public void Disable2()
    {
        //UIs[activeUI].GetComponent<CanvasGroup>().interactable = false;
        UIs[activeUI].GetComponent<scrUI>().inActive();
        activeUI -= 2;
    }
    */

    public void OnPointerClick(PointerEventData eventData)
    {
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
            StartCoroutine(LoadScene("SelectMusic"));
        }
    }

    IEnumerator LoadScene(string sceneName)
    {
        AsyncOperation asyncOper = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncOper.isDone)
        {
            yield return null;
        }
    }
}
