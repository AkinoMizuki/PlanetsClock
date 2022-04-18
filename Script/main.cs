using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TimeController;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;

public class main : MonoBehaviour
{/* mainクラス */

    [SerializeField]
    private TextMeshProUGUI TmpUGUI;

    public Camera ProjectionCameraTransform;

    /*=== アップデートチェック ===*/
    UpdateChecker.CheckProgram UpdateSequence = new UpdateChecker.CheckProgram();

    /*=== メインexeのディレクトリー取得用 ===*/
    public static string PlanetsClock_ExePass = "";

    //位置情報
    LocationUpdater LocationUpdater = new LocationUpdater();


    //関数オブジェクト呼び込み
    public Transform TimeController;    //時間取得関数
    public Transform GetSatellite;      //人工衛星データ関数

    public GameObject DomeProjector;    //ドームカメラ

    public GameObject Clock_obj_Transform; //時計本体
    public GameObject Hour_Transform;      //時針
    public GameObject Minutes_Transform;   //分
    public GameObject Seconds_Transform;   //秒
    

    public GameObject World_GridLine_Transform; //地平軸
    public GameObject EquatorialGridLine_Transform; //赤道軸
    public GameObject EclipticGridLine_Transform; //黄道軸
    public GameObject GalaxyGridLine_Transform; //銀河軸

    public GameObject AllSky_Transform;         //恒星

    public GameObject Moon_Blender_Transform; //月影

    /*=== アップデートウィンドウ ===*/
    public GameObject Update_Transform;         //アップデートウィンドウ

    //位置情報用
    private string[] locationInformation = new string[4];

    //ローカル変数
    //針
    private float h_Angle;  //時針
    private float m_Angle;  //分
    private float s_Angle;  //秒

    private double JD;      //ユリウス日
    private double MJD;     //修正ユリウス日
    private double GST;     //恒星時
    private double MoonAge; //月齢
    

    private float EarthRotat = (float)23.4;
    //原点用
    private Quaternion SyncRotat = Quaternion.Euler(0, 0, 0);
    //地平グリッド
    private Quaternion SyncRotat_GridLine = Quaternion.Euler(0, 0, 0);
    //赤道グリッド
    private Quaternion SyncRotat_EquatorialGridLine = Quaternion.Euler(0, 0, 0);
    //黄道グリッド
    private Quaternion SyncRotat_EclipticGridLine = Quaternion.Euler(0, 0, 0);
    //銀河グリッド
    private Quaternion SyncRotat_GalaxyGridLine = Quaternion.Euler(0, 0, 0);
    //地球傾き
    private Quaternion Earth_Rotat = Quaternion.Euler((float)23.4, 0, 0);
    //名古屋
    private double latitude = 35.16509646; 
    private Quaternion Nagoya_Rotat = Quaternion.Euler((float)35.16509646, 0, 0) ;
    private Quaternion Nagoya_Rotat_2 = Quaternion.Euler(0, -(float)136.90010642, 0);
    //歳差(掛ける元)
    private double Precession = (double)360 / (double)26000;
    //歳差の引く定数(0で2000年)
    private double Precession_constant = (double)2000;
    //歳差代入値
    private double Precession_value = (double)0;
    //月齢
    private double Moon = (double)360 / (double)29.53059;
    //月齢代入値
    private double Moon_value = (double)0;


    //ISS(ZARYA)
    //1 25544U 98067A   22038.37836806  .00007168  00000 + 0  13432 - 3 0  9993
    //2 25544  51.6416 259.4336 0006553 105.8037 251.0375 15.49806680325044
    /* ISS仮初期値 */
    //元期(ET)
    private double EpochTime { get; set; }
    //近地点引数(ω)
    private double Argument { get; set; }
    //軌道傾斜角(i)
    private double InclinationAngle { get; set; }
    //昇交点赤経(Ω)
    private double AscendingNode { get; set; }
    //離心率(e)
    private double Eccentricity { get; set; }
    //平均近点角(M0)
    private double MeanAnomaly { get; set; }
    //平均運動(M1)
    private double MeanMotion { get; set; }
    //平均運動変化係数(M2)
    private double MeanCoefficient { get; set; }

    // FPS counter data
    private const int NumFrameDeltas = 10;
    private float[] m_frameDeltas = new float[NumFrameDeltas];
    private int m_curFrameDelta;


    // Start is called before the first frame update
    void Awake()
    {
        Application.targetFrameRate = 3; //2FPSに設定
                                         // Start is called before the first frame update
    }


