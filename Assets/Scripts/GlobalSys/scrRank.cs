using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

[Serializable]
public class Data
{
    public string playername;
    public int score;
    public bool passed;
}

public class scrRank : MonoBehaviour
{
    public void Save(int hash, int lvl, Data data)//세이브
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, data);
        string h = hash.ToString() + "D" + lvl;
        string result = Convert.ToBase64String(ms.GetBuffer());
        PlayerPrefs.SetString(h, result);//변환해서 playerPrefs에 저장
    }

    public Data Load(int hash, int lvl)//로드
    {
        Data data = null;
        string h = hash.ToString() + "D" + lvl;
        string save = PlayerPrefs.GetString(h, null);        
        if (!string.IsNullOrEmpty(save))
        {
            var binaryFormatter = new BinaryFormatter();
            var memoryStream = new MemoryStream(Convert.FromBase64String(save));
            data = (Data)binaryFormatter.Deserialize(memoryStream);//형변환해서 사용
        }
        return data;
    }
}