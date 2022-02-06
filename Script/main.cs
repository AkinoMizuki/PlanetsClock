using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TimeController;
using TMPro;

public class main : MonoBehaviour
{/* mainクラス */

    [SerializeField]
    private TextMeshProUGUI TmpUGUI;

    public Transform TimeController; //時間取得関数

    public GameObject Hour_Transform;      //時針
    public GameObject Minutes_Transform;   //分
    public GameObject Seconds_Transform;   //秒

    public GameObject World_GridLine_Transform; //地平軸
    public GameObject EquatorialGridLine_Transform; //赤道軸
    public GameObject EclipticGridLine_Transform; //黄道軸
    public GameObject GalaxyGridLine_Transform; //銀河軸

    public GameObject Moon_Blender_Transform; //月影

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

    // Start is called before the first frame update
    void Start()
    {/* スタート関数 */

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
        Moon_Blender_Transform = GameObject.Find("Moon_Blender");               //月影

        //時計回転軸の初期化
        Hour_Transform.transform.localRotation = Quaternion.Euler(0, 0, h_Angle);
        Minutes_Transform.transform.localRotation = Quaternion.Euler(0, 0, m_Angle);
        Seconds_Transform.transform.localRotation = Quaternion.Euler(0, 0, s_Angle);

        //星の初期化
        EquatorialGridLine_Transform.transform.localRotation = Earth_Rotat;

        //人工衛星位置取得

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

        //ユリウス日取得
        JD = TimeController.GetComponent<TimeController>().GetJD(Date_time);
        //修正ユリウス日取得
        MJD = TimeController.GetComponent<TimeController>().GetMJD(JD);

        //GST取得
        GST = TimeController.GetComponent<TimeController>().GetGst(Date_time);

        //1日毎に人工衛星位置更新

        //ISS(ZARYA)
        //1 25544U 98067A   22037.48730771  .00006537  00000 + 0  12329 - 3 0  9991
        //2 25544  51.6415 263.8482 0006642 103.3178 318.7604 15.49790898324916

        //元期(ET)
        ISS_orbitalElements.EpochTime = 22037.48730771;
        //近地点引数(ω)
        ISS_orbitalElements.Argument = 103.3178;
        //軌道傾斜角(i)
        ISS_orbitalElements.InclinationAngle = 51.6415;
        //昇交点赤経(Ω)
        ISS_orbitalElements.AscendingNode = 263.8482;
        //離心率(e)
        ISS_orbitalElements.Eccentricity = 0.0006642;
        //平均近点角(M0)
        ISS_orbitalElements.MeanAnomaly = 318.7604;
        //平均運動(M1)
        ISS_orbitalElements.MeanMotion = 15.49790898;
        //平均運動変化係数(M2)
        ISS_orbitalElements.MeanCoefficient = 0.00006537;


        //緯度経度変換がNG

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
        Quaternion Rotat_EquatorialGridLine = Earth_Rotat * Nagoya_Rotat * Nagoya_Rotat_2 * Quaternion.Euler(0, Time_Rotat, 0);
        //黄道グリッド
        Quaternion Rotat_EclipticGridLine = Rotat_EquatorialGridLine * Quaternion.Euler(0, 0, EarthRotat) * Quaternion.Euler(0, -(float)Precession_value, 0);
        //銀河グリッド
        Quaternion Rotat_GalaxyGridLine = (Rotat_EclipticGridLine * Quaternion.Euler(0, 0, -EarthRotat)) * Quaternion.Euler((float)-62.6, 0, 0) * Quaternion.Euler(0, -123, 0);


        EquatorialGridLine_Transform.transform.localRotation = Rotat_EquatorialGridLine;    //赤道軸
        EclipticGridLine_Transform.transform.localRotation = Rotat_EclipticGridLine;      //黄道軸
        GalaxyGridLine_Transform.transform.localRotation = Rotat_GalaxyGridLine;        //銀河軸


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
        double s_gsh = ((GST - h_gsh) * 60 * 60) - (m_gsh * 60 );

        //0.00の切り捨て処理
        MoonAge = Math.Floor(MoonAge * 100) / 100;

        ISS_orbit.Altitude = Math.Floor(ISS_orbit.Altitude * 1000) / 1000;

        //テキスト更新
        TmpUGUI.text = "■Time<br> " +
            utc_time.Year.ToString("0000") + "/" + utc_time.Month.ToString("00") + "/" + utc_time.Day.ToString("00") + "/" +
            utc_time.Hour.ToString("00") + ":" + utc_time.Minute.ToString("00") + ":" + utc_time.Second.ToString("00") + " (UTC)<br> " +
            os_time.Year.ToString("0000") + "/" + os_time.Month.ToString("00") + "/" + os_time.Day.ToString("00") + "/" +
            os_time.Hour.ToString("00") + ":" + os_time.Minute.ToString("00") + ":" + os_time.Second.ToString("00") + " (JST)<br><br>" +
            " GST "+ h_gsh.ToString("00") + ":" + m_gsh.ToString("00") + ":" + s_gsh.ToString("00") + "<br>" +
            " JD  "+ JD +"<br>" +
            " MJD " + MJD + "<br><br>" +
            "■ISS(ZARYA)<br>" +
            " N "+ ISS_direction.N  + "<br>" +
            " E " + ISS_direction.E + "<br>" +
            " Alt " + ISS_orbit.Altitude.ToString("00.000") + "<br><br>" +
            "■Moon<br> " +
            MoonAge.ToString("00.00");

    }/* END_テキスト更新 */

}/* END_mainクラス */
