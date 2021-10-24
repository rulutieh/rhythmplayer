using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using System.Threading.Tasks;


public class FileSelecter : MonoBehaviour
{

    public GameObject bt, sfx, searchsys, rankpanel, scrollmod, DiffSelPanel, icons;
    public Queue<GameObject> b_queue = new Queue<GameObject>();
    List<GameObject> buttons = new List<GameObject>();
    MusicHandler player;
    public WWW www, picture;
    FileLoader Loader;
    SettingPanel setting;
    Manager Setting;
    //로드된 변수값들
    public string _name, _artist, _txtpath, _bgpath, _diff, _bgapath, _charter, _hash;
    public float _localoffset, minBPM, maxBPM, medianBPM;
    public int _diffcount;
    int keycount; //키카운트
    float keydelay;// 키보드난이도선택딜레이
    bool prefer; //선호 난이도 선택 저장
    string lastselect = "", lastselectdiff = ""; //마지막 곡 해시, 마지막 난이도별 해시
    SpriteRenderer rend;
    AudioSource aud, sfxaud;
    public AudioClip audClip;
    public AudioClip[] sfxs;
    public bool searching = false, isThreading, isSelectDiff, diffPlus, diffMinus;
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
        keycount = Manager.keycount;
        sfxaud = sfx.GetComponent<AudioSource>();
        aud = GetComponent<AudioSource>();
        //GetSubDirectories();
        Loader = GameObject.FindWithTag("FileSys").GetComponent<FileLoader>();
        Setting = GameObject.FindWithTag("world").GetComponent<Manager>();
        player = GameObject.FindWithTag("world").GetComponent<MusicHandler>();
        setting = GameObject.FindWithTag("setting").GetComponent<SettingPanel>();
        prefer = false;

