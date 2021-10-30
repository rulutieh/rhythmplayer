using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.IO;
using System.Linq;

public class NotePlayer : MonoBehaviour
{
    #region Variables
    float startTime;
    public float offset, loadprogress;
    public static float judgeoffset = -3.15f;
    public static bool isPlaying, isVideoLoaded, resultload, isLoaded;
    public static float PlaybackChanged, Playback;
    public static float bpm = 0, startbpm = 0;
    public static double multiply;
    float p, _TIME, _RTIME, barTIME;
    int sampleIDX, noteIDX, rnoteIDX, barIDX, timeIDX, preLoad, noteidx = 0, timingidx = 0;
    public int progress = 0;
    int[] keys7 = { 0, 1, 2, 3, 4, 5, 6 };
    int[] keys4 = { 0, 1, 2, 3 };
    public GameObject NoteObj, ColObj, endln, result, gameover, initsetting, judgeobj, barobj, LoadCircle;
    bool svEnd, noteEnd, rnoteEnd, sampleEnd, playfieldon, musicOn;
    int TimingCount, NoteCount, LastNoteTiming;
    public static int NoteCountLongnote;

    ScoreManager smanager;
    RankSystem RankSys;
    MusicHandler player;
    GameObject w;
    NowPlaying select;

    #endregion

    #region Notes, Sound, Queue, BPM Collections
    public Queue<GameObject> n_queue = new Queue<GameObject>();
    public Queue<GameObject> b_queue = new Queue<GameObject>();
    public Queue<GameObject> start_queue = new Queue<GameObject>();
    public Queue<GameObject> end_queue = new Queue<GameObject>();
    [Serializable]
    struct Timings
    {
        public float TIME;
        public double BPM;
        public Timings(float TIME, double BPM)
        {
            this.TIME = TIME;
            this.BPM = BPM;
        }
    }
    //노트
    [Serializable]
    struct Notes
    {
        public int COLUMN;
        public int TIME;
        public int KeySoundINDEX;
        public bool ISLN;
        public int LNLENGTH;
        public Notes(int COLUMN, int TIME, bool ISLN, int LNLENGTH, int KEYSOUND)
        {
            this.COLUMN = COLUMN;
            this.TIME = TIME;
            this.ISLN = ISLN;
            this.LNLENGTH = LNLENGTH;
            KeySoundINDEX = KEYSOUND;
        }

    }
    //샘플
    [Serializable]
    struct Samples
    {
        public int TIME;
        public int idx;
        public Samples(int time, int idx)
        {
            TIME = time;
            this.idx = idx;
        }
    }
    //노트 리스트
    [SerializeField]
    Notes[] NoteList;
    //타이밍 리스트
    [SerializeField]
    Timings[] TimeList;
    //마디선 리스트
    [SerializeField]
    List<int> barlist = new List<int>();
    [SerializeField]
    string[] keysounds;
    [SerializeField]
    List<string> LoadedKeySounds = new List<string>();
    [SerializeField]
    List<Samples> SampleList = new List<Samples>();
    #endregion

