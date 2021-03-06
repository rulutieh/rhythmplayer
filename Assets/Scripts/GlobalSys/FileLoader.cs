using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using System.Globalization;
using System.Threading.Tasks;
using TMPro;
using Newtonsoft.Json;

public class FileLoader : MonoBehaviour
{
    public bool threading = false;
    [SerializeField]
    TextMeshProUGUI tt;
    [SerializeField]
    GameObject LoadCircle;

    string t;

    FileSystemWatcher FolderWatcher;

    [Serializable]
    public class Chart
    {
        public string hash, charter, path, diffs;
        public int notes, longnotes, length;
        public Chart(string hash, string charter, string path, string diffs, int notes, int longnotes, int length)
        {
            this.hash = hash; this.charter = charter; this.path = path;
            this.diffs = diffs; this.notes = notes; this.longnotes = longnotes;
            this.length = length;
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
        public void AddCharts(int keyCounts, Chart c)
        {
            charts[keyCounts].Add(c);
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
                        if (a == b) return 0;
                        if (a > b) return 1;
                        else return -1;
                    }
                    else
                    {
                        int an = A.notes + A.longnotes * 2;
                        int bn = B.notes + B.longnotes * 2;
                        if (an == bn) return 0;
                        if (an > bn) return 1;
                        else return -1;
                    }
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
        public void NoteCounts(int idx, int keys, out int note, out int ln, out int time)
        {
            note = charts[keys][idx].notes;
            ln = charts[keys][idx].longnotes;
            time = charts[keys][idx].length;

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
        StartCoroutine(FirstLoad());
        //Run_Watcher();
    }
    IEnumerator FirstLoad()
    {
        yield return new WaitForSeconds(0.4f);
        Manager.FolderPath = PlayerPrefs.GetString("PATH", Manager.FolderPath);


        if (!Directory.Exists(Manager.FolderPath))
            Manager.FolderPath = Application.dataPath;


#if (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
    Manager.FolderPath = Path.Combine(Application.streamingAssetsPath, "Songs");
#endif

        if (!File.Exists(Path.Combine(Application.dataPath, "songlist.db")))
            ReLoad(); //첫 로딩 (DB파일 없을 때)
        else
        {
            var json = File.ReadAllText(Path.Combine(Application.dataPath, "songlist.db"));
            listorigin = JsonConvert.DeserializeObject<List<Song>>(json);
            SceneManager.LoadScene("Title", LoadSceneMode.Single);
            LoadCircle.SetActive(false);
        }   //DB파일 있을 시 불러오기
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {

            ReLoad();
        }
        tt.text = t;
    }

    public void ReLoad()
    {
        if (!threading)
        {
            list.Clear();
            listorigin.Clear();
            listkeysort.Clear();
            StartLoad();
            var s = GameObject.FindWithTag("SelSys");
            if (s)
            {
                s.GetComponent<FileSelecter>().goTitle();
            }
        }
    }
    async void StartLoad() //파일 로드 시작
    {
        LoadCircle.SetActive(true);
        threading = true;
        string root = Manager.FolderPath;
        string[] subdirectoryEntries = Directory.GetDirectories(root);
        //폴더수만큼구조체배열할당
        var tasks = new List<Task>();
        foreach (string subdirectory in subdirectoryEntries)
        {
            tasks.Add(LoadDirsAsync(root, subdirectory));
        }
        
        await Task.WhenAll(tasks);
        string json = JsonConvert.SerializeObject(listorigin, Formatting.Indented);
        File.WriteAllText(Path.Combine(Application.dataPath, "songlist.db"), json);
        SceneManager.LoadScene("Title", LoadSceneMode.Single);
        t = "";
        threading = false;
        LoadCircle.SetActive(false);
    }
    Task LoadDirsAsync(string path, string dir)
    {
        
        return Task.Run(() => LoadSubDirs(path, dir));
    }
    void LoadSubDirs(string path, string dir) //폴더별 파일 로드
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
        newSong.BGPath = NullStringCheck(BACKGROUND);
        FileInfo[] bga;
        bga = d.GetFiles("*.mpg"); // 동영상 불러오기
        if (bga.Length == 0) bga = d.GetFiles("*.mp4");
        if (bga.Length == 0) bga = d.GetFiles("*.flv");
        if (bga.Length != 0) {
            VIDEOFILE = Path.Combine(path, dir, bga[0].Name);
            newSong.BGAPath = VIDEOFILE;
        }
        //제목,작곡가,난이도,오프셋파싱
        StreamReader rdr = null;
        string line;
        try
        {
            for (int i = 0; i < Files.Length; i++)
            {
                bool wrongfile = false;
                int keycount = 7;

                rdr = new StreamReader(TXTFILE[i]);
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
                            wrongfile = true;
                        }
                            
                    }
                    if (line.Contains("AudioOffset"))
                    {
                        string str = line.Split(':')[1].Trim();
                        newSong.localoffset = float.Parse(str);
                    }
                    if (line == null) continue;
                }
                string _noter = "", _diff = "";

