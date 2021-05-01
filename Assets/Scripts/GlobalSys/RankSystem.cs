using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

[Serializable]
class ScoreData
{
    string key;
    string playername;
    int score;
    int state;
    public ScoreData(string key, string playername, int score, int state)
    {
        this.key = key;
        this.playername = playername;
        this.score = score;
        this.state = state;
    }
}

public class RankSystem : MonoBehaviour
{
    List<ScoreData> scores = new List<ScoreData>();

    private void Start()
    {
        LoadScore();
    }



    public void SaveScore(string key, string playername, int score, int state)
    {
        //local
        scores.Add(new ScoreData(key, playername, score, state));
        string json = JsonConvert.SerializeObject(scores, Formatting.Indented);
        File.WriteAllText(Path.Combine(Application.dataPath, "scoredb.json"), json);
        //mysql

        ///////
    }

    public void LoadScore()
    {
        string json = "";
        if (File.Exists(Path.Combine(Application.dataPath, "scoredb.json")))
        {
            json = File.ReadAllText(Path.Combine(Application.dataPath, "scoredb.json"));
            List<ScoreData> scores = JsonConvert.DeserializeObject<List<ScoreData>>(json);
        }
    }
}

