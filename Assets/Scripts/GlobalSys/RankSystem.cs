using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;


[Serializable]
class SongDB
{
    public string key; //hashtable

    public List<ScoreData> scores = new List<ScoreData>();

    public SongDB(string key)
    {
        this.key = key;
    }

    public void AddScore(string playername, int score,float acc, int state, int maxcombo, string date)
    {
        scores.Add(new ScoreData(playername, score, acc, state, maxcombo, date));
        SortScores();
    }

    void SortScores()
    {
        scores.Sort(delegate (ScoreData A, ScoreData B)
        {
            if (A.score == B.score) return 0;
            if (A.score < B.score) return 1;
            else return -1;
        });
    }
}


[Serializable]
class ScoreData
{
    public string playername;
    public string date;
    public float acc;
    public int score;
    public int state;
    public int maxcombo;
    public ScoreData(string playername, int score, float acc, int state, int maxcombo, string date)
    {
        this.acc = acc;
        this.playername = playername;
        this.score = score;
        this.state = state;
        this.maxcombo = maxcombo;
        this.date = date;
    }
}
[Serializable]
class ReceiveScoreData
{
    public string date;
    public string message;
    public string result;

    public static ReceiveScoreData CreateFromJson(string json)
    {
        return JsonConvert.DeserializeObject<ReceiveScoreData>(json);
    }
}




public class RankSystem : MonoBehaviour
{
    /*
    public delegate void ReceiveDelegate(string message);

    public event ReceiveDelegate OnReceive;
    */

    [SerializeField]
    List<SongDB> songs = new List<SongDB>();

    int id = -1;
    private void Awake()
    {

    }
    private void Start()
    {
        LoadScore();
    }
    //private void Update()
    //{
    //    if (Input.GetKey(KeyCode.A))
    //        SaveScore("323", "123", 123, 0, 2, "1");
    //}
    public void SelectSong(string key)
    {
        id = -1;
        for (int i = 0; i < songs.Count; i++)
        {
            if (songs[i].key == key) id = i;
        }
    }

    public int GetScoreCounts()
    {
        if (id == -1)
            return 0;
        else
            return songs[id].scores.Count;
    }
    public void GetInfo(int idx, out string pname, out int score, out float acc, out int state ,out int maxcombo, out string date)
    {
        pname = songs[id].scores[idx].playername;
        score = songs[id].scores[idx].score;
        acc = songs[id].scores[idx].acc;
        state = songs[id].scores[idx].state;
        maxcombo = songs[id].scores[idx].maxcombo;
        date = songs[id].scores[idx].date;
    }
    public void SaveScore(string key, string playername, int score, float acc, int state, int maxcombo, string date)
    {
        bool find = false;
        //local
        for (int i = 0; i < songs.Count; i++)
        {

            if (songs[i].key == key)
            {
                find = true;
                songs[i].AddScore(playername, score, acc, state, maxcombo, date);
            }
        }
        if (!find)
        {
            SongDB s = new SongDB(key);
            s.AddScore(playername, score,acc, state, maxcombo, date);
            songs.Add(s);
        }
        string json = JsonConvert.SerializeObject(songs, Formatting.Indented);
        json = CryptoManager.AESEncrypt128(json);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "scoredb.json"), json);
        //php+db

        //RequestToWebAsync("http://127.0.0.1:8000/print/hello");
        ///////
    }

    public void LoadScore()
    {
        string json = "";
        if (File.Exists(Path.Combine(Application.persistentDataPath, "scoredb.json")))
        {
            json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "scoredb.json"));
            json = CryptoManager.AESDecrypt128(json);
            songs = JsonConvert.DeserializeObject<List<SongDB>>(json);
        }
    }

    private string RequestToWeb(string url)
    {
        try
        {
            WebRequest webRequest;
            webRequest = WebRequest.Create(url);

            using (WebResponse res = webRequest.GetResponse())
            {
                Stream objStream;
                objStream = res.GetResponseStream();

                StreamReader objReader = new StreamReader(objStream);
                StringBuilder sb = new StringBuilder();

                string message = "";
                while (message != null)
                {
                    message = objReader.ReadLine();
                    if (message != null)
                        sb.Append(message);
                    var utf8Bytes = Encoding.UTF8.GetBytes(sb.ToString());
                    return Encoding.Default.GetString(utf8Bytes);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("asdf");
        }
        return "";
    }


    private async void RequestToWebAsync(string url)
    {
        var reqTask = Task.Run(() => this.RequestToWeb(url));
        var result = await reqTask;
        onReceive(result);
    }
    
    private void onReceive(string result)
    {
        var received = ReceiveScoreData.CreateFromJson(result);
        Debug.Log($"{received.date},{received.message},{received.result}");
    }
    



    //public void RegisterReceiver(ReceiveDelegate receiver)
    //{
    //    OnReceive += receiver;
    //}


}

