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
    public class Chart
    {
        public string hash, charter, path, diffs;
        public Chart(string hash, string charter, string path, string diffs)
        {
            this.hash = hash; this.charter = charter; this.path = path; this.diffs = diffs;
        }
    }
    public class Song
    {
        public string AudioPath { get; set; }
        public string BGAPath { get; set; }
        public string BGPath { get; set; }
        public string name { get; set; }
        public string artist { get; set; }
        public string tags { get; set; }
        public bool hasvideo { get; set; }
        public float localoffset { get; set; }
        public string directory { get; set; }
        List<Chart> charts = new List<Chart>();
        public Song(string dir, string name, string artist, ref string[] diffs, float offset, ref string[] id, ref string[] charter, ref string[] txtpath, string audiopath, string bgpath, string bgapath, bool hasvideo, string tags)
        {

            for (int i = 0; i < id.Length; i++)
            {
                charts.Add(new Chart(id[i], charter[i], txtpath[i], diffs[i]));
            }
            AudioPath = audiopath;
            BGAPath = bgapath;
            BGPath = bgpath;
            directory = dir;
            this.name = name;
            this.artist = artist;
            localoffset = offset;
            this.hasvideo = hasvideo;
            this.tags = tags;
            SortDiff();
        }
        void SortDiff()
        {

            charts.Sort(delegate (Chart A, Chart B)
            {
                int a, b;
                if (int.TryParse(A.diffs, out a) && int.TryParse(B.diffs, out b))
                {
                    if (int.Parse(A.diffs) > int.Parse(B.diffs)) return 1;
                    else return -1;
                }
                else return 0;
            });
        }

        public string getTxt(int idx)
        {
            return charts[idx].path;
        }
        public string getDiff(int idx)
        {
            return charts[idx].diffs;
        }
        public string getCharter(int idx)
        {
            return charts[idx].charter;
        }
        public string getID(int idx)
        {
            return charts[idx].hash;
        }
        public string maxDiff()
        {
            return charts[charts.Count - 1].diffs;
        }
        public int diffCount()
        {
            return charts.Count;
        }
    }
    public List<Song> list = new List<Song>();
    public List<Song> listorigin = new List<Song>();

    string DefaultBGPath = Path.Combine(Application.streamingAssetsPath, "Default", "background.jpg");
    string DefaultAudioPath = Path.Combine(Application.streamingAssetsPath, "Default", "virtual.mp3");
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
        if (Files.Length == 0) Files = d.GetFiles("*.osu");

        if (Files.Length == 0) return; //채보파일 없을시 무시

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
        //음악파일 불러오기
        //General 항목이 없을 시 폴더의 mp3, ogg 자동 로드
        Musics = d.GetFiles("*.mp3");
        if (Musics.Length == 0) Musics = d.GetFiles("*.ogg");
        if (Musics.Length == 0) return; //잘못된 폴더는 무시
        AUDIOFILE = Path.Combine(path, dir, Musics[0].Name);
        FileInfo[] bgs;
        bgs = d.GetFiles("*.jpg");    //배경 불러오기
        if (bgs.Length == 0) bgs = d.GetFiles("*.png");
        if (bgs.Length == 0)
            BACKGROUND = DefaultBGPath;
        else
            BACKGROUND = Path.Combine(path, dir, bgs[0].Name);
        //이미지 파일 없을시 default 로드
        FileInfo[] bga;
        bga = d.GetFiles("*.mpg"); // 동영상 불러오기
        if (bga.Length == 0) bga = d.GetFiles("*.mp4");
        if (bga.Length == 0) bga = d.GetFiles("*.flv");
        if (bga.Length != 0) { VIDEOFILE = Path.Combine(path, dir, bga[0].Name); hasvideo = true; }
        //제목,작곡가,난이도,오프셋파싱
        string _title = "", _artist = "", _tags = "";
        float _offset = 0f;
        string line;
        bool isOSUFile = false, isVirtual = false;
        for (int i = 0; i < Files.Length; i++)
        {
            StreamReader rdr = new StreamReader(TXTFILE[i]);
            while ((line = rdr.ReadLine()) != "[Metadata]")
            {
                if (line.Contains("osu file format"))
                {
                    //osu파일 체크
                    isOSUFile = true;
                }
                if (line.Contains("AudioFilename"))
                {
                    string str = line.Split(':')[1].Trim();
                    if (!str.Contains("virtual"))
                    {
                        //virtual = 음악파일 없이 키 사운드로만 재생
                        AUDIOFILE = Path.Combine(path, dir, str);
                    }
                    else
                    {
                        isVirtual = true;
                        AUDIOFILE = DefaultAudioPath;
                    }
                }
                if (line == null) return;
            }

            if (!isOSUFile)
            {
                //커스텀 확장자
                line = rdr.ReadLine(); _title = line;
                line = rdr.ReadLine(); _artist = line;
                line = rdr.ReadLine(); CHARTERS[i] = line;
                line = rdr.ReadLine(); _tags = line;
                line = rdr.ReadLine(); DIFFS[i] = line;
                line = rdr.ReadLine(); _offset = float.Parse(line);
            }
            else
            {
                //osu 확장자
                while ((line = rdr.ReadLine()) != "[TimingPoints]")
                {
                    if (line.Contains("Title:")) _title = line.Split(':')[1];
                    if (line.Contains("Artist:")) _artist = line.Split(':')[1];
                    if (line.Contains("Version:")) DIFFS[i] = line.Split(':')[1];
                    if (line.Contains("Tags:")) _tags = line.Split(':')[1];
                    if (!isVirtual)
                        _offset = 0.015f;
                    else if (line.Contains("normal-hitnormal1002.ogg"))
                    {
                        string[] strs = line.Split(',');
                        AUDIOFILE = Path.Combine(path, dir, "normal-hitnormal1002.ogg");
                        _offset = float.Parse(strs[1]) / 1000f + 0.01f;
                    }
                    if (line == null) return;
                }
                
            }


            _id[i] = CalculateMD5(TXTFILE[i]); //무결성 검사
            rdr.Close();
        }
        Song s;
        s = new Song(dir, _title, _artist, ref DIFFS, _offset, ref _id, ref CHARTERS, ref TXTFILE, AUDIOFILE, BACKGROUND, VIDEOFILE, hasvideo, _tags);
        list.Add(s);
        listorigin.Add(s);
    }
    #region SORT
    public void SortDiff()
    {
        list.Sort(delegate (Song A, Song B)
        {
            int a, b;
            if (!int.TryParse(A.maxDiff(), out a)) return 1;
            if (!int.TryParse(B.maxDiff(), out b)) return 1;
            if (a > b) return 1;
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
    #endregion
    public void searchbyHash(string hash)
    {
        // 해쉬값으로 곡 검색
        for (int i = 0; i < list.Count; i++)
        {
            for(int j = 0; j < list[i].diffCount(); j++)
                if (list[i].getID(j) == hash)
                {
                    scrSetting.decide = i;
                    scrSetting.diffselection = j;
                }
        }
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

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("Title", LoadSceneMode.Single);
    }
}
