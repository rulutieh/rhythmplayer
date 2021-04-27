using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.IO;


public class FileReader : MonoBehaviour
{


    public float offset, loadprogress;
    public static float judgeoffset = -3.15f, HP = 1f, Score;
    public static int noteIDX, barIDX, timeIDX, preLoad, combo = 0, maxcombo;
    public static bool isFailed, isPlaying, isVideoLoaded;
    public static float PlaybackChanged, Playback;
    public static float bpm = 0, startbpm = 0, nextbpm = 0;
    public static double multiply;
    float p, _TIME, barTIME;
    int noteidx = 0;
    int timingidx = 0;
    public int progress = 0;
    int[] keys = { 0, 1, 2, 3, 4, 5, 6 };
    public GameObject NoteObj, endln, result, gameover, judgeobj, barobj;
    bool isLoaded = false, svEnd, noteEnd, resultload;
    scrRank RankSys;
    MusicHandler player;
    AudioSource aud;
    GameObject w;
    NowPlaying select;
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
    [Serializable]
    struct Notes
    {
        public int COLUMN;
        public int TIME;
        public bool ISLN;
        public int LNLENGTH;
        public Notes(int COLUMN, int TIME, bool ISLN, int LNLENGTH)
        {
            this.COLUMN = COLUMN;
            this.TIME = TIME;
            this.ISLN = ISLN;
            this.LNLENGTH = LNLENGTH;
        }

    }
    [SerializeField]
    Notes[] NoteList;
    [SerializeField]
    Timings[] TimeList;
    [SerializeField]
    List<int> barlist = new List<int>();

