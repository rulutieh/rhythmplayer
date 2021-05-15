using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Threading.Tasks;


public class FileSelecter : MonoBehaviour
{

    public GameObject bt, sfx, searchsys, rankpanel, scrollmod;
    MusicHandler player;
    public WWW www, picture;
    FileLoader Loader;
    scrSetting Setting;
    //로드된 변수값들
    public string _name, _artist, _txtpath, _bgpath, _diff, _bgapath, _charter, _hash;
    public float _localoffset, minBPM, maxBPM, medianBPM;
    int _diffcount;
    bool prefer; //선호 난이도 선택 저장
    string lastselect = "", lastselectdiff = ""; //마지막 곡 해시, 마지막 난이도별 해시
    SpriteRenderer rend;
    AudioSource aud, sfxaud;
    public AudioClip audClip;
    public AudioClip[] sfxs;
    public bool searching = false, isThreading;
    //구조체
    class BPMS
    {
        public float time; public double bpms;
        public BPMS(float time, double bpms)
        {
            this.time = time;
            this.bpms = bpms;
        }
    }
    class MedianCac
    {
        public float time; public double bpm; public int key;
        public MedianCac(float time, double bpm)
        {
            this.time = time;
            this.bpm = bpm;
            key = Mathf.FloorToInt((float)bpm);
        }
    }
    List<BPMS> bpmlist = new List<BPMS>();
    List<MedianCac> medianlist = new List<MedianCac>();
    // Start is called before the first frame update
    void Awake()
    {
        sfxaud = sfx.GetComponent<AudioSource>();
        aud = GetComponent<AudioSource>();
        //GetSubDirectories();
        Loader = GameObject.FindWithTag("FileSys").GetComponent<FileLoader>();
        Setting = GameObject.FindWithTag("world").GetComponent<scrSetting>();
        player = GameObject.FindWithTag("world").GetComponent<MusicHandler>();
        prefer = false;
    }
    void Start()
    {
        init();
        aud.volume = sfxaud.volume = aud.volume = scrSetting.Volume; 
    }
    public void init()
    {
        DestroyBTs();
        LoadObjects(); //버튼 로드
        SortMod();      //모드적용
        if (scrSetting.sortsearch != "")
        {
            SortSearch(scrSetting.sortsearch);
        }
        else
        {
            SortMusic();    //정렬적용
        }

        int c = Loader.list.Count;
        if (scrSetting.decide < 0) scrSetting.decide = 0;
        if (scrSetting.decide > c - 1) scrSetting.decide = c - 1;
        Loader.searchbyHash(NowPlaying.HASH);
        LoadFileInfo();
    }
    // Update is called once per frame
    void Update()
    {
        int d = _diffcount - 1;
        if (Input.GetKeyDown(KeyCode.Return)) songDecide();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            goTitle();
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            SetSuffle();
        }//랜덤
        if (Input.GetKeyDown(KeyCode.F7))
        {
            SetSort();

        }//정렬
        if (Input.GetKeyDown(KeyCode.F8))
        {
            SetSpecial();
        }//랜덤

