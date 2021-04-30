using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Security.Cryptography;
using System;
using System.Globalization;

public class FileLoader : MonoBehaviour
{
    public class Song
    {

        public string name;
        public string artist;
        public string tags;
        public bool hasvideo;
        public float localoffset;
        public string[] diff;
        string[] txtpath;
        string[] charter;
        string audiopath;
        string bgpath;
        string bgapath;
        string[] id;
        public Song(string name, string artist, ref string[] diffs, float offset, ref string[] id, ref string[] charter, ref string[] txtpath, string audiopath, string bgpath, string bgapath, bool hasvideo, string tags)
        {
            this.id = id;
            this.name = name;
            this.artist = artist;
            this.diff = diffs;
            localoffset = offset;
            this.txtpath = txtpath;
            this.charter = charter;
            this.audiopath = audiopath;
            this.bgpath = bgpath;
            this.bgapath = bgapath;
            this.hasvideo = hasvideo;
            this.tags = tags;
        }
        public string getAudio()
        {
            return audiopath;
        }
        public string getTxt(int idx)
        {
            return txtpath[idx];
        }
        public string getBGA()
        {
            return bgapath;
        }
        public string getBG()
        {
            return bgpath;
        }
        public string getDiff(int idx)
        {
            return diff[idx];
        }
        public string getCharter(int idx)
        {
            return charter[idx];
        }
        public string getID(int idx)
        {
            return id[idx];
        }
        public string maxDiff()
        {
            return diff[diff.Length - 1];
        }
        public int diffCount()
        {
            return diff.Length;
        }
    }
    public List<Song> list = new List<Song>();
    public List<Song> listorigin = new List<Song>();
    // Start is called before the first frame update
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        StartLoad(); //첫로드
        StartCoroutine(loadComplete());
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            list.Clear();
            listorigin.Clear();
            StartLoad();
            var s = GameObject.FindWithTag("SelSys");
            if (s)
            {
                s.GetComponent<FileSelecter>().init(); //리로드
            }
        }
    }
    public void StartLoad() //파일 로드 시작
    {
        int idx = 0;
        string root = Path.Combine(Application.streamingAssetsPath, "Songs");
        string[] subdirectoryEntries = Directory.GetDirectories(root);
        //폴더수만큼구조체배열할당

        foreach (string subdirectory in subdirectoryEntries)
        {
            LoadSubDirs(root, subdirectory, idx);
            idx++;
        }
    }
    void LoadSubDirs(string path, string dir, int idx) //폴더별 파일 로드
    {
        string AUDIOFILE, BACKGROUND, VIDEOFILE = "";
        bool hasvideo = false;
        DirectoryInfo d = new DirectoryInfo(dir);
        FileInfo[] Files = d.GetFiles("*.txt");
        string[] TXTFILE = new string[Files.Length]; //채보파일들
        string[] DIFFS = new string[Files.Length]; //채보별난이도들
        string[] CHARTERS = new string[Files.Length]; //채보별노터
        string[] _id = new string[Files.Length];     //채보별해쉬
        for (int i = 0; i < Files.Length; i++) //여러 난이도일 경우 여러번 반복
        {
            TXTFILE[i] = Path.Combine(path, dir, Files[i].Name); //경로합치기
        }
        //채보파일 불러오기        
        FileInfo[] Musics;
        Musics = d.GetFiles("*.mp3"); //음악파일 불러오기
        if (Musics.Length == 0) Musics = d.GetFiles("*.ogg");
        AUDIOFILE = Path.Combine(path, dir, Musics[0].Name);
        FileInfo[] bgs;
        bgs = d.GetFiles("*.jpg");    //배경 불러오기
        if (bgs.Length == 0) bgs = d.GetFiles("*.png");
        BACKGROUND = Path.Combine(path, dir, bgs[0].Name);
        FileInfo[] bga;
        bga = d.GetFiles("*.mpg"); // 동영상 불러오기
        if (bga.Length == 0) bga = d.GetFiles("*.mp4");
        if (bga.Length == 0) bga = d.GetFiles("*.flv");
        if (bga.Length != 0) { VIDEOFILE = Path.Combine(path, dir, bga[0].Name); hasvideo = true; }
        //제목,작곡가,난이도,오프셋파싱
        string _title = "", _artist = "", _tags = "";
        float _offset = 0f;
        string line;
        for (int i = 0; i < Files.Length; i++)
        {
            StreamReader rdr = new StreamReader(TXTFILE[i]);
            line = rdr.ReadLine();
            line = rdr.ReadLine(); _title = line;
            line = rdr.ReadLine(); _artist = line;
            line = rdr.ReadLine(); CHARTERS[i] = line;
            line = rdr.ReadLine(); _tags = line;
            line = rdr.ReadLine(); DIFFS[i] = line;
            line = rdr.ReadLine(); _offset = float.Parse(line);
            _id[i] = CalculateMD5(TXTFILE[i]); //무결성 검사
            Debug.Log($"{_title} {DIFFS[i]} Hash {_id[i]}");
            rdr.Close();
        }
        Song s;
        s = new Song(_title, _artist, ref DIFFS, _offset, ref _id, ref CHARTERS, ref TXTFILE, AUDIOFILE, BACKGROUND, VIDEOFILE, hasvideo, _tags);
        list.Add(s);
        listorigin.Add(s);
    }
    public void SortDiff()
    {
        list.Sort(delegate (Song A, Song B)
        {
            if (int.Parse(A.maxDiff()) > int.Parse(B.maxDiff())) return 1;
            else return -1;
        });
    }
    public void SortArtist()
    {
        list.Sort(delegate (Song A, Song B)
        {
            var comparer = StringComparer.Create(new CultureInfo("ja-JP"), true);
            return comparer.Compare(A.artist, B.artist);
        });
    }
    public void SortName()
    {
        list.Sort(delegate (Song A, Song B)
        {
            var comparer = StringComparer.Create(new CultureInfo("ja-JP"), true);
            return comparer.Compare(A.name, B.name);
        });
    }
    string CalculateMD5(string filename)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(filename))
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
    IEnumerator loadComplete()
    {

        yield return null;

        SceneManager.LoadScene("Title", LoadSceneMode.Single);
    }
}