        isSelectDiff = false;
    }
    void Start()
    {
        for (int i = 0; i < 30; i++)
        {
            GameObject bts = Instantiate(bt);
            b_queue.Enqueue(bts);
            buttons.Add(bts);
            bts.SetActive(false);

        }
        init();
        aud.volume = sfxaud.volume = aud.volume = Manager.Volume;
        player.ReleaseKeysound();

    }
    public void init()
    {
        DestroyBTs();
        SortMusic();
        SortMod();

        int c = Loader.list.Count;
        if (Manager.decide < 0) Manager.decide = 0;
        if (Manager.decide > c - 1) Manager.decide = c - 1;
        if (!Loader.searchbyHash(NowPlaying.HASH))
        {
            Manager.decide = Mathf.RoundToInt(UnityEngine.Random.Range(0.0f, (float)Loader.list.Count));
        }
        if (Loader.list.Count != 0) LoadFileInfo();


        DestroyBTs();
        LoadObjects();
    }
    // Update is called once per frame
    void Update()
    {
        int d = _diffcount - 1;
        if (!setting.active)
        {
            if (Input.GetKeyDown(KeyCode.Return)) songDecide();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (DiffSelPanel.activeSelf)
                {
                    DiffSelPanel.SetActive(false);
                }
                else
                    goTitle();
            }
            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                Manager.decide += 100;
                SongScroll();
                DestroyBTs();
                LoadObjects();
            }
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                Manager.decide -= 100;
                SongScroll();
                DestroyBTs();
                LoadObjects();
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

            if (Manager.scrollSpeed < 4f) //스크롤 업
                if (Input.GetKeyDown(KeyCode.F4))
                {
                    Manager.scrollSpeed += 0.1f;
                    Setting.SaveSelection();
                }
            if (Manager.scrollSpeed > 1f) //스크롤 다운
                if (Input.GetKeyDown(KeyCode.F3))
                {
                    Manager.scrollSpeed -= 0.1f;
                    Setting.SaveSelection();
                }

            if (!isThreading)
            {

                //키보드 컨트롤
                if (Input.GetKeyDown(KeyCode.UpArrow)) { Manager.decide--; SongScroll(); }
                if (Input.GetKeyDown(KeyCode.DownArrow)) { Manager.decide++; SongScroll(); }
                float translation = Input.GetAxis("Vertical");
                if (keydelay < 0)
                {
                    if (translation > 0.95f) { Manager.decide--; SongScroll(); keydelay = 0.05f; }
                    if (translation < -0.95f) { Manager.decide++; SongScroll(); keydelay = 0.05f; }
                }
                else
                {
                    keydelay -= Time.deltaTime;
                }
                //마우스휠
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                if (!rankpanel.GetComponent<RankPanel>().isOver && !scrollmod.GetComponent<ModButton>().isOver)
                {
                    if (scroll > 0.001f) { Manager.decide--; SongScroll(); }
                    if (scroll < -0.001f) { Manager.decide++; SongScroll(); }
                }
                if (Manager.decide <= Loader.list.Count - 1 && Loader.list.Count != 0)
                    if (DiffChangeEnable())
                    {
                        if (Manager.diffselection == 0)
                            diffMinus = false;
                        if (Manager.diffselection == d)
                            diffPlus = false;
                        if (Manager.diffselection > 0)
                            if (Input.GetKeyDown(KeyCode.LeftArrow) || diffMinus) { Manager.diffselection--; getDiffinfo(); prefer = false; diffMinus = false; }
                        if (Manager.diffselection < d)
                            if (Input.GetKeyDown(KeyCode.RightArrow) || diffPlus) { Manager.diffselection++; getDiffinfo(); prefer = true; diffPlus = false; }
                    }
            }
            int max = 0;
            int min = Int32.MaxValue;
            //var bts = GameObject.FindGameObjectsWithTag("SelBT");
            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i].activeInHierarchy)
                {
                    buttons[i].GetComponent<SongButton>().Pooling();

                    if (max < buttons[i].GetComponent<SongButton>().idx)
                        max = buttons[i].GetComponent<SongButton>().idx;
                    if (min > buttons[i].GetComponent<SongButton>().idx)
                        min = buttons[i].GetComponent<SongButton>().idx;
                }
            }
            if (max < Manager.decide + 10)
                ButtonPooling(Manager.decide + 10);
            if (min > Manager.decide - 10)
                ButtonPooling(Manager.decide - 10);
        }
    }
    public bool DiffChangeEnable()
    {
        return Loader.list[Manager.decide].getID(0, keycount) == lastselect;
    }
    public void getDiffinfo()
    {
        _diff = Loader.list[Manager.decide].getDiff(Manager.diffselection, Manager.keycount);
        _charter = Loader.list[Manager.decide].getCharter(Manager.diffselection, Manager.keycount);
        LoadNoteFiles();
        player.PlaySFX(6);
    }
    void LoadObjects()
    {
        for (int i = Manager.decide - 15; i < Manager.decide + 15; i++)
        {
            if (i > Loader.list.Count - 1 || i < 0)
            {
                continue;
            }
            string min, max, res = "";
            min = Loader.list[i].getDiff(0, Manager.keycount);
            max = Loader.list[i].getDiff(Loader.list[i].diffCount(Manager.keycount) - 1, Manager.keycount);
            if (string.Compare(min, max) == 0) res = min;
            else
                for (int j = 0; j < Loader.list[i].diffCount(Manager.keycount); j++)
                {
                    res += Loader.list[i].getDiff(j, Manager.keycount);
                    if (j + 1 != Loader.list[i].diffCount(Manager.keycount)) res += ", ";
                }

            var ist = b_queue.Dequeue();
            ist.SetActive(true);
            ist.GetComponent<SongButton>().setInfo(i, Loader.list[i].name, Loader.list[i].artist, res);
            ist.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        }
    }
    void LoadIcons()
    {
        var ics = GameObject.FindGameObjectsWithTag("icons");
        for (var i = 0; i < ics.Length; i++)
        {
            Destroy(ics[i]);
        }
        for (int i = 0; i < _diffcount; i++)
        {
            var inst = Instantiate(icons);
            inst.transform.SetParent(GameObject.FindGameObjectWithTag("UICanvas").transform, false);
            inst.GetComponent<SelectIcon>().idx = i;
            inst.GetComponent<SelectIcon>().diffcount = _diffcount;
        }
    }
    void ButtonPooling(int idx)
    {
        if (idx > Loader.list.Count - 1) return;
        if (idx < 0) return;

        string min, max, res = "";
        min = Loader.list[idx].getDiff(0, Manager.keycount);
        max = Loader.list[idx].getDiff(Loader.list[idx].diffCount(Manager.keycount) - 1, Manager.keycount);
        if (string.Compare(min, max) == 0) res = min;
        else
            for (int j = 0; j < Loader.list[idx].diffCount(Manager.keycount); j++)
            {
                res += Loader.list[idx].getDiff(j, Manager.keycount);
                if (j + 1 != Loader.list[idx].diffCount(Manager.keycount)) res += ", ";
            }
        var ist = b_queue.Dequeue();
        ist.SetActive(true);
        ist.GetComponent<SongButton>().setInfo(idx, Loader.list[idx].name, Loader.list[idx].artist, res);
    }
    void DestroyBTs()
    {
        var bts = GameObject.FindGameObjectsWithTag("SelBT");
        for (var i = 0; i < bts.Length; i++)
        {
            b_queue.Enqueue(bts[i]);
            bts[i].SetActive(false);
        }
    }
    public void SongScroll()
    {
        player.PlaySFX(2);
        int c = Loader.list.Count;

        if (Manager.decide < 0) Manager.decide = 0;
        if (Manager.decide > c - 1) Manager.decide = c - 1;

    }
    void LoadNoteFiles()
    {
        int d = Manager.decide;
        int s = Manager.diffselection;
        _txtpath = Loader.list[d].getTxt(s, keycount);
        NowPlaying.FILE = _txtpath;
        NowPlaying.LEVEL = Loader.list[d].getDiff(s, keycount);
        NowPlaying.HASH = Loader.list[d].getID(s, keycount);
        Loader.list[d].NoteCounts(
            s,
            keycount,
            out NowPlaying.NOTECOUNTS,
            out NowPlaying.LONGNOTECOUNTS,
            out NowPlaying.LengthMS
            );
        rankpanel.GetComponent<RankPanel>().LoadRanks(NowPlaying.HASH);
        countNotes(_txtpath);
    }
    void LoadFileInfo()
    {

        SongScroll();

        int d = Manager.decide;
        _diffcount = Loader.list[d].diffCount(keycount);
        try
        {
            

    
        if (Manager.diffselection > _diffcount - 1)
        {
            if (prefer)
                Manager.diffselection = _diffcount - 1;
            else
                Manager.diffselection = 0;
        }

        _name = Loader.list[d].name;
        _artist = Loader.list[d].artist;
        _diff = Loader.list[d].getDiff(Manager.diffselection, keycount);
        _localoffset = Loader.list[d].localoffset;
        _bgpath = Loader.list[d].BGPath;
        _bgapath = Loader.list[d].BGAPath;
        _charter = Loader.list[d].getCharter(Manager.diffselection, keycount);
        NowPlaying.isBGA = string.IsNullOrEmpty(Loader.list[d].BGAPath);
        NowPlaying.isVirtual = Loader.list[d].isvirtual;


        LoadNoteFiles();
        LoadIcons();
        NowPlaying.MUSICFILE = Loader.list[d].AudioPath;
        NowPlaying.OFFSET = _localoffset;
        NowPlaying.BGFILE = _bgpath;
        NowPlaying.BGAFILE = _bgapath;
        NowPlaying.TITLE = _name;
        NowPlaying.ARTIST = _artist;
        NowPlaying.FOLDER = Loader.list[d].directory;
        //fmod 사운드 릴리즈
        StopCoroutine(CheckLoadedAndPlay());
        if (player.isLoaded() == FMOD.OPENSTATE.PLAYING || player.isLoaded() == FMOD.OPENSTATE.READY)
            player.ReleaseMP3();



        player.LoadSound(NowPlaying.MUSICFILE);

        //선택변경 감지를 위한 마지막 선택곡 저장
        lastselect = Loader.list[d].getID(0, keycount);
        lastselectdiff = Loader.list[d].getID(Manager.diffselection, keycount);
        //fmod 사운드 로딩
        StartCoroutine(CheckLoadedAndPlay());
        }
        catch
        {
            RoomChanger.roomchanger.goRoom("Title");
            Loader.ReLoad();
        }
        var obj = GameObject.Find("AlbumUI");
        if (obj)
            obj.GetComponent<AlbumArt>().LoadAlbumArt();
    }
    public void SetSuffle()
    {
        player.PlaySFX(0);
        Manager.modselection++; if (Manager.modselection > 2) Manager.modselection = 0;
        SortMod();
        Setting.SaveSelection();
    }
    public void SetSpecial()
    {
        player.PlaySFX(0);
        Manager.specialselection++; if (Manager.specialselection > 1) Manager.specialselection = 0;
        SortSpecial();
    }
    public void SetSort()
    {
        player.PlaySFX(0);
        Manager.sortselection++;
        SortMusic();
        Setting.SaveSelection();
    }
    void SortMod()
    {
        switch (Manager.modselection)
        {
            case 0:
                Manager.Mirror = false;
                Manager.Random = false;
                break;
            case 1:
                Manager.Mirror = true;
                Manager.Random = false;
                break;
            case 2:
                Manager.Mirror = false;
                Manager.Random = true;
                break;
        }
    }
    void SortSpecial()
    {
        switch (Manager.specialselection)
        {
            case 0:
                Manager.AutoPlay = false;
                break;
            case 1:
                Manager.AutoPlay = true;
                break;
        }
    }
    void SortMusic()
    {
        if (Manager.sortselection > 4) Manager.sortselection = 0;
        switch (Manager.sortselection)
        {
            case 0:
                Loader.SortName();
                break;
            case 1:
                Loader.SortArtist();
                break;
            case 2:
                Loader.SortDiff(0);
                break;
            case 3:
                Loader.SortDiff(1);
                break;
            case 4:
                Loader.SortNPS();
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
    public void goTitle()
    {
        if (!setting.escdisable)
        {
            if (player.isLoaded() == FMOD.OPENSTATE.PLAYING || player.isLoaded() == FMOD.OPENSTATE.READY)
                player.ReleaseMP3();
            Loader.list.Clear();
            Loader.listkeysort.Clear();
            RoomChanger.roomchanger.goRoom("Title");
        }
    }

    // DELEGATE 정렬
    public void SortSearch(string text)
    {
        Loader.list.Clear();
        for (int i = 0; i < Loader.listkeysort.Count; i++)
        {
            if (
                Loader.listkeysort[i].name.ToUpper().Contains(text) ||
                Loader.listkeysort[i].artist.ToUpper().Contains(text) ||
                Loader.listkeysort[i].tags.ToUpper().Contains(text)
                ) Loader.list.Add(Loader.listkeysort[i]);
        }
        SortMusic();
        SongScroll();
        DestroyBTs();
        LoadObjects();
    }
    public void songDecide()
    {
        if (Manager.decide < Loader.list.Count && Loader.list.Count != 0)
        {
            {
                if (Loader.list[Manager.decide].getID(0, Manager.keycount) != lastselect) //곡 변경후 선택 시 (md5 해쉬 비교) 중복로드 방지
                {

                    _diffcount = 0;
                    Setting.SaveSelection();
                    player.PlaySFX(0);
                    LoadFileInfo();

                }
                else if (!isThreading) //이미 곡 고르고 프리뷰 플레이시
                {
                    if (_diffcount > 1)
                    {
                        if (DiffSelPanel.activeSelf)
                        {
                            player.PlaySFX(4);
                            StopCoroutine(CheckLoadedAndPlay());
                            player.StopMP3();
                            RoomChanger.roomchanger.goRoom("PlayMusic");
                        }
                        else
                            DiffSelPanel.SetActive(true);
                    }
                    else
                    {
                        player.PlaySFX(4);
                        StopCoroutine(CheckLoadedAndPlay());
                        player.StopMP3();
                        RoomChanger.roomchanger.goRoom("PlayMusic");
                    }
                }
            }
        }
    }
    async void countNotes(string filePath) //채보 파일 노트 카운트, BPM 계산
    {
        bpmlist.Clear();
        medianlist.Clear();
        double _bpmorigin = 1;
        double _sv = 1;
        double _bpm = 0;
        isThreading = true;
        minBPM = 65526f; maxBPM = 0;
        NowPlaying.TIMINGCOUNTS = 0;
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
                            else if (!line.Contains(":"))
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
                    }
                }
            });
            rdr.Close();
        }
        bpmlist.Add(new BPMS(NowPlaying.LengthMS, _bpm));
        /////////////////////////////////가중치계산/////////////////////////////////////
        for (int i = 0; i < bpmlist.Count; i++)
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
            if (A.time == B.time) return 0;
            if (A.time > B.time) return -1;
            else return 1;
        }
        );
        medianBPM = (float)medianlist[0].bpm;

        NowPlaying.MEDIAN = 1 / (medianBPM / 60000f);
        /////////////////////////////////완    료/////////////////////////////////////
        isThreading = false;
    }

}