    void Start()
    {/* スタート関数 */

#if UNITY_STANDALONE_WIN
        /*=== メインexeのディレクトリー取得 ===*/
        PlanetsClock_ExePass = Environment.CurrentDirectory;
        /*=== アップデート確認 ===*/
        UploadCheck(PlanetsClock_ExePass);
#endif

        //幅、高さ、フルスクリーン無効(window表示)、リフレッシュレート
        //Screen.SetResolution(200, 400, false, 60);

        // 自動スリープを無効にする場合
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        Clock_obj_Transform = GameObject.Find("Clock_obj");      //時針

        Vector3 pos = Clock_obj_Transform.transform.position;
        pos.x = Clock_obj_Transform.transform.position.x;
        pos.y = ProjectionCameraTransform.pixelHeight * (float)1.5; // 画面4分の1の場所に設置
        pos.z = Clock_obj_Transform.transform.position.z;
        Clock_obj_Transform.transform.position = pos;               // 座標を設定

        //ドームカメラ
        DomeProjector = GameObject.Find("Dome Projector");
        //ドームカメラ回転
        DomeProjector.transform.transform.localRotation = Quaternion.Euler(0, 180, 0);

        h_Angle = 0;  //時針
        m_Angle = 0;  //分
        s_Angle = 0;  //秒

        //時計の針のキャッシュ
        Hour_Transform = GameObject.Find("Hour");      //時針
        Minutes_Transform = GameObject.Find("Minutes");   //分針
        Seconds_Transform = GameObject.Find("Seconds");   //秒針


        World_GridLine_Transform = GameObject.Find("World_GridLine");           //地平軸
        EquatorialGridLine_Transform = GameObject.Find("EquatorialGridLine");   //赤道軸
        EclipticGridLine_Transform = GameObject.Find("EclipticGridLine");       //黄道軸
        GalaxyGridLine_Transform = GameObject.Find("GalaxyGridLine");           //銀河軸
        AllSky_Transform = GameObject.Find("AllSky_Obj");                       //恒星
        Moon_Blender_Transform = GameObject.Find("Moon_Blender");               //月影

        //時計回転軸の初期化
        Hour_Transform.transform.localRotation = Quaternion.Euler(0, 0, h_Angle);
        Minutes_Transform.transform.localRotation = Quaternion.Euler(0, 0, m_Angle);
        Seconds_Transform.transform.localRotation = Quaternion.Euler(0, 0, s_Angle);

        //星の初期化
        EquatorialGridLine_Transform.transform.localRotation = Earth_Rotat;
        AllSky_Transform.transform.localRotation = Earth_Rotat;

        //元期(ET)
        EpochTime = 22097.02427676;
        //近地点引数(ω);
        Argument = 356.4183;
        //軌道傾斜角(i)
        InclinationAngle = 51.6453;
        //昇交点赤経(Ω)
        AscendingNode = 329.0557;
        //離心率(e);
        Eccentricity = 0.0004559;
        //平均近点角(M0);
        MeanAnomaly = 147.1455;
        //平均運動(M1)
        MeanMotion = 15.49913292;
        //平均運動変化係数(M2)
        MeanCoefficient = 0.00011708;


        //人工衛星位置取得
        StartCoroutine(GetSatellite.GetComponent<GetSatellite>().GetWeb_Satellitet());

        //テキスト初期化
        TmpUGUI.text = "■Time<br>" +
            " 2000/01/01 00:00:00(UTC)<br>" +
            " 2000/01/01 00:00:00(JST)<br><br>" +
            " GST 23:56:4.09<br>" +
            " JD  2451544.50000<br>" +
            " MJD 51544.00000<br><br>" +
            "■ISS(ZARYA)<br>" +
            " N 180'00'00\"<br>" +
            " E 180'00'00\"<br>" +
            " Altitude 400<br>" +
            "■Moon<br>" +
            " 00.0";


    }/* END_スタート関数 */