    int TimingCount, NoteCount;
    public static int NoteCountLongnote, COOL, GREAT, GOOD, MISS, BAD;
    // Start is called before the first frame update
    void Awake()
    {
        
        Application.targetFrameRate = 500;
        barIDX = 0;
        noteIDX = 0;
        timeIDX = 0;
        aud = GetComponent<AudioSource>();
    }
    void Start()
    {
        //랭킹 등록 시스템
        w = GameObject.FindWithTag("world");
        RankSys = w.GetComponent<scrRank>();
        player = w.GetComponent<MusicHandler>();
        preLoad = 3000;
        aud.volume = scrSetting.Volume;

        isFailed = isPlaying = false;
        NoteCountLongnote = COOL = GREAT = GOOD = MISS = BAD = 0; //판정 초기화
        Score = 0; 
        HP = 1f;
        combo = maxcombo = 0;

        //고른 음악정보 가져오기

        string filePath = NowPlaying.FILE;
        offset = NowPlaying.OFFSET;
        aud.clip = NowPlaying.AUD;
        //player.LoadSound(NowPlaying.MUSICFILE);
        ReadFile(filePath); //파일 읽기 시작

        NoteList = new Notes[NowPlaying.NOTECOUNTS + NowPlaying.LONGNOTECOUNTS];
        TimeList = new Timings[NowPlaying.TIMINGCOUNTS];

        if (scrSetting.Random) //노트 랜덤배치
            Random(keys);
        if (scrSetting.Mirror)
            for (int i = 0; i < keys.Length; i++) //노트 미러배치
                keys[i] = 6 - i;
    }
    // Update is called once per frame
    void Update()
    {
        judgeoffset = -3.15f + scrSetting.stageYPOS;

        if (combo > maxcombo) maxcombo = combo; //최대 콤보 체크
        if (HP <= 0) //게임오버
        {
            isFailed = true;
            gameover.SetActive(true);
        }
        //노트, 타이밍 생성
        if (isLoaded)
        {
            if (!scrSetting.isFixedScroll)
            {
                multiply = 3f / 410f * scrSetting.scrollSpeed;
            }
            else
                multiply = 3f / NowPlaying.MEDIAN * scrSetting.scrollSpeed;
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
                if (__t < Playback && !resultload)
                {
                    resultload = true;
                    StartCoroutine(ShowResult());
                }
            }

            //if (aud.isPlaying) isPlaying = true;
        }
    }
    void AudioStart() //오프셋 Invoke로 실행
    {
        //aud.Play();
        player.PlayMP3();
        isPlaying = true;
    }
    IEnumerator SongInit() //async 종료 후 해당 데이터로 init
    {
        GetBarTime();
        yield return new WaitUntil(() => isVideoLoaded);
        Debug.Log("Load Time : " + Time.timeSinceLevelLoad);
        startbpm = bpm = (float)TimeList[0].BPM; //1비트당 소모되는 ms
        StartCoroutine(BpmChange());
        StartCoroutine(NoteSystem());
        StartCoroutine(mBarSystem());
        PlaybackChanged = Playback = -2000f;
        p = Playback;
        Invoke("AudioStart", offset + 1.9f + scrSetting.GlobalOffset);

        isLoaded = true;

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
        int cc = (int)Mathf.Round((NoteList[idx].COLUMN - 36) / 73f);
        cc = keys[cc]; //배치옵션
        var note = Instantiate(NoteObj, new Vector2(transform.position.x, 1000f), Quaternion.identity);
        note.gameObject.GetComponent<scrNote>().SetInfo(cc, NoteList[idx].TIME, NoteList[idx].ISLN, NoteList[idx].LNLENGTH, _TIME);
        if (NoteList[idx].ISLN)
        {
            StartCoroutine(CreateLongnoteEnd(NoteList[idx].LNLENGTH, cc, note)); //롱노트끝 생성 코루틴
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
        _TIME = GetNoteTime(NoteList[noteIDX].TIME);
        float _TIME2 = NoteList[noteIDX].TIME;
        yield return new WaitUntil(() => _TIME <= PlaybackChanged + preLoad && !noteEnd );

        for (int i = 0; i < 7; i++)
        {
            CreateNote(noteIDX, _TIME); //노트생성
            int temp = noteIDX;
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
    IEnumerator mBarSystem()
    {
        float t = GetNoteTime(barlist[barIDX]);
        yield return new WaitUntil(() => t <= PlaybackChanged + preLoad && !noteEnd);
        var b = Instantiate(barobj);
        b.GetComponent<scrMeasureBar>()._TIME = t;

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
        lnend.GetComponent<scrNoteEnd>().setInfo(cc, LNTIME, note, __TIME);
        note.gameObject.GetComponent<scrNote>().setLnEnd(lnend);

    } //롱노트끝을 생성
    //async

    async void ReadFile(string filePath) //배열에 노트 구조체 추가 (쓰레딩)
    {
        FileInfo fileInfo = new FileInfo(filePath);
        double _bpmorigin = 1;
        double _bpm = 1;
        double _sv = 1;
        bool hitObjects = false; //노트 정보 검색
        bool Timings = false;
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
                    if (line.Contains(","))
                    {
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
                            string[] arr = line.Split(',', ':');

                            int lnlength = 0;
                            bool isln = false;
                            if (int.Parse(arr[3]) != 1 && int.Parse(arr[3]) != 5)
                            {
                                isln = true;
                                lnlength = int.Parse(arr[5]);
                            }
                            NoteList[noteidx] = new Notes(int.Parse(arr[0]), int.Parse(arr[2]), isln, lnlength);
                            noteidx++;
                        }
                    }
                    if (Timings && (line == string.Empty)) Timings = false;
                    progress++;
                }
            });

            rdr.Close();

            StartCoroutine(SongInit());
        }
        else
            Debug.Log("error");
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
    public void SetJudge(int a)
    {
        combo = 0;
        judgeobj.SetActive(true);
        if (a == 1) //롱노트 아애 안눌럿을시
            judgeobj.GetComponent<scrJudge>().setInfo(5);
        else
            judgeobj.GetComponent<scrJudge>().setInfo(4);
    }
}
