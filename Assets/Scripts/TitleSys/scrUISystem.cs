using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scrUISystem : MonoBehaviour
{
    public GameObject[] UIs;
    public int activeUI = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            if (activeUI > 0) Disable();
    }
    public void Enable()
    {
        activeUI++;
        //UIs[activeUI].GetComponent<CanvasGroup>().interactable = true;
        UIs[activeUI].GetComponent<scrUI>().Active();
    }
    public void Disable()
    {
        //UIs[activeUI].GetComponent<CanvasGroup>().interactable = false;
        UIs[activeUI].GetComponent<scrUI>().inActive();
        activeUI--;
    }
    public void NextRoom()
    {
        StartCoroutine(LoadScene("SelectMusic"));
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