    // Update is called once per frame
    void Update()
    {/* アップデート関数 */

        Vector3 pos = Clock_obj_Transform.transform.position;
        pos.x = Clock_obj_Transform.transform.position.x;
        pos.y = ProjectionCameraTransform.pixelHeight * (float)1.5; // 画面4分の1の場所に設置
        pos.z = Clock_obj_Transform.transform.position.z;
        Clock_obj_Transform.transform.position = pos;               // 座標を設定

        /*=== 取得 ===*/
        // 構造体型のインスタンス化
        TimeController.UtcTime utc_time;                    //UTC時間
        TimeController.OsTime os_time;                      //OS時間
        TimeController.Date Date_time;                      //Date時間
        TimeController.OrbitalElements ISS_orbitalElements; //データ
        TimeController.Orbit ISS_orbit;                     //ISS位置
        TimeController.Direction ISS_direction;             //北東

        //UTC時間取得 
        utc_time = TimeController.GetComponent<TimeController>().GetUtcTime();
        //OS時間取得 
        os_time = TimeController.GetComponent<TimeController>().GetOsTime();

        //手動変換
        Date_time.Year = utc_time.Year;
        Date_time.Month = utc_time.Month;
        Date_time.Day = utc_time.Day;
        Date_time.Hour = utc_time.Hour;
        Date_time.Minute = utc_time.Minute;
        Date_time.Second = utc_time.Second;

        if (Date_time.Hour == 0)
        {/*=== 人工衛星位置取得 ===*/
            if (Date_time.Minute == 0)
            {
                if (Date_time.Second == 0)
                {
                    //人工衛星位置取得
                    StartCoroutine(GetSatellite.GetComponent<GetSatellite>().GetWeb_Satellitet());
                }
            }
        }/*=== END_人工衛星位置取得 ===*/

        //ユリウス日取得
        JD = TimeController.GetComponent<TimeController>().GetJD(Date_time);
        //修正ユリウス日取得
        MJD = TimeController.GetComponent<TimeController>().GetMJD(JD);

        //GST取得
        GST = TimeController.GetComponent<TimeController>().GetGst(Date_time);

        //1日毎に人工衛星位置更新
        //元期(ET)
        ISS_orbitalElements.EpochTime = EpochTime;
        //近地点引数(ω)
        ISS_orbitalElements.Argument = Argument;
        //軌道傾斜角(i)
        ISS_orbitalElements.InclinationAngle = InclinationAngle;
        //昇交点赤経(Ω)
        ISS_orbitalElements.AscendingNode = AscendingNode;
        //離心率(e)
        ISS_orbitalElements.Eccentricity = Eccentricity;
        //平均近点角(M0)
        ISS_orbitalElements.MeanAnomaly = MeanAnomaly;
        //平均運動(M1)
        ISS_orbitalElements.MeanMotion = MeanMotion;
        //平均運動変化係数(M2)
        ISS_orbitalElements.MeanCoefficient = MeanCoefficient;


        if (Input.location.isEnabledByUser)
        {/* === 位置情報取得 ===*/
            locationInformation = LocationUpdater.GetLocation();
            
            //位置情報取得
            if ("Running" == locationInformation[0])
            {
                latitude = double.Parse(locationInformation[1]);
                Nagoya_Rotat_2 = Quaternion.Euler(0, -float.Parse(locationInformation[2]), 0);
            }

        }/* === END_位置情報取得 ===*/

        
        if (latitude >= 0)
        {/* === 地球座標変換 ===*/

            Nagoya_Rotat = Quaternion.Euler((float)(90 - latitude), 0, 0);

        }
        else
        {

            Nagoya_Rotat = Quaternion.Euler((float)(90 + -latitude), 0, 0);

        }/* === END_地球座標変換 ===*/


        //ISS(ZARYA)
        //1 25544U 98067A   22038.37836806  .00007168  00000 + 0  13432 - 3 0  9993
        //2 25544  51.6416 259.4336 0006553 105.8037 251.0375 15.49806680325044
        /*人工衛星配列データを要求*/
        string[,,] Stations = GetSatellite.GetComponent<GetSatellite>().SendStations();
        
        if (Stations[0, 0, 0] != null)
        {/*=== 軌道データnull確認 ===*/

            if (Stations[0, 0, 0] != "StationsData")
            {/*=== ISS(ZARYA) ===*/

                try
                {
                    //元期(ET)
                    ISS_orbitalElements.EpochTime = double.Parse(Stations[0, 1, 3]);
                    //近地点引数(ω)
                    ISS_orbitalElements.Argument = double.Parse(Stations[0, 2, 5]);
                    //軌道傾斜角(i)
                    ISS_orbitalElements.InclinationAngle = double.Parse(Stations[0, 1, 3]);
                    //昇交点赤経(Ω)
                    ISS_orbitalElements.AscendingNode = double.Parse(Stations[0, 2, 3]);
                    //離心率(e)
                    ISS_orbitalElements.Eccentricity = double.Parse("0." + Stations[0, 2, 4]);
                    //平均近点角(M0)
                    ISS_orbitalElements.MeanAnomaly = double.Parse(Stations[0, 2, 6]);
                    //平均運動(M1)
                    ISS_orbitalElements.MeanMotion = double.Parse(Stations[0, 2, 7].Substring(0, 10));
                    //平均運動変化係数(M2)
                    ISS_orbitalElements.MeanCoefficient = double.Parse("0" + Stations[0, 1, 4]);
                }
                catch 
                {
                    //元期(ET)
                    ISS_orbitalElements.EpochTime = EpochTime;
                    //近地点引数(ω)
                    ISS_orbitalElements.Argument = Argument;
                    //軌道傾斜角(i)
                    ISS_orbitalElements.InclinationAngle = InclinationAngle;
                    //昇交点赤経(Ω)
                    ISS_orbitalElements.AscendingNode = AscendingNode;
                    //離心率(e)
                    ISS_orbitalElements.Eccentricity = Eccentricity;
                    //平均近点角(M0)
                    ISS_orbitalElements.MeanAnomaly = MeanAnomaly;
                    //平均運動(M1)
                    ISS_orbitalElements.MeanMotion = MeanMotion;
                    //平均運動変化係数(M2)
                    ISS_orbitalElements.MeanCoefficient = MeanCoefficient;
                }

            }/*=== END_ISS(ZARYA) ===*/

        }/*=== END_軌道データnull確認 ===*/

        //ISS位置取得
        ISS_orbit = TimeController.GetComponent<TimeController>().GetOrbit(ISS_orbitalElements, Date_time);
        //変換
        Vector3 ISS_Vector3 = new Vector3(ISS_orbit.OrbitVector3.x, ISS_orbit.OrbitVector3.y, ISS_orbit.OrbitVector3.z);

        //緯度経度変換
        ISS_direction = TimeController.GetComponent<TimeController>().LongitudeLatitude(ISS_Vector3);


        //月歳取得
        MoonAge = TimeController.GetComponent<TimeController>().GetMoonAge(Date_time);

        /*=== 更新 ===*/
        //アナログ時計更新
        AnalogSetTime(os_time);

        //テキスト反映
        SetTent(utc_time, os_time, GST, JD, MoonAge, ISS_orbit, ISS_direction);

        //星空を更新
        float Time_Rotat = (float)(((360 / 24) * GST) - 90);

        //歳差角度計算
        Precession_value = Precession * ((float)utc_time.Year - Precession_constant);
        //赤道グリッド
        Quaternion Rotat_EquatorialGridLine = Nagoya_Rotat * Nagoya_Rotat_2 * Quaternion.Euler(0, Time_Rotat, 0);
        //黄道グリッド
        Quaternion Rotat_EclipticGridLine = Rotat_EquatorialGridLine * Quaternion.Euler(0, 0, EarthRotat) * Quaternion.Euler(0, -(float)Precession_value, 0);
        //銀河グリッド
        Quaternion Rotat_GalaxyGridLine =  Rotat_EclipticGridLine * Quaternion.Euler((float)62.6, 0, 0) * Quaternion.Euler(0, 123, 0);
        //恒星
        Quaternion Rotat_AllSky = Rotat_EclipticGridLine * Quaternion.Euler(0, 0, -EarthRotat);

        EquatorialGridLine_Transform.transform.localRotation = Rotat_EquatorialGridLine;    //赤道軸
        EclipticGridLine_Transform.transform.localRotation = Rotat_EclipticGridLine;        //黄道軸
        GalaxyGridLine_Transform.transform.localRotation = Rotat_GalaxyGridLine;            //銀河軸
        AllSky_Transform.transform.localRotation = Rotat_AllSky;                            //恒星

        //EquatorialGridLine_Transform.transform.localRotation = Earth_Rotat * Nagoya_Rotat * Quaternion.Euler(0, -Time_Rotat, 0);

        //月を更新
        Moon_value = Moon * MoonAge;
        Moon_Blender_Transform.transform.localRotation = Quaternion.Euler(180, 0, 0) * Quaternion.Euler(-(float)Moon_value, 0, 0);    //月影軸

    }/* END_アップデート関数 */

