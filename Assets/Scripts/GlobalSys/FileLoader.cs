using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using System.Globalization;

public class FileLoader : MonoBehaviour
{
    FileSystemWatcher FolderWatcher;
    [Serializable]
    public class Chart
    {
        public string hash, charter, path, diffs;
        public Chart(string hash, string charter, string path, string diffs)
        {
            this.hash = hash; this.charter = charter; this.path = path; this.diffs = diffs;
        }
    }
    [Serializable]
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
        public bool isvirtual { get; set; }
        public List<Chart>[] charts = new List<Chart>[9]; //키모드 갯수 분할 1키~8키
        public Song()
        {
            for (int i = 0; i < charts.Length; i++)
            {
                charts[i] = new List<Chart>();
            }
        }
        public void AddCharts(int keyCounts, string id, string noter, string txtpath, string diffs)
        {
            charts[keyCounts].Add(new Chart(id, noter, txtpath, diffs));
        }
        public void SortDiff()
        {
            for (int i = 0; i < charts.Length; i++)
            {
                charts[i].Sort(delegate (Chart A, Chart B)
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
        }
        public string getTxt(int idx, int keys)
        {
            return charts[keys][idx].path;
        }
        public string getDiff(int idx, int keys)
        {
            return charts[keys][idx].diffs;
        }
        public string getCharter(int idx, int keys)
        {
            return charts[keys][idx].charter;
        }
        public string getID(int idx, int keys)
        {
            return charts[keys][idx].hash;
        }
        public string maxDiff(int keys)
        {
            return charts[keys][charts[keys].Count - 1].diffs;
        }
        public int diffCount(int keys)
        {
            return charts[keys].Count;
        }
        public bool CheckKeymodeExists(int keys)
        {
            if (charts[keys].Count != 0)
                return true;
            else
                return false;
        }


    }

    [SerializeField]
    public List<Song> list = new List<Song>();
    [SerializeField]
    public List<Song> listkeysort = new List<Song>();
    [SerializeField]
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
        try
        {
            GlobalSettings.FolderPath = PlayerPrefs.GetString("FOLDER", GlobalSettings.FolderPath);
            StartLoad(); //첫로드
        }
        catch (Exception ie)
        {
            Debug.Log(ie);
        }
        StartCoroutine(loadComplete());
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            ReLoad();
        }
    }
    public void ReLoad()
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
    public void StartLoad() //파일 로드 시작
    {
        int idx = 0;
        string root = GlobalSettings.FolderPath;
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
        Song newSong = new Song();
        newSong.directory = dir;
        string AUDIOFILE, BACKGROUND, VIDEOFILE = "";
        bool isvirtual = false;

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
        if (Musics.Length == 0) Musics = d.GetFiles("*.wav");
        if (Musics.Length == 0) return; //잘못된 폴더는 무시
        AUDIOFILE = Path.Combine(path, dir, Musics[0].Name);
        newSong.AudioPath = AUDIOFILE;

        FileInfo[] bgs;
        bgs = d.GetFiles("*.jpg");    //배경 불러오기
        if (bgs.Length == 0) bgs = d.GetFiles("*.png");
        if (bgs.Length == 0)
            BACKGROUND = DefaultBGPath;   //이미지 파일 없을시 default 로드
        else
            BACKGROUND = Path.Combine(path, dir, bgs[0].Name);
        newSong.BGPath = BACKGROUND;

        FileInfo[] bga;
        bga = d.GetFiles("*.mpg"); // 동영상 불러오기
        if (bga.Length == 0) bga = d.GetFiles("*.mp4");
        if (bga.Length == 0) bga = d.GetFiles("*.flv");
        if (bga.Length != 0) {
            VIDEOFILE = Path.Combine(path, dir, bga[0].Name);
            newSong.hasvideo = true;
            newSong.BGAPath = VIDEOFILE;
        }
        else
            newSong.hasvideo = false;
        

        //제목,작곡가,난이도,오프셋파싱
        string line;
        bool[] correctfile = new bool[Files.Length];
        try
        {
            for (int i = 0; i < Files.Length; i++)
            {
                int keycount = 7;

                correctfile[i] = true;

                StreamReader rdr = new StreamReader(TXTFILE[i]);
                while ((line = rdr.ReadLine()) != "[Metadata]")
                {

                    if (line.Contains("AudioFilename"))
                    {
                        string str = line.Split(':')[1].Trim();
                        if (!str.Contains("virtual") && !str.Contains("nothing"))
                        {
                            //virtual = 음악파일 없이 키 사운드로만 재생
                            newSong.AudioPath = Path.Combine(path, dir, str);
                        }
                        else
                        {
                            isvirtual = true;
                            if (File.Exists(Path.Combine(path, dir, "preview.mp3")))
                            {
                                newSong.AudioPath = Path.Combine(path, dir, "preview.mp3");
                            }
                            else
                                newSong.AudioPath = DefaultAudioPath;
                        }
                    }
                    if (line.Contains("Mode"))
                    {
                        if (!line.EndsWith("3"))
                        {
                            correctfile[i] = false;
                            goto WRONGFILE;
                        }
                            
                    }
                    if (line.Contains("AudioOffset"))
                    {
                        string str = line.Split(':')[1].Trim();
                        newSong.localoffset = float.Parse(str);
                    }
                    if (line == null) return;
                }
                string _noter = "", _diff = "";

                //osu 확장자
                while ((line = rdr.ReadLine()) != "[TimingPoints]")
                {
                    if (line.Contains("Title:")) newSong.name = line.Remove(0, 6);
                    if (line.Contains("Artist:")) newSong.artist = line.Remove(0, 7);
                    if (line.Contains("Tags:")) newSong.tags = line.Remove(0, 4);
                    if (line.Contains("Version:")) _diff = line.Remove(0, 8);
                    if (line.Contains("Creator:")) _noter = line.Remove(0, 8);
                    if (line.Contains("CircleSize:"))
                        keycount = int.Parse(line.Split(':')[1]);
                    if (line == null) return;
                }
                newSong.isvirtual = isvirtual;
                newSong.AddCharts(
                    keycount,
                    CryptoManager.CalculateMD5(TXTFILE[i]),
                    _noter,
                    TXTFILE[i],
                    _diff
                    );

                WRONGFILE:
                rdr.Close();
            }
        }
        catch (Exception ie)
        {
            Debug.Log(ie);
            return;
        }
        //올바른 파일이 없으면 무시
        for (int i = 0; i < correctfile.Length; i++)
        {
            Debug.Log("add");
            if (correctfile[i])
            {
                newSong.SortDiff();
                listorigin.Add(newSong);
                return;
            }
        }
    }
    #region SORT
    public void SortByKeycounts(int keycount)
    {
        for(int i = 0; i < listorigin.Count; i++)
        {
            if (listorigin[i].CheckKeymodeExists(keycount))
            {
                listkeysort.Add(listorigin[i]);
                list.Add(listorigin[i]);
            }
        }
    }
    public void SortDiff()
    {
        list.Sort(delegate (Song A, Song B)
        {
            int a, b;
            if (!int.TryParse(A.maxDiff(GlobalSettings.keycount), out a)) return 1;
            if (!int.TryParse(B.maxDiff(GlobalSettings.keycount), out b)) return 1;
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
    public bool searchbyHash(string hash)
    {
        // 해쉬값으로 곡 검색
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < list[i].diffCount(GlobalSettings.keycount); j++)
                if (list[i].getID(j, GlobalSettings.keycount) == hash)
                {
                    GlobalSettings.decide = i;
                    GlobalSettings.diffselection = j;
                    return true;
                }
        }
        return false;
    }
    private void Run_Watcher()
    {

        FolderWatcher = new FileSystemWatcher();

        FolderWatcher.Filter = "*.*";

        FolderWatcher.Path = GlobalSettings.FolderPath;

        FolderWatcher.IncludeSubdirectories = true;

        FolderWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;

        FolderWatcher.Changed += new FileSystemEventHandler(OnChanged);

        FolderWatcher.Created += new FileSystemEventHandler(OnChanged);

        FolderWatcher.Deleted += new FileSystemEventHandler(OnChanged);

        FolderWatcher.Renamed += new RenamedEventHandler(OnChanged);

        FolderWatcher.EnableRaisingEvents = true;

    }
    private void OnChanged(object source, FileSystemEventArgs e)
    {
        //  Show that a file has been created, changed, or deleted.
        WatcherChangeTypes wct = e.ChangeType;
        Console.WriteLine("File {0} {1}", e.FullPath, wct.ToString());
    }

    IEnumerator loadComplete()
    {

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("Title", LoadSceneMode.Single);
    }
}
