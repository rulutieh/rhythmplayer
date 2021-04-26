using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomChanger : MonoBehaviour
{
    public static RoomChanger roomchanger;
    GameObject sel;
    // Start is called before the first frame update
    void Start()
    {
        roomchanger = this.GetComponent<RoomChanger>();
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "PlayMusic")
        {

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StartCoroutine(LoadScene("SelectMusic"));
                GetComponent<MusicHandler>().StopMP3();
                GetComponent<MusicHandler>().ReleaseMP3();
            }
        }
    }
    public void goRoom(string rname)
    {
        StartCoroutine(LoadScene(rname));
    }
    IEnumerator LoadScene(string scenename)
    {
        AsyncOperation asyncOper = SceneManager.LoadSceneAsync(scenename);
        while (!asyncOper.isDone)
        {
            yield return null;
        }
    }
}