                //osu 확장자
                while ((line = rdr.ReadLine()) != "[TimingPoints]")
                {
                    if (line.Contains("Title:"))
                    {
                        string s = line.Remove(0, 6);
                        newSong.name = s;
                        t = s;
                    }
                    if (line.Contains("Artist:")) newSong.artist = line.Remove(0, 7);
                    if (line.Contains("Tags:")) newSong.tags = NullStringCheck(line.Remove(0, 4));
                    if (line.Contains("Version:"))
                    {
                        _diff = NullStringCheck(line.Remove(0, 8));
                        if (_diff.Contains("easy lvl")) _diff =  _diff.Remove(0, 14);
                        else if (_diff.Contains("hard lvl")) _diff = _diff.Remove(0, 14);
                        else if (_diff.Contains("normal lvl")) _diff =_diff.Remove(0, 16);
                    }
                    if (line.Contains("Creator:")) _noter = NullStringCheck(line.Remove(0, 8));
                    if (line.Contains("CircleSize:"))
                        keycount = int.Parse(line.Split(':')[1]);
                    if (line == null) continue;
                }
                while ((line = rdr.ReadLine()) != "[HitObjects]") { }
                int notecounts = 0;
                int lastnote = 0;
                int longnotecounts = 0;
                while ((line = rdr.ReadLine()) != null)
                {
                    string[] arr = line.Split(',');
                    lastnote = int.Parse(arr[2]);
                    if (int.Parse(arr[3]) != 1 && int.Parse(arr[3]) != 5)
                    {
                        longnotecounts++;
                    }
                    else
                        notecounts++;
                }
                if (!wrongfile)
                {
                    Chart c = new Chart(
                        CryptoManager.CalculateMD5(TXTFILE[i]),
                        _noter,
                        TXTFILE[i],
                        _diff,
                        notecounts,
                        longnotecounts,
                        lastnote
                        );
                    newSong.isvirtual = isvirtual;
                    newSong.AddCharts(
                        keycount,
                        c
                        );
                }
                rdr.Close();
            }
            newSong.SortDiff();
            if (!(newSong is null))
                listorigin.Add(newSong);
        }
        catch (Exception ie)
        {
            Debug.Log(ie);
            if (rdr != null)
                rdr.Dispose();
            return;
        }
    }
    string NullStringCheck(string s)
    {
        if (s != null) return s;
        else return "";
    }
#region SORT
    public void SortByKeycounts()
    {
        try
        {
            for (int i = 0; i < listorigin.Count; i++)
            {
                if (listorigin[i].CheckKeymodeExists(Manager.keycount))
                {

                    listkeysort.Add(listorigin[i]);
                    list.Add(listorigin[i]);
                }
            }
        }
        catch
        {
            ReLoad();
        }
    }
    public void SortDiff(int mode)
    {
        list.Sort(delegate (Song A, Song B)
        {
            int a, b;
            if (mode == 0)
            {
                int.TryParse(A.getDiff(0, Manager.keycount), out a);
                int.TryParse(B.getDiff(0, Manager.keycount), out b);
            }
            else
            {
                int.TryParse(A.maxDiff(Manager.keycount), out a);
                int.TryParse(B.maxDiff(Manager.keycount), out b);
            }

            if (a == b) return 0;
            if (a > b) return 1;
            else return -1;
        });
    }
    public void SortNPS()
    {
        list.Sort(delegate (Song A, Song B)
        {
            A.NoteCounts(
            A.diffCount(Manager.keycount) - 1,
            Manager.keycount,
            out int note,
            out int ln,
            out int time
            );
            float anps = (note + ln * 1.5f) / time;
            B.NoteCounts(
            B.diffCount(Manager.keycount) - 1,
            Manager.keycount,
            out note,
            out ln,
            out time
            );
            float bnps = (note + ln * 1.5f) / time;
            if (anps > bnps) return 1;
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
            for (int j = 0; j < list[i].diffCount(Manager.keycount); j++)
                if (list[i].getID(j, Manager.keycount) == hash)
                {
                    Manager.decide = i;
                    Manager.diffselection = j;
                    return true;
                }
        }
        return false;
    }
    private void Run_Watcher()
    {

        FolderWatcher = new FileSystemWatcher();

        FolderWatcher.Filter = "*.*";

        FolderWatcher.Path = Manager.FolderPath;

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

        ReLoad();
    }

    IEnumerator loadComplete()
    {
        yield return new WaitUntil(() => !threading);

        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Title", LoadSceneMode.Single);
    }
}