        if (scrSetting.scrollSpeed < 4f) //스크롤 업
                if (Input.GetKeyDown(KeyCode.F4))
                {
                    scrSetting.scrollSpeed += 0.1f;
                    Setting.SaveSelection();
                }
            if (scrSetting.scrollSpeed > 1f) //스크롤 다운
                if (Input.GetKeyDown(KeyCode.F3))
                {
                    scrSetting.scrollSpeed -= 0.1f;
                    Setting.SaveSelection();
                }
        if (!isThreading)
        {
            
            //키보드 컨트롤
            if (Input.GetKeyDown(KeyCode.UpArrow)) { scrSetting.decide--; SongScroll(); }
            if (Input.GetKeyDown(KeyCode.DownArrow)) { scrSetting.decide++; SongScroll(); }
            //마우스휠
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (!rankpanel.GetComponent<RankPanel>().isOver && !scrollmod.GetComponent<ModButton>().isOver)
            {
                if (scroll > 0.001f) { scrSetting.decide--; SongScroll(); }
                if (scroll < -0.001f) { scrSetting.decide++; SongScroll(); }
            }
            if (scrSetting.decide <= Loader.list.Count - 1)
                if (Loader.list[scrSetting.decide].getID(0) == lastselect)
                {
                    if (scrSetting.diffselection > 0)
                        if (Input.GetKeyDown(KeyCode.LeftArrow)) { scrSetting.diffselection--; getDiffinfo(); prefer = false; }
                    if (scrSetting.diffselection < d)
                        if (Input.GetKeyDown(KeyCode.RightArrow)) { scrSetting.diffselection++; getDiffinfo(); prefer = true; }
                }
        }

    }
    void getDiffinfo()
    {
        _diff = Loader.list[scrSetting.decide].getDiff(scrSetting.diffselection);
        _charter = Loader.list[scrSetting.decide].getCharter(scrSetting.diffselection);
        LoadNoteFiles();
        player.PlaySFX(6);
    }
    void LoadObjects()
    {
        for (int i = 0; i < Loader.list.Count; i++)
        {
            string min, max, res = "";
            min = Loader.list[i].getDiff(0);
            max = Loader.list[i].getDiff(Loader.list[i].diffCount() - 1);
            if (string.Compare(min, max) == 0) res = min;
            else
                for (int j = 0; j < Loader.list[i].diffCount(); j++)
                {
                    res += Loader.list[i].getDiff(j);
                    if (j + 1 != Loader.list[i].diffCount()) res += ", ";
                }
            var ist = Instantiate(bt) as GameObject;
            ist.GetComponent<scrButton>().setInfo(i, Loader.list[i].name, Loader.list[i].artist, res);
            ist.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        }

    }
    void LoadNoteFiles()
    {
        _txtpath = Loader.list[scrSetting.decide].getTxt(scrSetting.diffselection);
        NowPlaying.FILE = _txtpath;
        NowPlaying.HASH = Loader.list[scrSetting.decide].getID(scrSetting.diffselection);
        rankpanel.GetComponent<RankPanel>().LoadRanks(NowPlaying.HASH);
        Debug.Log(_txtpath);
        countNotes(_txtpath);
    }
    void LoadFileInfo()
    {
        _diffcount = Loader.list[scrSetting.decide].diffCount();
        if (scrSetting.diffselection > _diffcount - 1)
        {
            if (prefer)
                scrSetting.diffselection = _diffcount - 1;
            else
                scrSetting.diffselection = 0;
        }

        _name = Loader.list[scrSetting.decide].name;
        _artist = Loader.list[scrSetting.decide].artist;
        _diff = Loader.list[scrSetting.decide].getDiff(scrSetting.diffselection);
        _localoffset = Loader.list[scrSetting.decide].localoffset;
        _bgpath = Loader.list[scrSetting.decide].getBG();
        _bgapath = Loader.list[scrSetting.decide].getBGA();
        _charter = Loader.list[scrSetting.decide].getCharter(scrSetting.diffselection);
        NowPlaying.isBGA = Loader.list[scrSetting.decide].hasvideo;



        LoadNoteFiles();
        NowPlaying.MUSICFILE = Loader.list[scrSetting.decide].getAudio();       
        NowPlaying.OFFSET = _localoffset;
        NowPlaying.LEVEL = _diff;
        NowPlaying.BGFILE = _bgpath;
        NowPlaying.BGAFILE = _bgapath;
        NowPlaying.TITLE = _name;
        NowPlaying.ARTIST = _artist;
        //fmod 사운드 릴리즈
        StopCoroutine(CheckLoadedAndPlay());
        if (player.isLoaded() == FMOD.OPENSTATE.PLAYING || player.isLoaded() == FMOD.OPENSTATE.READY)
            player.ReleaseMP3();



        player.LoadSound(NowPlaying.MUSICFILE);

        //선택변경 감지를 위한 마지막 선택곡 저장
        lastselect = Loader.list[scrSetting.decide].getID(0);
        lastselectdiff = Loader.list[scrSetting.decide].getID(scrSetting.diffselection);
        //fmod 사운드 로딩
        StartCoroutine(CheckLoadedAndPlay());


        

        var obj = GameObject.Find("AlbumUI");
        if (obj)
            obj.GetComponent<scrAlbumArt>().LoadAlbumArt();
    }
    public void SetSuffle()
    {
        player.PlaySFX(0);
        scrSetting.modselection++; if (scrSetting.modselection > 2) scrSetting.modselection = 0;
        SortMod();
        Setting.SaveSelection();
    }
    public void SetSpecial()
    {
        player.PlaySFX(0);
        scrSetting.specialselection++; if (scrSetting.specialselection > 1) scrSetting.specialselection = 0;
        SortSpecial();
    }
    public void SetSort()
    {
        player.PlaySFX(0);
        scrSetting.sortselection++;
        SortMusic();
        Setting.SaveSelection();
    }
    void SortMod()
    {
        switch (scrSetting.modselection)
        {
            case 0:
                scrSetting.Mirror = false;
                scrSetting.Random = false;
                break;
            case 1:
                scrSetting.Mirror = true;
                scrSetting.Random = false;
                break;
            case 2:
                scrSetting.Mirror = false;
                scrSetting.Random = true;
                break;
        }
    }
    void SortSpecial()
    {
        switch (scrSetting.specialselection)
        {
            case 0:
                scrSetting.AutoPlay = false;
                break;
            case 1:
                scrSetting.AutoPlay = true;
                break;
        }
    }
    void SortMusic()
    {
        if (scrSetting.sortselection > 2) scrSetting.sortselection = 0;
        switch (scrSetting.sortselection)
        {
            case 0:
                Loader.SortName();
                break;
            case 1:
                Loader.SortArtist();
                break;
            case 2:
                Loader.SortDiff();
                break;
        }
        DestroyBTs();
        LoadObjects();
    }
    IEnumerator CheckLoadedAndPlay()
    {
        while (player.isLoaded() != FMOD.OPENSTATE.READY)
            yield return null;
        player.PlayMP3();
    }
    void goTitle()
    {
        if (player.isLoaded() == FMOD.OPENSTATE.PLAYING || player.isLoaded() == FMOD.OPENSTATE.READY)
            player.ReleaseMP3();
        SceneManager.LoadScene("Title", LoadSceneMode.Single);
        Destroy(gameObject);
    }

    void DestroyBTs()
    {
        var gameObjects = GameObject.FindGameObjectsWithTag("SelBT");
        for (var i = 0; i < gameObjects.Length; i++)
        {
            Destroy(gameObjects[i]);
        }
    }
    // DELEGATE 정렬

    public void SortSearch(string text)
    {
        Loader.list.Clear();
        for (int i = 0; i < Loader.listorigin.Count; i++)
        {
            if (
                Loader.listorigin[i].name.ToUpper().Contains(text) ||
                Loader.listorigin[i].artist.ToUpper().Contains(text) ||
                Loader.listorigin[i].tags.ToUpper().Contains(text)
                ) Loader.list.Add(Loader.listorigin[i]);
        }
        SortMusic();
        DestroyBTs();
        LoadObjects();
    }

    public void SongScroll()
    {
        player.PlaySFX(2);
        //outofrange 방지
        int c = Loader.list.Count;
        if (scrSetting.decide < 0) scrSetting.decide = 0;
        if (scrSetting.decide > c - 1) scrSetting.decide = c - 1;
        //곡 변경시 난이도 배열범위 초과 방지
        //_diffcount = Loader.list[scrSetting.decide].diffCount();
        //scrSetting.diffselection = _diffcount - 1;
    }
    public void songDecide()
    {
        if (scrSetting.decide < Loader.list.Count && Loader.list.Count != 0)
        {
            {
                if (Loader.list[scrSetting.decide].getID(0) != lastselect) //곡 변경후 선택 시 (md5 해쉬 비교) 중복로드 방지
                {
                    _diffcount = 0;
                    Setting.SaveSelection();
                    player.PlaySFX(0);
                    LoadFileInfo();
                }
                else if (!isThreading) //이미 곡 고르고 프리뷰 플레이시
                {
                    player.PlaySFX(4);
                    StopCoroutine(CheckLoadedAndPlay());
                    player.StopMP3();
                    RoomChanger.roomchanger.goRoom("PlayMusic");

                }
            }
        }
    }
    async void countNotes(string filePath) //채보 파일 노트 카운트, BPM 계산
    {
        int lastnote = 0;
        bpmlist.Clear();
        medianlist.Clear();
        double _bpmorigin = 1;
        double _sv = 1;
        double _bpm = 0;
        isThreading = true;
        minBPM = 65526f; maxBPM = 0;
        NowPlaying.NOTECOUNTS = NowPlaying.TIMINGCOUNTS = NowPlaying.LONGNOTECOUNTS = 0;
        FileInfo fileInfo = new FileInfo(filePath);
        if (fileInfo.Exists)
        {    
            string line;

            bool hitObjects = false; //노트 정보 검색
            bool Timings = false;
            StreamReader rdr = new StreamReader(filePath);
            await Task.Run(() => {
                while ((line = rdr.ReadLine()) != null)
                {
                    if (!Timings) Timings = line.Contains("[TimingPoints]");
                    if (!hitObjects) hitObjects = line.Contains("[HitObjects]");
                    if (line.Contains(","))
                    {
                        if (Timings && !hitObjects)
                        {
                            if (line == string.Empty)
                            {
                                Timings = false;
                                
                            }
                            else
                            {
                                
                                NowPlaying.TIMINGCOUNTS++;

                                string[] arr = line.Split(',');

                                string strbpm = arr[1];

                                if (arr[6] == "1")
                                {
                                    _bpmorigin = double.Parse(strbpm);
                                    _bpm = _sv * _bpmorigin; //if bpm
                                }
                                else
                                {
                                    _sv = -double.Parse(strbpm) / 100;
                                    _bpm = _bpmorigin * _sv; //if svchange
                                }


                                _bpm = 1f / _bpm * 1000f * 60f;
                                if (_bpm < 0) continue;
                                if (_bpm < minBPM) minBPM = (float)_bpm; //최소 BPM
                                if (_bpm > maxBPM) maxBPM = (float)_bpm; //최대 BPM

                                bpmlist.Add(new BPMS(float.Parse(arr[0]), _bpm));
                            }
                        }
                        if (hitObjects)
                        {
                            string[] arr = line.Split(',');
                            lastnote = int.Parse(arr[2]);
                            if (int.Parse(arr[3]) != 1 && int.Parse(arr[3]) != 5)
                            {
                                NowPlaying.LONGNOTECOUNTS++;
                            }
                            else
                                NowPlaying.NOTECOUNTS++;
                        }
                        
                    }
                    
                }
            });
            rdr.Close();
        }
        bpmlist.Add(new BPMS(lastnote, _bpm));
        /////////////////////////////////가중치계산/////////////////////////////////////
        for(int i = 0; i < bpmlist.Count; i++)
        {
            float t;
            double b;
            if (i == 0)
            {
                t = 0;
                b = bpmlist[0].bpms;
            }
            else
            {
                t = bpmlist[i - 1].time;
                b = bpmlist[i - 1].bpms;
            }
            bool find = false;
            for (int j = 0; j < medianlist.Count; j++)
            {
                if (Mathf.Abs((float)(b - medianlist[j].bpm)) < 0.1f)
                {
                    find = true;
                    medianlist[j].time += bpmlist[i].time - t;
                }
            }
            if (!find) medianlist.Add(new MedianCac(bpmlist[i].time - t, b));
        }

        for (int i = 0; i < medianlist.Count; i++)
            if (medianlist[i].bpm <= 30f) medianlist.RemoveAt(i); //너무 적은 수치일시 적용방지

        medianlist.Sort(delegate (MedianCac A, MedianCac B)
        {

            if (A.time >= B.time) return -1;
            else return 1;
        }
        );
        medianBPM = (float)medianlist[0].bpm;
        
        NowPlaying.MEDIAN = 1 / (medianBPM / 60000f);
        /////////////////////////////////완    료/////////////////////////////////////
        isThreading = false;
    }   

}