    void AnalogSetTime(OsTime os_time)
    {/* アナログ時計更新 */

        //アナログ針角度計算
        h_Angle = -(float)((360 / 12) * os_time.Hour + 0.5 * os_time.Minute);
        m_Angle = -(float)((360 / 60) * os_time.Minute);
        s_Angle = -(float)((360 / 60) * os_time.Second);

        //アナログ時刻反映
        Hour_Transform.transform.localRotation = Quaternion.Euler(0, 0, h_Angle);
        Minutes_Transform.transform.localRotation = Quaternion.Euler(0, 0, m_Angle);
        Seconds_Transform.transform.localRotation = Quaternion.Euler(0, 0, s_Angle);

    }/* END_アナログ時計更新 */

    void SetTent(UtcTime utc_time, OsTime os_time, double GST, double JD, double MoonAge, Orbit ISS_orbit, Direction ISS_direction)
    {/* テキスト更新 */

        double h_gsh = Math.Truncate(GST);
        double m_gsh = Math.Truncate((GST - h_gsh) * 60);
        double s_gsh = ((GST - h_gsh) * 60 * 60) - (m_gsh * 60);

        //0.00の切り捨て処理
        MoonAge = Math.Floor(MoonAge * 100) / 100;

        //方位
        string NorS = "N";
        string EorW = "E";

        if (ISS_direction.N <= 0)
        {
            NorS = "S";
        }
        if (ISS_direction.E <= 0)
        {
            EorW = "W";
        }

        //NorS ISS緯度変換
        ISS_direction.N = Math.Abs(ISS_direction.N);
        double h_NorS = Math.Truncate(ISS_direction.N);
        double m_NorS = Math.Truncate((ISS_direction.N - h_NorS) * 60);
        double s_NorS = ((ISS_direction.N - h_NorS) * 60 * 60) - (m_NorS * 60);

        //EorW ISS緯度変換
        ISS_direction.E = Math.Abs(ISS_direction.E);
        double h_EorW = Math.Truncate(ISS_direction.E);
        double m_EorW = Math.Truncate((ISS_direction.E - h_EorW) * 60);
        double s_EorW = ((ISS_direction.E - h_EorW) * 60 * 60) - (m_EorW * 60);

        //ISS高度0.000で切り捨て
        ISS_orbit.Altitude = Math.Floor(ISS_orbit.Altitude * 1000) / 1000;

        //FPS計算
        float fps = 1f / Time.deltaTime;
        // Maintain a running average of the last N frame deltas, for a more stable frame counter.
        m_frameDeltas[m_curFrameDelta++] = Time.deltaTime;
        if (m_curFrameDelta >= 10)
            m_curFrameDelta = 0;
        float totalFrameDelta = 0.0f;
        for (int i = 0; i < 10; i++)
            totalFrameDelta += m_frameDeltas[i];
        //fps =  Mathf.Round(totalFrameDelta != 0.000f ? (float)NumFrameDeltas / totalFrameDelta : 0);
        fps = (float)NumFrameDeltas / totalFrameDelta;

        if (locationInformation[0] == "")
        {
            locationInformation[0] = "non";
        }

        //テキスト更新
        TmpUGUI.text = "■Time<br> " +
            utc_time.Year.ToString("0000") + "/" + utc_time.Month.ToString("00") + "/" + utc_time.Day.ToString("00") + "/" +
            utc_time.Hour.ToString("00") + ":" + utc_time.Minute.ToString("00") + ":" + utc_time.Second.ToString("00") + " (UTC)<br> " +
            os_time.Year.ToString("0000") + "/" + os_time.Month.ToString("00") + "/" + os_time.Day.ToString("00") + "/" +
            os_time.Hour.ToString("00") + ":" + os_time.Minute.ToString("00") + ":" + os_time.Second.ToString("00") + " (JST)<br><br>" +
            " GST "+ h_gsh.ToString("00") + ":" + m_gsh.ToString("00") + ":" + s_gsh.ToString("00") + "<br>" +
            " JD  "+ JD +"<br>" +
            " MJD " + MJD + "<br><br>" +
            "■ISS(ZARYA)  ■Gps Status<br>" +
            " " + NorS + " " + h_NorS.ToString("000") + "˚" + m_NorS.ToString("00") + "'" + s_NorS.ToString("00") + "\"  " + locationInformation[0] + "<br>" + 
            " " + EorW + " " + h_EorW.ToString("000") + "˚" + m_EorW.ToString("00") + "'" + s_EorW.ToString("00") + "\"<br>" +
            " Alt " + ISS_orbit.Altitude.ToString("00.000") + "<br><br>" +
            "■Moon<br> " +
            MoonAge.ToString("00.00") +
            "         FPS " + fps.ToString("000.000");

    }/* END_テキスト更新 */


    /*********************************************************************
    *   アップデートチェック
    *********************************************************************/
    public void UploadCheck(string UpExe)
    {/*=== アップデートチェック ===*/

        bool UpResult = false;
        var version = Application.version;

        /* ==== 別スレット ==== */
        var UpResultTask = Task.Run(() =>
        {
            UpResult = UpdateSequence.UpdateCheck(UpExe, version);
        });/* ==== END_別スレット ==== */

        UpResultTask.Wait();

        //アクティブ変更
        Update_Transform.SetActive(UpResult);

    }/*=== END_アップデートチェック ===*/



}/* END_mainクラス */