    #region Unity Callbacks
    void Awake()
    {
        barIDX = 0; //bar 풀링 idx
        noteIDX = 0; //노트 풀링idx
        timeIDX = 0; //타이밍 풀링 idx
        rnoteIDX = 0; //노트콜리더 풀링idx
        sampleIDX = 0;
        resultload = false;
        isLoaded = false;
        musicOn = false;
        smanager = GetComponent<ScoreManager>();
        if (Manager.keycount == 7)
        {
            if (Manager.Random) //노트 랜덤배치
                Random(keys7);
            for (int i = 0; i < keys7.Length; i++)
            {
                if (Manager.Mirror) keys7[i] = 6 - i;//노트 미러배치
            }
        }
        else
        {
            if (Manager.Random) //노트 랜덤배치
                Random(keys4);
            for (int i = 0; i < keys4.Length; i++)
            {
                if (Manager.Mirror) keys4[i] = 3 - i;//노트 미러배치
            }
        }
    }
    void Start()
    {
        LoadCircle.SetActive(true);
        Playback = 0;
        //랭킹 등록 시스템
        w = GameObject.FindWithTag("world");
        RankSys = w.GetComponent<RankSystem>();
        player = w.GetComponent<MusicHandler>(); //Fmod sound system
        player.ReleaseKeysound();
        preLoad = (int)(1500f / Manager.scrollSpeed); //노트 풀링 오프셋

        playfieldon = true;
        isPlaying = false;
        NoteCountLongnote = 0;

        //고른 음악정보 가져오기

        string filePath = NowPlaying.PLAY.FILE;
        offset = NowPlaying.PLAY.OFFSET;
        ReadFile(filePath); //파일 읽기 시작

        NoteList = new Notes[NowPlaying.PLAY.NOTECOUNTS + NowPlaying.PLAY.LONGNOTECOUNTS];
        TimeList = new Timings[NowPlaying.PLAY.TIMINGCOUNTS];

        for (int i = 0; i < 200; i++) // 미리 200개의 객체 미리 생성
        {
            SetPooling(0);
            SetPooling(1);
            SetPooling(2);
        }
        for (int i = 0; i < 20; i++) 
        {
            BarPooling();
        }
    }
    void SetPooling(int index)
    {
        GameObject cobj;
        switch (index)
        {
            case 0:
                cobj = Instantiate(ColObj, new Vector2(0, 1000f), Quaternion.identity);
                n_queue.Enqueue(cobj);
                cobj.SetActive(false);
                break;
            case 1:
                cobj = Instantiate(NoteObj, new Vector2(0, 1000f), Quaternion.identity);
                start_queue.Enqueue(cobj);
                cobj.SetActive(false);
                break;
            case 2:
                cobj = Instantiate(endln, new Vector2(0, 1000f), Quaternion.identity);
                end_queue.Enqueue(cobj);
                cobj.SetActive(false);
                break;
        }
    }
    void BarPooling()
    {
        GameObject b = Instantiate(barobj, new Vector2(0, 1000f), Quaternion.identity);
        b_queue.Enqueue(b);
        b.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {

        judgeoffset = -3.15f + Manager.stageYPOS;

        //노트, 타이밍 생성
        if (isLoaded)
        {
            if (!musicOn)
            {
                if (Playback > -100f + offset + Manager.GlobalOffset * 1000f)
                {
                    AudioStart();
                    musicOn = true;
                }
            }
            multiply = 3f / 410f * Manager.scrollSpeed;
            //if (!GlobalSettings.isFixedScroll)
            //{
            //    multiply = 3f / 410f * GlobalSettings.scrollSpeed;
            //}
            //else
            //    multiply = 3f / NowPlaying.PLAY.MEDIAN * GlobalSettings.scrollSpeed;
            //p += Time.deltaTime * 1000f; //use deltatime
            //Playback = p;
            Playback = Time.timeSinceLevelLoad * 1000f - startTime + p; //use scenemanagement
            PlaybackChanged = GetNoteTime(Playback); // reamtime에 변속 계산 < 계산량 증가

            if (noteEnd) //게임 종료 시
            {
                if (!resultload)
                {
                    int __t = LastNoteTiming + 4000;
                    if (__t < Playback - 3700f && playfieldon)
                    {
                        playfieldon = false;
                        var gameObjects = GameObject.FindGameObjectsWithTag("player");
                        for (var i = 0; i < gameObjects.Length; i++)
                        {
                            gameObjects[i].GetComponent<ColumnSetting>().onResult();
                        }

                    }

                    if (__t < Playback)
                    {
                        //결과창 로드
                        if (!ScoreManager.isFailed && !Manager.AutoPlay) //저장
                            smanager.SaveScores();
                        resultload = true;
                        StartCoroutine(ShowResult());
                        w.GetComponent<Manager>().SaveSettings();
                    }
                }
            }

            if (SampleList.Count != 0)
                SampleSystem();
        }
    }
    #endregion

    #region InitSettings
    void AudioStart() //오프셋 Invoke로 실행
    {
        if (!NowPlaying.PLAY.isVirtual)
            player.PlayMP3();
        isPlaying = true;
    }
    IEnumerator SongInit() //async 종료 후 해당 데이터로 init
    {
        //마디선 추가
        GetBarTime();


        //변속곡 전용 마디선 추가
        if (TimeList.Length >= 70)
            for (int i = 0; i < 130; i++)
            {
                BarPooling();
            }

        //비디오 대기
        yield return new WaitUntil(() => isVideoLoaded);
        yield return new WaitForSeconds(1.2f);
        LoadCircle.GetComponent<LoadIcon>().Fade();
        yield return new WaitUntil(() => !Input.GetKey(KeyCode.LeftControl));
        startbpm = bpm = (float)TimeList[0].BPM; //1비트당 소모되는 ms

        //스트리밍 시작
        StartCoroutine(BpmChange());
        StartCoroutine(NoteSystem());
        StartCoroutine(InputSystem());
        StartCoroutine(mBarSystem());

        PlaybackChanged = Playback = -2000f;
        p = Playback;
        startTime = Time.timeSinceLevelLoad * 1000f;
        isLoaded = true;

        yield return new WaitForSeconds(1f);
        initsetting.GetComponent<playerinit>().hideChilds(); //스테이지 설정 끄기

    }
    void Random<T>(T[] array) //랜덤 옵션 셔플
    {
        int random1;
        int random2;

        T tmp;

        for (int index = 0; index < array.Length; ++index)
        {
            random1 = UnityEngine.Random.Range(0, array.Length);
            random2 = UnityEngine.Random.Range(0, array.Length);

            tmp = array[random1];
            array[random1] = array[random2];
            array[random2] = tmp;
        }
    }

    #endregion

    #region Time Caculate Algorhythm
    void GetBarTime()
    {
        for (int i = 0; i < TimeList.Length; i++)
        {
            float _t;
            if (i + 1 == TimeList.Length)
            {
                _t = NoteList[NoteList.Length - 1].TIME;
            }
            else
            {
                _t = TimeList[i + 1].TIME;
            }

            int a = Mathf.FloorToInt((float)((_t - TimeList[i].TIME) / (4 * TimeList[i].BPM)));
            barlist.Add((int)TimeList[i].TIME);

            for (int j = 1; j < a; j++)
            {
                barlist.Add(Mathf.RoundToInt((float)(TimeList[i].TIME + j * 4 * TimeList[i].BPM)));
            }
        }
    }
    float GetNoteTime(double TIME) //BPM에 따른 노트 위치 계산
    {
        double newTIME = TIME;
        double prevBPM = 1;
        for (int i = 0; i < TimeList.Length; i++)
        {
            double _time = TimeList[i].TIME;
            double _listbpm = TimeList[i].BPM;
            double _bpm;
            if (_time > TIME) break; //변속할 타이밍이 아니면 빠져나오기
            _bpm = (NowPlaying.PLAY.MEDIAN / _listbpm);
            newTIME += (_bpm - prevBPM) * (TIME - _time); //거리계산
            prevBPM = _bpm; //이전bpm값
        }
        return (float)newTIME; //최종값 리턴
    }
    #endregion

    #region Note Streaming Systems
    void SampleSystem()
    {
        int sampleTime = SampleList[sampleIDX].TIME;
        if (sampleTime <= Playback && !sampleEnd)
        {
            int temp = sampleIDX;
            for (int i = 0; i < 7; i++)
            {
                player.PlaySample(SampleList[sampleIDX].idx);

                if (sampleIDX < SampleList.Count - 1)
                    sampleIDX++;
                else
                {
                    sampleEnd = true;
                    break;
                }
                if (SampleList[temp].TIME != SampleList[sampleIDX].TIME) break;
            }
        }
    }
    IEnumerator BpmChange()
    {
        float _bpmchange = TimeList[timeIDX].TIME;
        yield return new WaitUntil(() => _bpmchange <= Playback && !svEnd);

        bpm = (float)TimeList[timeIDX].BPM;
        if (timeIDX < TimeList.Length - 1)
        {
            timeIDX++;
        }
        else svEnd = true;

        StartCoroutine(BpmChange());
    }
    IEnumerator NoteSystem()
    {
        //노트 렌더링 오브젝트 출력 (변속타임)
        _TIME = GetNoteTime(NoteList[noteIDX].TIME);
        yield return new WaitUntil(() => _TIME <= PlaybackChanged + preLoad && !noteEnd);
        int temp = noteIDX;
        for (int i = 0; i < Manager.keycount; i++)
        {
            int cc = NoteList[noteIDX].COLUMN;
            while (start_queue.Count < 10) //풀링 큐 부족할 시 노트 추가생성 기다리기
            {
                SetPooling(1);
                yield return null;
            }
            var note = start_queue.Dequeue();
            note.SetActive(true);
            note.gameObject.GetComponent<NoteRenderer>().SetInfo(
                cc,
                NoteList[noteIDX].TIME,
                NoteList[noteIDX].ISLN,
                NoteList[noteIDX].LNLENGTH,
                _TIME
                );

            if (NoteList[noteIDX].ISLN)
                StartCoroutine(CreateLongnoteEnd(NoteList[noteIDX].LNLENGTH, cc, note)); //롱노트끝 생성 코루틴


            if (noteIDX < NoteList.Length - 1)
                noteIDX++;
            else
            {
                noteEnd = true;
                break;
            }
            if (NoteList[temp].TIME != NoteList[noteIDX].TIME) break;
        } //7라인 검사후 없으면 break;
        StartCoroutine(NoteSystem());
    }
    IEnumerator InputSystem()
    {
        //노트 콜리젼 오브젝트 출력 (리얼타임)
        _RTIME = NoteList[rnoteIDX].TIME;
        float _TIME2 = NoteList[rnoteIDX].TIME;
        yield return new WaitUntil(() => _RTIME <= Playback + 1000f && !rnoteEnd);
        int temp = rnoteIDX;
        for (int i = 0; i < Manager.keycount; i++)
        {
            int cc = NoteList[rnoteIDX].COLUMN;
            while (n_queue.Count < 3) //풀링 큐 부족할 시 노트 추가생성 기다리기
            {
                SetPooling(0);
                yield return null;
            }
            var note = n_queue.Dequeue();
            note.SetActive(true);
            note.gameObject.GetComponent<ColNote>().SetInfo(
                cc,
                NoteList[rnoteIDX].TIME,
                NoteList[rnoteIDX].ISLN,
                NoteList[rnoteIDX].LNLENGTH,
                _RTIME,
                NoteList[rnoteIDX].KeySoundINDEX
                );
            if (rnoteIDX < NoteList.Length - 1)
                rnoteIDX++;
            else
            {
                rnoteEnd = true;
                break;
            }
            if (NoteList[temp].TIME != NoteList[rnoteIDX].TIME) break;
        } //7라인 검사후 없으면 break;
        StartCoroutine(InputSystem());
    }
    IEnumerator mBarSystem()
    {
        float t = GetNoteTime(barlist[barIDX]);

        yield return new WaitUntil(() => t <= PlaybackChanged + preLoad && !noteEnd);

        for (int i = 0; i < 10; i++)
        {
            while (b_queue.Count < 3)
            {
                BarPooling();
                yield return null;
            }
            var bar = b_queue.Dequeue();
            bar.SetActive(true);
            bar.GetComponent<MeasureBars>()._TIME = t;

            barIDX++;
            if (barIDX == barlist.Count) break;

            t = GetNoteTime(barlist[barIDX]);
        }
        if (barIDX != barlist.Count)
            StartCoroutine(mBarSystem());
    }
    IEnumerator CreateLongnoteEnd(int LNTIME, int cc, GameObject note)
    {
        float __TIME = GetNoteTime(LNTIME);
        yield return new WaitUntil(() => __TIME <= PlaybackChanged + preLoad);
        while (end_queue.Count < 3) //풀링 큐 부족할 시 노트 추가생성 기다리기
        {
            SetPooling(2);
            yield return null;
        }
        var lnend = end_queue.Dequeue();
        lnend.SetActive(true);
        lnend.GetComponent<NoteEnd>().setInfo(cc, LNTIME, note, __TIME);
        note.gameObject.GetComponent<NoteRenderer>().setLnEnd(lnend);

    } //롱노트끝을 생성
    IEnumerator ShowResult()
    {
        yield return new WaitForSeconds(2f);
        //RankSys.updateScore(1, (int)Mathf.Round(Score), !isFailed);
        Instantiate(result);
    } //결과창
    #endregion

    #region Async Parsing File / Load Audio Files
    //async
    async void ReadFile(string filePath) //배열에 노트 구조체 추가 (쓰레딩)
    {
        //키사운드 로딩

        string path = NowPlaying.PLAY.FOLDER;
        string searchPatterns = "*.ogg|*.wav";
        keysounds = searchPatterns
                              .Split('|')
                              .SelectMany(searchPattern => Directory.GetFiles(path, searchPattern)).ToArray();
        FileInfo fileInfo = new FileInfo(filePath);
        double _bpmorigin = 1;
        double _bpm = 1;
        double _sv = 1;
        bool hitObjects = false; //노트 정보 검색
        bool Timings = false;
        bool Events = false;
        if (fileInfo.Exists)
        {
            string line;
            LastNoteTiming = 0;
            StreamReader rdr = new StreamReader(filePath);
            await Task.Run(() =>
            {
                while ((line = rdr.ReadLine()) != null)
                {
                    //if (line.Contains(""))

                    if (!Timings) Timings = line.Contains("[TimingPoints]");
                    if (!hitObjects) hitObjects = line.Contains("[HitObjects]");
                    if (!Events) Events = line.Contains("[Events]");
                    if (line.Contains(","))
                    {
                        if (Events && !Timings && !hitObjects)
                        {
                            if ((line.Contains("Sample") || line.Substring(0,1) == "5") && !line.Contains("Storyboard"))
                            {
                                int ksindex = -1;
                                bool exists = false;
                                string[] ksarr = line.Split(',');

                                string samplesound = ksarr[3];
                                samplesound = samplesound.Replace("\"", string.Empty).Trim();
                                //int sampletiming = int.Parse(ksarr[1]);
                                if (!int.TryParse(ksarr[1], out int sampletiming))
                                {
                                    continue;
                                }
                                if (samplesound.Contains(".ogg") || samplesound.Contains(".wav"))
                                {

                                    for (int i = 0; i < LoadedKeySounds.Count; i++)
                                    {
                                        if (LoadedKeySounds[i] == samplesound)
                                        {
                                            exists = true;
                                            ksindex = i;
                                            SampleList.Add(new Samples(sampletiming, i));
                                        }
                                    }
                                    if (!exists)
                                    {
                                        for (int i = 0; i < keysounds.Length; i++)
                                        {
                                            if (string.Compare(Path.GetFileName(keysounds[i]), samplesound) == 0)
                                            {

                                                player.LoadKeySound(keysounds[i]);
                                                LoadedKeySounds.Add(samplesound);
                                                ksindex = LoadedKeySounds.Count - 1;
                                                SampleList.Add(new Samples(sampletiming, ksindex));
                                            }
                                        }
                                    }
                                }


                            }
                        }
                        if (Timings && !hitObjects)
                        {
                            string[] arr2 = line.Split(',');
                            string strbpm = arr2[1];

                            if (arr2[6] == "1")
                            {
                                _sv = 1f;
                                _bpmorigin = double.Parse(strbpm);
                                _bpm = _bpmorigin; //if bpm // o2jam 확장자
                                
                            }
                            else
                            {
                                _sv = -double.Parse(strbpm) / 100;
                                _bpm = _bpmorigin * _sv; //if svchange // osumania 확장자
                            }

                            if (_bpm < 0) continue;
                            TimeList[timingidx] = new Timings(float.Parse(arr2[0]), _bpm);
                            timingidx++;
                        }
                        if (hitObjects)
                        {
                            int ksindex = -1;
                            bool exists = false;
                            string[] ksarr = line.Split(':');

                            string keysound = ksarr[ksarr.Length - 1];

                            if (keysound.Contains(".ogg") || keysound.Contains(".wav"))
                            {

                                for (int i = 0; i < LoadedKeySounds.Count; i++)
                                {
                                    if (LoadedKeySounds[i] == keysound)
                                    {
                                        exists = true;
                                        ksindex = i;
                                    }
                                }
                                if (!exists)
                                {
                                    for (int i = 0; i < keysounds.Length; i++)
                                    {
                                        if (string.Compare(Path.GetFileName(keysounds[i]), keysound) == 0)
                                        {

                                            player.LoadKeySound(keysounds[i]);
                                            LoadedKeySounds.Add(keysound);
                                            ksindex = LoadedKeySounds.Count - 1;
                                        }
                                    }
                                }
                            }
                            string[] arr = line.Split(',', ':');
                            int nt = int.Parse(arr[2]);
                            int lnlength = 0;
                            bool isln = false;
                            if (int.Parse(arr[3]) != 1 && int.Parse(arr[3]) != 5)
                            {
                                isln = true;
                                lnlength = int.Parse(arr[5]);
                            }
                            int col;
                            if (Manager.keycount == 7)
                            {
                                col = (int)Mathf.Round((int.Parse(arr[0]) - 36) / 73f);
                                col = keys7[col];
                            }
                            else
                            {
                                col = (int)Mathf.Round((int.Parse(arr[0]) - 64) / 128f);
                                col = keys4[col];
                            }
                            
                            NoteList[noteidx] = new Notes(col, nt, isln, lnlength, ksindex);
                            noteidx++;

                            if (LastNoteTiming < lnlength) LastNoteTiming = lnlength;
                            if (LastNoteTiming < nt) LastNoteTiming = nt;
                        }
                    }
                    if (Timings && (line == string.Empty)) Timings = false;
                    progress++;
                }
            });

            rdr.Close();

            SortNote();

            SortSample();

            StartCoroutine(SongInit());
        }
        else
            Debug.Log("error");
    }
    public void SortNote()
    {
        Array.Sort(NoteList, delegate (Notes A, Notes B)
        {
            if (A.TIME == B.TIME)
                return 0;
            if (A.TIME > B.TIME)
                return 1;
            else
                return -1;
        });
    }
    public void SortSample()
    {
        SampleList.Sort(delegate (Samples A, Samples B)
        {
            if (A.TIME == B.TIME)
                return 0;
            if (A.TIME > B.TIME)
                return 1;
            else
                return -1;
        });
    }
    #endregion

}
