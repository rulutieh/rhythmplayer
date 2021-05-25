using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.IO;
using System.Linq;

public class FileReader : MonoBehaviour
{


    public float offset, loadprogress;
    public static float judgeoffset = -3.15f;
    public static bool isPlaying, isVideoLoaded, resultload, isLoaded;
    public static float PlaybackChanged, Playback;
    public static float bpm = 0, startbpm = 0;
    public static double multiply;
    float p, _TIME, _RTIME, barTIME;
    int sampleIDX, noteIDX, rnoteIDX, barIDX, timeIDX, preLoad, noteidx = 0, timingidx = 0;
    public int progress = 0;
    int[] keys = { 0, 1, 2, 3, 4, 5, 6 };
    public GameObject NoteObj, ColObj, endln, result, gameover, initsetting, judgeobj, barobj, LoadCircle;
    bool svEnd, noteEnd, rnoteEnd, sampleEnd, playfieldon, musicOn;
    RankSystem RankSys;
    //fmod
    MusicHandler player;
    GameObject w;
    NowPlaying select;
    //타이밍
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


    int TimingCount, NoteCount;
    public static int NoteCountLongnote;
    // Start is called before the first frame update
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

        if (GlobalSettings.Random) //노트 랜덤배치
            Random(keys);
        for (int i = 0; i < keys.Length; i++) 
        {
            if (GlobalSettings.Mirror) keys[i] = 6 - i;//노트 미러배치
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
        preLoad = (int)(1500f/GlobalSettings.scrollSpeed); //노트 풀링 오프셋
       
        playfieldon = true;
        isPlaying = false;
        NoteCountLongnote = 0; 

        //고른 음악정보 가져오기

        string filePath = NowPlaying.FILE;
        offset = NowPlaying.OFFSET;
        ReadFile(filePath); //파일 읽기 시작

        NoteList = new Notes[NowPlaying.NOTECOUNTS + NowPlaying.LONGNOTECOUNTS];
        TimeList = new Timings[NowPlaying.TIMINGCOUNTS];

    }
    // Update is called once per frame
    void Update()
    {
        
        judgeoffset = -3.15f + GlobalSettings.stageYPOS;



        //노트, 타이밍 생성
        if (isLoaded)
        {
            if (!musicOn)
            {
                if (Playback > -100f + offset + GlobalSettings.GlobalOffset * 1000f)
                {
                    AudioStart();
                    musicOn = true;
                }
            }

            if (!GlobalSettings.isFixedScroll)
            {
                multiply = 3f / 410f * GlobalSettings.scrollSpeed;
            }
            else
                multiply = 3f / NowPlaying.MEDIAN * GlobalSettings.scrollSpeed;
            p += Time.deltaTime * 1000f;
            Playback = p;
            PlaybackChanged = GetNoteTime(Playback); // reamtime에 변속 계산 < 계산량 증가

            if (noteEnd) //게임 종료 시
            {
                int __t;
                if (NoteList[NoteList.Length - 1].ISLN)
                {
                    __t = NoteList[NoteList.Length - 1].LNLENGTH + 4000;
                }
                else
                {
                    __t = NoteList[NoteList.Length - 1].TIME + 4000;
                }
                if (__t < Playback - 3700f && playfieldon)
                {
                    playfieldon = false;
                    var gameObjects = GameObject.FindGameObjectsWithTag("player");
                    for (var i = 0; i < gameObjects.Length; i++)
                    {
                        gameObjects[i].GetComponent<ColumnSetting>().onResult();
                    }
                    
                }

                if (__t < Playback && !resultload)
                {
                    //결과창 로드
                    if (!ScoreManager.isFailed && !GlobalSettings.AutoPlay) //저장
                    RankSys.SaveScore(
                        NowPlaying.HASH, 
                        GlobalSettings.playername, 
                        Mathf.RoundToInt(ScoreManager.Score), 
                        (ScoreManager.BAD == 0 && ScoreManager.MISS == 0) ? 1 : 0 ,
                        ScoreManager.maxcombo,
                        DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")
                        );
                    resultload = true;
                    StartCoroutine(ShowResult());
                    w.GetComponent<GlobalSettings>().SaveSettings();
                }
            }

            if (SampleList.Count != 0)
                SampleSystem();
        }
    }
    void AudioStart() //오프셋 Invoke로 실행
    {
        if (!NowPlaying.isVirtual)
            player.PlayMP3();
        isPlaying = true;
    }
    IEnumerator SongInit() //async 종료 후 해당 데이터로 init
    {

        GetBarTime();
        yield return new WaitUntil(() => isVideoLoaded);
        Debug.Log("Load Time : " + Time.timeSinceLevelLoad);
        yield return new WaitForSeconds(1.2f);
        LoadCircle.GetComponent<LoadIcon>().Fade();
        yield return new WaitUntil(() => !Input.GetKey(KeyCode.LeftControl));
        startbpm = bpm = (float)TimeList[0].BPM; //1비트당 소모되는 ms
        StartCoroutine(BpmChange());
        StartCoroutine(NoteSystem());
        StartCoroutine(InputSystem());
        StartCoroutine(mBarSystem());

        PlaybackChanged = Playback = -2000f;
        p = Playback;
        //Invoke("AudioStart", offset + 1.9f + scrSetting.GlobalOffset);
        isLoaded = true;

        yield return new WaitForSeconds(1f);
        initsetting.GetComponent<playerinit>().hideChilds(); //스테이지 설정 끄기

    }
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
        double prevBPM = 1f;
        for (int i = 0; i < TimeList.Length; i++)
        {
            float _time = TimeList[i].TIME;
            double _listbpm = TimeList[i].BPM;
            double _bpm;
            if (_time > TIME) break; //변속할 타이밍이 아니면 빠져나오기
            _bpm = NowPlaying.MEDIAN / _listbpm;
            newTIME += (_bpm - prevBPM) * (TIME - _time); //거리계산
            prevBPM = _bpm; //이전bpm값
        }
        return (float)newTIME; //최종값 리턴
    }
    void CreateNote(int idx, double t) // 노트 생성 메소드
    {
        int cc = NoteList[idx].COLUMN;
        var note = Instantiate(NoteObj, new Vector2(transform.position.x, 1000f), Quaternion.identity);
        note.gameObject.GetComponent<NoteRenderer>().SetInfo(cc, NoteList[idx].TIME, NoteList[idx].ISLN, NoteList[idx].LNLENGTH, _TIME);
        if (NoteList[idx].ISLN)
        {
            StartCoroutine(CreateLongnoteEnd(NoteList[idx].LNLENGTH, cc, note)); //롱노트끝 생성 코루틴
        }
    }
    void CreateCollision(int idx, double t) // 노트 생성 메소드
    {
        int cc = NoteList[idx].COLUMN;
        var note = Instantiate(ColObj, new Vector2(transform.position.x, 1000f), Quaternion.identity);
        note.gameObject.GetComponent<ColNote>().SetInfo(cc, NoteList[idx].TIME, NoteList[idx].ISLN, NoteList[idx].LNLENGTH, _RTIME, NoteList[idx].KeySoundINDEX);
    }
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
            } //7라인 검사후 없으면 break;
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
        float _TIME2 = NoteList[noteIDX].TIME;
        yield return new WaitUntil(() => _TIME <= PlaybackChanged + preLoad && !noteEnd );
        int temp = noteIDX;
        for (int i = 0; i < 7; i++)
        {
            CreateNote(noteIDX, _TIME); //노트생성
            
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
        //순간이동 노트의 경우에는 raycast가 무시되므로 고정 속도의 콜리더 사용
        _RTIME = NoteList[rnoteIDX].TIME;
        float _TIME2 = NoteList[rnoteIDX].TIME;
        yield return new WaitUntil(() => _RTIME <= Playback + 2500f && !rnoteEnd);
        int temp = rnoteIDX;
        for (int i = 0; i < 7; i++)
        {
            CreateCollision(rnoteIDX, _RTIME); //노트생성
            
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
        var b = Instantiate(barobj);
        b.GetComponent<MeasureBars>()._TIME = t;

        barIDX++;
        if (barIDX != barlist.Count)
            StartCoroutine(mBarSystem());
    }
    IEnumerator ShowResult()
    {
        yield return new WaitForSeconds(2f);
        //RankSys.updateScore(1, (int)Mathf.Round(Score), !isFailed);
        Instantiate(result);
    } //결과창
    IEnumerator CreateLongnoteEnd(int LNTIME, int cc, GameObject note)
    {
        float __TIME = GetNoteTime(LNTIME);
        yield return new WaitUntil(() => __TIME <= PlaybackChanged + preLoad);
        var lnend = Instantiate(endln, new Vector2(transform.position.x, 1000f), Quaternion.identity);
        lnend.GetComponent<NoteEnd>().setInfo(cc, LNTIME, note, __TIME);
        note.gameObject.GetComponent<NoteRenderer>().setLnEnd(lnend);

    } //롱노트끝을 생성
    //async
    async void ReadFile(string filePath) //배열에 노트 구조체 추가 (쓰레딩)
    {
        //키사운드 로딩
        
        string path = NowPlaying.FOLDER;
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
            StreamReader rdr = new StreamReader(filePath);
            await Task.Run(() => {
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
                            if (line.Contains("Sample") && !line.Contains("Storyboard"))
                            {
                                int ksindex = -1;
                                bool exists = false;
                                string[] ksarr = line.Split(',');

                                string samplesound = ksarr[3];
                                samplesound = samplesound.Replace("\"", string.Empty).Trim();
                                //int sampletiming = int.Parse(ksarr[1]);
                                if (!int.TryParse(ksarr[1],out int sampletiming))
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
                                _bpmorigin = double.Parse(strbpm);
                                _bpm = _sv * _bpmorigin; //if bpm // o2jam 확장자
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
                                        if (string.Compare(Path.GetFileName(keysounds[i]),keysound) == 0)
                                        {
                                            
                                            player.LoadKeySound(keysounds[i]);
                                            LoadedKeySounds.Add(keysound);
                                            ksindex = LoadedKeySounds.Count - 1;
                                        }
                                    }
                                }
                            }
                            string[] arr = line.Split(',', ':');

                            int lnlength = 0;
                            bool isln = false;
                            if (int.Parse(arr[3]) != 1 && int.Parse(arr[3]) != 5)
                            {
                                isln = true;
                                lnlength = int.Parse(arr[5]);
                            }
                            int col = (int)Mathf.Round((int.Parse(arr[0]) - 36) / 73f);
                            col = keys[col];
                            NoteList[noteidx] = new Notes(col, int.Parse(arr[2]), isln, lnlength, ksindex);
                            noteidx++;
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




}
