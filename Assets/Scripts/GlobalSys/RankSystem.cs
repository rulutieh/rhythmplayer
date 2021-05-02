using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

public class RankSystem : MonoBehaviour
{
    private static readonly string PASSWORD = "gsakljerngoan4alyn3oitajlf";

    private static readonly string KEY = PASSWORD.Substring(0, 128 / 8);

    [Serializable]
    class SongDB
    {
        public string key;

        public List<ScoreData> scores = new List<ScoreData>();

        public SongDB(string key)
        {
            this.key = key;
        }

        public void AddScore( string playername, int score, int state, int maxcombo, string date)
        {
            scores.Add(new ScoreData(playername, score, state, maxcombo, date));
            SortScores();
        }

        void SortScores()
        {
            scores.Sort(delegate (ScoreData A, ScoreData B)
            {
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
        public int score;
        public int state;
        public int maxcombo;
        public ScoreData(string playername, int score, int state, int maxcombo, string date)
        {
            this.playername = playername;
            this.score = score;
            this.state = state;
            this.maxcombo = maxcombo;
            this.date = date;
        }
    }
    [SerializeField]
    List<SongDB> songs = new List<SongDB>();

    int id = -1;

    private void Start()
    {
        LoadScore();
    }
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
    public int GetScores(int idx)
    {
        return songs[id].scores[idx].score;
    }
    public string GetPlayerName(int idx)
    {
        return songs[id].scores[idx].playername;
    }
    public int GetState(int idx)
    {
        return songs[id].scores[idx].state;
    }
    public int GetCombo(int idx)
    {
        return songs[id].scores[idx].maxcombo;
    }
    public string GetDate(int idx)
    {
        return songs[id].scores[idx].date;
    }
    public void SaveScore(string key, string playername, int score, int state, int maxcombo, string date)
    {
        bool find = false;
        //local
        for (int i = 0; i < songs.Count; i++)
        {

            if (songs[i].key == key)
            {
                find = true;
                songs[i].AddScore(playername, score, state, maxcombo, date);
            }
        }
        if (!find)
        {
            SongDB s = new SongDB(key);
            s.AddScore(playername, score, state, maxcombo, date);
            songs.Add(s);
        }
        string json = JsonConvert.SerializeObject(songs, Formatting.Indented);
        json = AESEncrypt128(json);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "scoredb.json"), json);
        //mysql

        ///////
    }

    public void LoadScore()
    {
        string json = "";
        if (File.Exists(Path.Combine(Application.persistentDataPath, "scoredb.json")))
        {
            json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "scoredb.json"));
            json = AESDecrypt128(json);
            songs = JsonConvert.DeserializeObject<List<SongDB>>(json);
        }
    }

    public static string AESEncrypt128(string plain)
    {
        byte[] plainBytes = Encoding.UTF8.GetBytes(plain);

        RijndaelManaged myRijndael = new RijndaelManaged();
        myRijndael.Mode = CipherMode.CBC;
        myRijndael.Padding = PaddingMode.PKCS7;
        myRijndael.KeySize = 128;

        MemoryStream memoryStream = new MemoryStream();

        ICryptoTransform encryptor = myRijndael.CreateEncryptor(Encoding.UTF8.GetBytes(KEY), Encoding.UTF8.GetBytes(KEY));

        CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        cryptoStream.Write(plainBytes, 0, plainBytes.Length);
        cryptoStream.FlushFinalBlock();

        byte[] encryptBytes = memoryStream.ToArray();

        string encryptString = Convert.ToBase64String(encryptBytes);

        cryptoStream.Close();
        memoryStream.Close();

        return encryptString;
    }

    public static string AESDecrypt128(string encrypt)
    {
        byte[] encryptBytes = Convert.FromBase64String(encrypt);

        RijndaelManaged myRijndael = new RijndaelManaged();
        myRijndael.Mode = CipherMode.CBC;
        myRijndael.Padding = PaddingMode.PKCS7;
        myRijndael.KeySize = 128;

        MemoryStream memoryStream = new MemoryStream(encryptBytes);

        ICryptoTransform decryptor = myRijndael.CreateDecryptor(Encoding.UTF8.GetBytes(KEY), Encoding.UTF8.GetBytes(KEY));

        CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

        byte[] plainBytes = new byte[encryptBytes.Length];

        int plainCount = cryptoStream.Read(plainBytes, 0, plainBytes.Length);

        string plainString = Encoding.UTF8.GetString(plainBytes, 0, plainCount);

        cryptoStream.Close();
        memoryStream.Close();

        return plainString;
    }

}

