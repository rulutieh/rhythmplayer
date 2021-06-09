using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
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

    public void AddScore(ScoreData sd)
    {
        scores.Add(sd);
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
public class ScoreData
{
    public string playername;
    public string date;
    public int maxcombo;
    public int KOOL, COOL, GOOD, BAD, MISS;
    public int score, state;
    public float acc;
    [JsonConstructor]
    public ScoreData(string playername, int k, int c, int g, int b, int m, int maxcombo, string date)
    {
        this.playername = playername;
        this.maxcombo = maxcombo;
        this.date = date;
        KOOL = k; COOL = c; GOOD = g; BAD = b; MISS = m;
        getScores();
    }
    public ScoreData(ConvertOnlineScores o)
    {
        playername = o.uid;
        KOOL = o.kk;
        COOL = o.cc;
        GOOD = o.gg;
        BAD = o.bb;
        MISS = o.mm;
        maxcombo = o.maxcombo;
        date = o.created_at;
        getScores();
    }
    void getScores()
    {
        float c = KOOL + COOL + GOOD + MISS + BAD;
        float s = (GOOD / (2 * c)) + (BAD / (6 * c));
        float sum = (KOOL / c) + (COOL * 19f / (c * 20f)) + s;
        acc = ((KOOL + COOL) / c) + s;
        score = Mathf.RoundToInt(1000000f * sum);
        state = 0;
        if (BAD == 0 && MISS == 0) state = 1;
    }
}
[Serializable]
public class ConvertOnlineScores
{
    public string hash;
    public string uid;
    public int sco;
    public int kk, cc, gg, bb, mm;
    public int maxcombo;
    public int bitwise;
    public string created_at;
}
[Serializable]
class ReceiveResult
{
    [JsonProperty("date")]
    public string date { get; set; }
    [JsonProperty("message")]
    public string message { get; set; }
    [JsonProperty("result")]
    public string result { get; set; }

    public static ReceiveResult CreateFromJson(string json)
    {
        return JsonConvert.DeserializeObject<ReceiveResult>(json);


    }
}
[Serializable]
public class ReceiveRank
{
    [JsonProperty("myscore")]
    public ConvertOnlineScores myscore { get; set; }

    [JsonProperty("score")]
    public List<ConvertOnlineScores> scores { get; set; }

    [JsonProperty("result")]
    public string retrived { get; set; }

    [JsonProperty("ranking")]
    public int ranking { get; set; }
}




public class RankSystem : MonoBehaviour
{
    /*
    public delegate void ReceiveDelegate(string message);

    public event ReceiveDelegate OnReceive;
    */

    public enum PlayOption
    {
        NONE = 0,
        MIRROR = 1, RANDOM = 2, GROOVE = 4,
        EASY = 8, NORMAL = 16, HARD = 32,
        SDEATH = 64, PATTACK = 128
    }


    [SerializeField]
    List<SongDB> songs = new List<SongDB>();

    public List<ScoreData> onlineScores = new List<ScoreData>();

    public bool AsyncLoading = false;

    int id = -1;

    public ReceiveRank receivedScores;

    public ScoreData myScore;

    private void Awake()
    {

    }
    private void Start()
    {
        myScore = new ScoreData("", 0, 0, 0, 0, 0, 0, "");
        LoadScore();
        receivedScores = new ReceiveRank();
    }

    public void SelectSong(string key)
    {
        myScore = new ScoreData("", 0, 0, 0, 0, 0, 0, "");
        AsyncLoading = true;
        onlineScores.Clear();

        id = -1;
        for (int i = 0; i < songs.Count; i++)
        {
            if (songs[i].key == key) id = i;
        }
        Debug.Log($"key : {key}");
        string req = string.Format(
        "http://127.0.0.1:8000/api/score/get/{0}/uid/{1}",
        key,
        GlobalSettings.UID
        );
        RequestToWebAsync(req);
    }

    public int GetScoreCounts(bool online)
    {
        if (online)
        {
            return onlineScores.Count;
        }
        if (id == -1)
            return 0;
        else
        {
            return songs[id].scores.Count;
        }
    }
    public void GetInfo(bool isOnline, int idx, out string pname, out int score, out float acc, out int state, out int maxcombo, out string date)
    {
        ScoreData s;
        if (isOnline)
            s = onlineScores[idx];
        else
            s = songs[id].scores[idx];
        pname = s.playername;
        score = s.score;
        acc = s.acc;
        state = s.state;
        maxcombo = s.maxcombo;
        date = s.date;
    }
    public void GetMyScore(out string pname, out int score, out float acc, out int state, out int maxcombo, out string date, out int ranking)
    {
            ScoreData s = myScore;
            pname = s.playername;
            score = s.score;
            acc = s.acc;
            state = s.state;
            maxcombo = s.maxcombo;
            date = s.date;
        ranking = receivedScores.ranking;
    }
    public void SaveScore(string key, string playername, int k, int c, int g, int b, int m, int maxcombo, string date)
    {
        ScoreData sd = new ScoreData(playername, k, c, g, b, m, maxcombo, date);
        bool find = false;
        //local
        for (int i = 0; i < songs.Count; i++)
        {

            if (songs[i].key == key)
            {
                find = true;
                songs[i].AddScore(sd);
            }
        }
        if (!find)
        {
            SongDB s = new SongDB(key);
            s.AddScore(sd);
            songs.Add(s);
        }
        string json = JsonConvert.SerializeObject(songs, Formatting.Indented);
        json = CryptoManager.AESEncrypt128(json);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "scoredb.json"), json);
        //php+db

        int bitwise = 0;

        //mob/add/{name}/tier/{tier}/attack/{attack}/hp/{hp}/mp/{mp}
        string req = string.Format(
            "http://127.0.0.1:8000/api/score/add/{0}/uid/{1}/sco/{2}/kk/{3}/cc/{4}/gg/{5}/bb/{6}/mm/{7}/maxcombo/{8}/bitwise/{9}",
            key,
            GlobalSettings.UID,
            sd.score,
            k,
            c,
            g,
            b,
            m,
            maxcombo,
            bitwise
            );
        //Debug.Log(req);
        RequestToWebAsync(req);
        ///////
    }

    public void LoadScore()
    {
        string json = "";
        if (File.Exists(Path.Combine(Application.persistentDataPath, "scoredb.json")))
        {
            json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "scoredb.json"));
            json = CryptoManager.AESDecrypt128(json);
            Debug.Log(json);
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
            Debug.Log(ex);
        }
        return "";
    }


    private async void RequestToWebAsync(string url)
    {
        var reqTask = Task.Run(() => this.RequestToWeb(url));
        var result = await reqTask;

        onReceive(result);

        AsyncLoading = false;
    }

    private void onReceive(string result)
    {
        //ScoreData s = JsonConvert.DeserializeObject<ScoreData>(result);
        //onlineScores.Add(s);
        //Debug.Log($"{s.score},{s.playername},{s.acc}");
        try
        {
            var received = ReceiveResult.CreateFromJson(result);

            Debug.Log(received.result);

            if (received.result == "retrieved")
            {

                receivedScores = JsonConvert.DeserializeObject<ReceiveRank>(result);

                Debug.Log(receivedScores.ranking);

                if (receivedScores.scores.Count != 0)
                {
                    for (int i = 0; i < receivedScores.scores.Count; i++)
                    {

                        ScoreData s1 = new ScoreData(receivedScores.scores[i]);
                        onlineScores.Add(s1);
                    }
                    myScore = new ScoreData(receivedScores.myscore);
                }
            }
        }
        catch (Exception ie)
        {
            Debug.Log(ie);
            Debug.Log("No Network");
        }


    }



    //public void RegisterReceiver(ReceiveDelegate receiver)
    //{
    //    OnReceive += receiver;
    //}


}

