using System;
using UnityEngine;

public partial class TimeController : MonoBehaviour
{/* TimeControllerクラス */

    /***************************************/
    /*             構造体の宣言            */
    /***************************************/
    public struct UtcTime
    {/*=== UTC時間構造体 ===*/

        public double Year;
        public double Month;
        public double Day;
        public double Hour;
        public double Minute;
        public double Second;

    }/*=== END_UTC時間構造体 ===*/

    public struct OsTime
    {/*=== Pc時間構造体 ===*/

        public double Year;
        public double Month;
        public double Day;
        public double Hour;
        public double Minute;
        public double Second;

    }/*=== END_Pc時間構造体 ===*/

    public struct Date
    {/*=== Date時間構造体 ===*/

        public double Year;
        public double Month;
        public double Day;
        public double Hour;
        public double Minute;
        public double Second;

    }/*=== END_Date時間構造体 ===*/

    public struct MotionTime
    {/*=== 時間Date時間構造体 ===*/

        public double Year;
        public double Month;
        public double Day;
        public double Hour;
        public double Minute;
        public double Second;
        public double JD;
        public double MJD;
        public double GST;

    }/*=== END_時間Date時間構造体 ===*/

    public struct OrbitalElements
    {/*=== 軌道要素 ===*/

        //元期(ET)
        public double EpochTime;
        //近地点引数(ω)
        public double Argument;
        //軌道傾斜角(i)
        public double InclinationAngle;
        //昇交点赤経(Ω)
        public double AscendingNode;
        //離心率(e)
        public double Eccentricity;
        //平均近点角(M0)
        public double MeanAnomaly;
        //平均運動(M1)
        public double MeanMotion;
        //平均運動変化係数(M2)
        public double MeanCoefficient;

    }/*=== END_軌道要素 ===*/

    public struct Orbit
    {/*=== 軌道計算結果用 ===*/

        public Vector3 OrbitVector3;
        public double Altitude;

    }/*=== END_軌道計算結果用 ===*/

    public struct Direction
    {/* 方位 */

        //北
        public double N;
        //東
        public double E;

    }/* END_方位 */

    /***************************************/
    /*      　  グローバル関数             */
    /***************************************/

    public UtcTime GetUtcTime()
    {/*=== UTC時間取得 ===*/

        // 構造体型のインスタンス化
        UtcTime utc_time;

        DateTime Utc = DateTime.UtcNow;

        //年取得
        utc_time.Year = Utc.Year;
        //月取得
        utc_time.Month = Utc.Month;
        //日取得
        utc_time.Day = Utc.Day;
        //時間取得
        utc_time.Hour = Utc.Hour;
        //分取得
        utc_time.Minute = Utc.Minute;
        //秒取得
        utc_time.Second = Utc.Second;

        return utc_time;

    }/*=== END_UTC時間取得 ===*/

    public OsTime GetOsTime()
    {/*=== UTC時間取得 ===*/

        // 構造体型のインスタンス化
        OsTime os_time;

        DateTime os = DateTime.Now;

        //年取得
        os_time.Year = os.Year;
        //月取得
        os_time.Month = os.Month;
        //日取得
        os_time.Day = os.Day;
        //時間取得
        os_time.Hour = os.Hour;
        //分取得
        os_time.Minute = os.Minute;
        //秒取得
        os_time.Second = os.Second;

        return os_time;

    }/*=== END_UTC時間取得 ===*/


    public static float dir2Rot(double angle)
    {
        //[度数をラジアンに変換]
        //ラジアン = 度数 * Math.PI / 180
        return (float)(angle / 180 * Math.PI);
    }
    public static double rot2Dir(float radian)
    {
        //[ラジアンを度数に変換]
        //度数 = ラジアン * 180 / Math.PI
        return (double)(radian * 180 / Math.PI);
    }


    public double RoundNum(double num, double max)
    {/*=== 四捨五入 ===*/

        double results = num % max;
        if (results < 0)
        {
            results += max;
        }
        return results;

    }/*=== END_四捨五入 ===*/


    /***************************************/
    /*      　  天文関数             */
    /***************************************/

    public double DeltaT()
    {/*=== Terrestrial Time ===*/
        return (37 + 32);
    }/*=== Terrestrial Time ===*/


    public double GetJD(Date date)
    {/*=== ユリウス日取得 ===*/

        double year = date.Year;
        double month = date.Month;
        double day = date.Day;
        double time_in_day = date.Hour / 24 + date.Minute / 1440 + date.Second / 86400;

        if(month <= 2)
        {
            year = year - 1;
            month = month + 12;
        }

        double julian_day = Math.Floor(365.25 * (year + 4716)) + Math.Floor(30.6001 * (month + 1)) + day - 1524.5;
        double transition_offset = 0;

        if (julian_day < 2299160.5)
        {
            transition_offset = 0;
        }
        else
        {
            double tmp = Math.Floor(year / 100);
            transition_offset = 2 - tmp + Math.Floor(tmp / 4);
        }

        return julian_day + transition_offset + time_in_day;

    }/*=== END_ユリウス日取得 ===*/

    public double GetMJD(double JD)
    {/*=== 修正ユリウス日 ===*/

        return JD - 2400000.5;

    }/*=== END_修正ユリウス日取得 ===*/



    /*=== JDToUTC ===*/



    //----------------------------------------
    //--年周運動
    //----------------------------------------
    //function Console.TimeExercise.AnnualExercise(date, Annual)
    //--年周運動
    //
    //    --日時
    //    local year = tonumber(date.year) + Annual
    //    local month = tonumber(date.month)
    //    local day = tonumber(date.day)
    //    local hour = tonumber(date.hour)
    //    local min = tonumber(date.min)
    //    local sec = tonumber(date.sec)
    //
    //
    //    local Time = { year = tonumber(year), month = tonumber(month), day = tonumber(day), hour = tonumber(hour), min = tonumber(min), sec = tonumber(sec) }
    //    --ユリウス通日
    //    local jd = Orb.Time.JD(Time)
    //    local mjd = jd - 2400000.5
    //    --グリニッジ視恒星時
    //    local gst = Orb.Time.gst(Time)
    //
    //    return {
    //        year = year,
    //        month = month,
    //        day = day,
    //        hour = hour,
    //        min = min,
    //        sec = sec,
    //        jd = jd,
    //        mjd = mjd,
    //        gst = gst
    //}
    //
    //end--END_年周運動
    //
    //----------------------------------------
    //--月周運動
    //----------------------------------------
    //function Console.TimeExercise.LunarMovement(date, Lunar)
    //--月周運動
    //
    //    --日時代入
    //    local Time = {year= tonumber(date.year) ,month = tonumber(date.month) ,day = tonumber(date.day), hour = tonumber(date.hour), min = tonumber(date.min), sec = tonumber(date.sec)}
    //    
    //    --1月を取得
    //    local Set_t = {year=2000, month = 1, day = 1, hour = 0, min = 0, sec = 0}
    //    local Old_t = { year = 2000, month = 2, day = 1, hour = 0, min = 0, sec = 0 }
    //    local JD_m = Orb.Time.JD(Old_t) -Orb.Time.JD(Set_t)
    //
    //    --ユリウス通日
    //    local jd = Orb.Time.JD(Time) +(JD_m * Lunar)
    //    local mjd = jd - 2400000.5
    //    --グリニッジ視恒星時
    //    local gst = Orb.Time.gst(Time) 
    //    local fjd = Orb.Time.JDToUTC(jd)
    //
    //    --日時
    //    local year = tonumber(fjd.year)
    //    local month = tonumber(fjd.month)
    //    local day = tonumber(fjd.day)
    //    local hour = tonumber(fjd.hour)
    //    local min = tonumber(fjd.min)
    //    local sec = tonumber(fjd.sec)
    //
    //    return {
    //        year = year,
    //        month = month,
    //        day = day,
    //        hour = hour,
    //        min = min,
    //        sec = sec,
    //        jd = jd,
    //        mjd = mjd,
    //        gst = gst
    //    }
    //
    //end--END_月周運動
    //
    /****************************************/
    /*              日周運動                */
    /****************************************/
    public MotionTime DiurnalMotion(Date date ,double Diurnal)
    {/*日周運動*/

        // 構造体型のインスタンス化
        MotionTime motion_time; //出力用
        Date Set_t; //過去時間
        Date Old_t; //経過時間用

        /* 1日を取得 */

        /* セット */
        //年取得
        Set_t.Year = 2000;
        //月取得
        Set_t.Month = 1;
        //日取得
        Set_t.Day = 1;
        //時間取得
        Set_t.Hour = 0;
        //分取得
        Set_t.Minute = 0;
        //秒取得
        Set_t.Second = 0;

        /* 旧用セット */
        //年取得
        Old_t.Year = 2000;
        //月取得
        Old_t.Month = 1;
        //日取得
        Old_t.Day = 2;
        //時間取得
        Old_t.Hour = 0;
        //分取得
        Old_t.Minute = 0;
        //秒取得
        Old_t.Second = 0;

        //1日を取得
        double JD_d = GetJD(Old_t) - GetJD(Set_t);
        //ユリウス通日
        double jd = GetJD(date) + (JD_d * Diurnal);
        double mjd = jd - 2400000.5;
        //グリニッジ視恒星時
        double gst = GetGst(date);
        Date fjd = JDToUTC(jd);

        //日時
        motion_time.Year = fjd.Year;
        motion_time.Month = fjd.Month;
        motion_time.Day = fjd.Day;
        motion_time.Hour = fjd.Hour;
        motion_time.Minute = fjd.Minute;
        motion_time.Second = fjd.Second;
        motion_time.JD = jd;
        motion_time.MJD = mjd;
        motion_time.GST = gst;

        return motion_time;

    }/*日周運動*/


    //----------------------------------------
    //--時運動
    //----------------------------------------
    //function Console.TimeExercise.TimeExercise(date, h_Time)
    //--時運動
    //
    //    --日時代入
    //    local Time = {year= tonumber(date.year) ,month = tonumber(date.month) ,day = tonumber(date.day), hour = tonumber(date.hour), min = tonumber(date.min), sec = tonumber(date.sec)}
    //
    //    --1時間を取得
    //    local Set_t = {year=2000, month = 1, day = 1, hour = 0, min = 0, sec = 0}
    //    local Old_t = { year = 2000, month = 1, day = 1, hour = 1, min = 0, sec = 0 }
    //    local JD_h = Orb.Time.JD(Old_t) -Orb.Time.JD(Set_t)
    //
    //    --ユリウス通日
    //    local jd = Orb.Time.JD(Time) +(JD_h * h_Time)
    //    local mjd = jd - 2400000.5
    //    --グリニッジ視恒星時
    //    local gst = Orb.Time.gst(Time) 
    //    local fjd = Orb.Time.JDToUTC(jd)
    //
    //    --日時
    //    local year = tonumber(fjd.year)
    //    local month = tonumber(fjd.month)
    //    local day = tonumber(fjd.day)
    //    local hour = tonumber(fjd.hour)
    //    local min = tonumber(fjd.min)
    //    local sec = tonumber(fjd.sec)
    //
    //    return {
    //        year = year,
    //        month = month,
    //        day = day,
    //        hour = hour,
    //        min = min,
    //        sec = sec,
    //        jd = jd,
    //        mjd = mjd,
    //        gst = gst
    //    }
    //
    //end--END_時運動
    //
    //----------------------------------------
    //--分運動
    //----------------------------------------
    //function Console.TimeExercise.MinuteExercise(date, Minute)
    //--分運動
    //
    //    --日時代入
    //    local Time = {year= tonumber(date.year) ,month = tonumber(date.month) ,day = tonumber(date.day), hour = tonumber(date.hour), min = tonumber(date.min), sec = tonumber(date.sec)}
    //
    //    --1分を取得
    //    local Set_t = {year=2000, month = 1, day = 1, hour = 0, min = 0, sec = 0}
    //    local Old_t = { year = 2000, month = 1, day = 1, hour = 0, min = 1, sec = 0 }
    //    local JD_min = Orb.Time.JD(Old_t) -Orb.Time.JD(Set_t)
    //
    //    --ユリウス通日
    //    local jd = Orb.Time.JD(Time) +(JD_min * Minute)
    //    local mjd = jd - 2400000.5
    //    --グリニッジ視恒星時
    //    local gst = Orb.Time.gst(Time) 
    //    local fjd = Orb.Time.JDToUTC(jd)
    //
    //    --日時
    //    local year = tonumber(fjd.year)
    //    local month = tonumber(fjd.month)
    //    local day = tonumber(fjd.day)
    //    local hour = tonumber(fjd.hour)
    //    local min = tonumber(fjd.min)
    //    local sec = tonumber(fjd.sec)
    //
    //    return {
    //        year = year,
    //        month = month,
    //        day = day,
    //        hour = hour,
    //        min = min,
    //        sec = sec,
    //        jd = jd,
    //        mjd = mjd,
    //        gst = gst
    //    }
    //
    //end--END_分運動
    //
    //----------------------------------------
    //--秒周運動
    //----------------------------------------
    //function Console.TimeExercise.SecondExercise(date, Second)
    //--秒周運動
    //
    //    --日時代入
    //    local Time = {year= tonumber(date.year) ,month = tonumber(date.month) ,day = tonumber(date.day), hour = tonumber(date.hour), min = tonumber(date.min), sec = tonumber(date.sec)}
    //
    //    --1秒を取得
    //    local Set_t = {year=2000, month = 1, day = 1, hour = 0, min = 0, sec = 0}
    //    local Old_t = { year = 2000, month = 1, day = 1, hour = 0, min = 0, sec = 1 }
    //    local JD_s = Orb.Time.JD(Old_t) -Orb.Time.JD(Set_t)
    //
    //    --ユリウス通日
    //    local jd = Orb.Time.JD(Time) +(JD_s * Second)
    //    local mjd = jd - 2400000.5
    //    --グリニッジ視恒星時
    //    local gst = Orb.Time.gst(Time) 
    //    local fjd = Orb.Time.JDToUTC(jd)
    //
    //    --日時
    //    local year = tonumber(fjd.year)
    //    local month = tonumber(fjd.month)
    //    local day = tonumber(fjd.day)
    //    local hour = tonumber(fjd.hour)
    //    local min = tonumber(fjd.min)
    //    local sec = tonumber(fjd.sec)
    //
    //    return {
    //        year = year,
    //        month = month,
    //        day = day,
    //        hour = hour,
    //        min = min,
    //        sec = sec,
    //        jd = jd,
    //        mjd = mjd,
    //        gst = gst
    //    }
    //
    //end--END_秒周運動

    public double OverTime(double EpochTime, Date SetTime)
    {/*=== 時間日変換 ===*/
    
        //元期からの経過日数
        double DeltaEpochTime = 0;
        //西暦年
        double E_year = Math.Floor(EpochTime / 1000);
        //経過日数
        double E_day = EpochTime - Math.Floor(E_year * 1000);

        if (57 <= E_year && E_year <= 99)
        {/* 1957年から1999年まで */

            E_year += 1900;

        }
        else /* 2000年以降 */
        {
            E_year += 2000;
        }

        // 構造体型のインスタンス化
        Date E_date; //過去時間
        Date date; //経過時間用

        /* 1年を取得 */
        //年取得
        E_date.Year = E_year;
        //月取得
        E_date.Month = 1;
        //日取得
        E_date.Day = 1;
        //時間取得
        E_date.Hour = 0;
        //分取得
        E_date.Minute = 0;
        //秒取得
        E_date.Second = 0;

        //経過日数
        MotionTime date_Time = DiurnalMotion(E_date, E_day - 1);

        /*構造体変更*/
        //年取得
        date.Year = date_Time.Year;
        //月取得
        date.Month = date_Time.Month;
        //日取得
        date.Day = date_Time.Day;
        //時間取得
        date.Hour = date_Time.Hour;
        //分取得
        date.Minute = date_Time.Minute;
        //秒取得
        date.Second = date_Time.Second;

        double ETJD = GetJD(date);
        double SetJD = GetJD(SetTime);

        //経過JD
        DeltaEpochTime = SetJD - ETJD;
        
        
        return DeltaEpochTime;
    
    
    }/*=== END_時間日変換 ===*/

    public Direction LongitudeLatitude(Vector3 ISS_Pos)
    {/*=== 緯度経度変換 ===*/

        // 構造体型のインスタンス化
        Direction Pos;

        float Longitude; //経度
        float Latitude;  //緯度

        //経度
        float SubLongitude = Mathf.Pow(Mathf.Abs(ISS_Pos.x), 2) + Mathf.Pow(Mathf.Abs(ISS_Pos.y), 2) + Mathf.Pow(Mathf.Abs(ISS_Pos.z), 2);
        Longitude = Mathf.Asin(ISS_Pos.z / Mathf.Sqrt(SubLongitude)) * Mathf.Deg2Rad;

        //-緯度
        Latitude = Mathf.Atan2(ISS_Pos.y, ISS_Pos.x) * Mathf.Deg2Rad;


        //Debug.Log(Latitude);
        //Debug.Log(Longitude);
        //Debug.Log("");

        Pos.E = (double)Latitude;
        Pos.N = (double)Longitude;

        return Pos;

    }/*=== END_緯度経度変換 ===*/


    public double GetGst(Date date)
    {/*=== GetGst ===*/

        double rad = Math.PI / 180;
        double hour = date.Hour;
        double min = date.Minute;
        double sec = date.Second;

        double time_in_sec = hour * 3600 + min * 60 + sec;
        double time_in_day = hour / 24 + min / 1440 + sec / 86400;
        double jd = GetJD(date);
        double jd0 = jd - time_in_day;

        //Greenwich Mean Sidereal Time(GMST) at 0:00
        double t = (jd0 - 2451545.0) / 36525;
        double gmst_at_zero = (24110.5484 + 8640184.812866 * t + 0.093104 * t * t + 0.0000062 * t * t * t) / 3600;
        if (gmst_at_zero > 24)
        {
            gmst_at_zero %= 24;
        }

        //GMST at target time
        double gmst = gmst_at_zero + (time_in_sec * 1.00273790925) / 3600;
        //mean obliquity of the ecliptic
        double e = 23 + 26.0 / 60 + 21.448 / 3600 - 46.8150 / 3600 * t - 0.00059 / 3600 * t * t + 0.001813 / 3600 * t * t * t;
        //Nutation in longitude
        double omega = 125.04452 - 1934.136261 * t + 0.0020708 * t * t + t * t * t / 450000;
        double long1 = 280.4665 + 36000.7698 * t;
        double long2 = 218.3165 + 481267.8813 * t;
        double phai = -17.20 * Math.Sin(omega * rad) - (-1.32 * Math.Sin(2 * long1 * rad)) - 0.23 * Math.Sin(2 * long2 * rad) + 0.21 * Math.Sin(2 * omega * rad);
        //Greenwich Apparent Sidereal Time(GAST / GST)
        double gast = gmst + ((phai / 15) * (Math.Cos(e * rad))) / 3600;
        gast = RoundNum(gast, 24);

        return gast;

    }/*=== END_GetGst ===*/


    public Date JDToUTC(double jd)
    {/*=== JDをUTCに変換 ===*/

        // 構造体型のインスタンス化
        Date date;

        double mjd = jd - 2400000.5;
        double mjd0 = Math.Floor(mjd);
        double flac = mjd - mjd0;
        double n = mjd0 + 678881;
        double a = 4 * n + 3 + 4 * Math.Floor(((double)3 / 4) * Math.Floor(((4 * (n + 1)) / (146097)) + 1));
        double b = 5 * Math.Floor((a % 1461) / 4) + 2;
        double year = Math.Floor(a / 1461);
        double month = Math.Floor(b / 153) + 3;
        double day = Math.Floor((b % 153) / 5) + 1;

        if (month == 13)
        {
            year ++;
            month = 1;
        }

        if (month == 14)
        {
            year++;
            month = 2;
        }

        double h = flac * 24;
        double hour = Math.Floor(h);
        double m = (h - hour) * 60;
        double min = Math.Floor(m);
        double sec = Math.Floor((m - min) * 60);

        //年取得
        date.Year = year;
        //月取得
        date.Month = month;
        //日取得
        date.Day = day;
        //時間取得
        date.Hour = hour;
        //分取得
        date.Minute = min;
        //秒取得
        date.Second = sec;

        return date;

    }/*=== END_JDをUTCに変換 ===*/



    public double GetMoonAge(Date date)
    {/*=== 月齢計算 ===*/

        double Moon = 29.53059;
        double MoonAge = 0.0;
        double year = date.Year;
        double month = date.Month;
        double day = date.Day;
        double hour = date.Hour;
        double min = date.Minute;
        double sec = date.Second;

        /*時間を日に変換*/
        day += hour / 24 + min / 1440 + sec / 86400;

        /* 19年毎に式が変わる*/
        if (1943 <= year && year <= 1961)
        {/* 1943 <= year <= 1961 */
            MoonAge = ((year - 1952) * 11 + month + day) % Moon;
        }
        else if (1962 <= year && year <= 1980)
        {/* 1962 <= year <= 1980 */
            MoonAge = ((year - 1971) * 11 + month + day) % Moon;
        }
        else if (1981 <= year && year <= 1999)
        {/* 1981 <= year <= 1999 */
            MoonAge = ((year - 1990) * 11 + month + day) % Moon;
        }
        else if (2000 <= year)
        {/* 2000年以降 */


            MoonAge = (((year - 2009) % (double)19) * 11 + month + day) % Moon;

            if (month == 1 || month == 2)
            {/* 1月2月補正値 */

                MoonAge += 2;
            
            }/* END_1月2月補正値 */

        }
        else
        {/*=== 上記以外 ===*/
            MoonAge = ((year - 1952) * 11 + month + day) % Moon;
        }

        return MoonAge;

    }/*=== END_月齢計算 ===*/


    public Orbit GetOrbit(OrbitalElements orbital_elements, Date date)
    {
        // 構造体型のインスタンス化
        Orbit orbit;

        /* 軌道要素 */
        //元期(ET)
        double EpochTime = orbital_elements.EpochTime;
        //近地点引数(ω)
        double Argument = orbital_elements.Argument;
        //軌道傾斜角(i)
        double InclinationAngle = orbital_elements.InclinationAngle;
        //昇交点赤経(Ω)
        double AscendingNode = orbital_elements.AscendingNode;
        //離心率(e)
        double Eccentricity = orbital_elements.Eccentricity;
        //平均近点角(M0)
        double MeanAnomaly = orbital_elements.MeanAnomaly;
        //平均運動(M1)
        double MeanMotion = orbital_elements.MeanMotion;
        //平均運動変化係数(M2)
        double MeanCoefficient = orbital_elements.MeanCoefficient;

        //元期からの経過日数
        double DeltaEpochTime = OverTime(EpochTime, date);


        /* 軌道長半径 */
        //平均運動(rev / day)
        double Mm = MeanMotion + MeanCoefficient * DeltaEpochTime;
        //係数GM(km3 / day2)
        double GM = 2.975537 * Mathf.Pow(10, 15);
        //軌道長半径(km)
        double a = Mathf.Pow((float)(GM / ((4 * Mathf.Pow(Mathf.PI, 2)) * Mathf.Pow((float)Mm,2))), ((float)1 / 3));
        /* 離心近点角E(rev) */
        double M = (MeanAnomaly / 360) + (MeanMotion * DeltaEpochTime);
        M = M + (MeanCoefficient / 2) * (Mathf.Pow((float)DeltaEpochTime, 2));

        //整数切り捨て
        double Sub_M = Math.Floor(M);
        M = M - Sub_M;
        double E = M * 360;              //少数に360をかける

        //地球を中心とする人工衛星の三次元座標計算
        double U = a * Mathf.Cos((float)E * Mathf.Deg2Rad) - a * Eccentricity;
        double V = a * Mathf.Sqrt(1 - Mathf.Pow((float)Eccentricity, 2)) * Math.Sin((float)E* Mathf.Deg2Rad);

        //(ω)
        double Omega_Zero = 180 * 0.174 * Mathf.Pow((float)((float)2 - 2.5 * Mathf.Sin((float)InclinationAngle * Mathf.Deg2Rad)), 2);
        double Omega_Fast = Mathf.PI * Mathf.Pow((float)(a / 6371), (float)3.5);
        double Omega = Argument + (Omega_Zero / Omega_Fast) * DeltaEpochTime;

        //(Ω)
        double Ohm_Zero = 180 * 0.174 * Math.Cos((float)InclinationAngle * Mathf.Deg2Rad);
        double Ohm = AscendingNode - (Ohm_Zero / Omega_Fast) * DeltaEpochTime;
        //計算結果用
        double[] xyz_Axle = new double[3];
        xyz_Axle[0] = 0;
        xyz_Axle[1] = 0;
        xyz_Axle[2] = 0;

        double XYZ1 = Mathf.Sin((float)(Omega * Mathf.Deg2Rad)) * U + Mathf.Cos((float)Omega * Mathf.Deg2Rad) * V;
        double XYZ2 = Mathf.Cos((float)Omega * Mathf.Deg2Rad) * U - Mathf.Sin((float)Omega * Mathf.Deg2Rad) * V;
        xyz_Axle[0] = Mathf.Cos((float)Ohm * Mathf.Deg2Rad) * XYZ2 - Mathf.Sin((float)Ohm * Mathf.Deg2Rad) * (Mathf.Cos((float)InclinationAngle * Mathf.Deg2Rad) * XYZ1);
        xyz_Axle[1] = Mathf.Sin((float)Ohm * Mathf.Deg2Rad) * XYZ2 + Mathf.Cos((float)Ohm * Mathf.Deg2Rad) * (Mathf.Cos((float)InclinationAngle * Mathf.Deg2Rad) * XYZ1);
        xyz_Axle[2] = Mathf.Sin((float)InclinationAngle * Mathf.Deg2Rad) * XYZ1;
        //x cosΩ(cosωU-sinωV)-sinΩ(cosi(sinωU + cosωV))
        //y sinΩ(cosωU-sinωV)+cosΩ(cosi(sinωU + cosωV))
        //z sini(sinωU+cosωV)

        //観測赤径角度
        double theta_g = ((360 / 24) * GetGst(date));

        //計算結果用
        double[] XYZ_Axle = new double[3];
        XYZ_Axle[0] = xyz_Axle[0] * Mathf.Cos((float)(-theta_g * Mathf.Deg2Rad)) - xyz_Axle[1] * Mathf.Sin((float)(-theta_g * Mathf.Deg2Rad));
        XYZ_Axle[1] = xyz_Axle[0] * Mathf.Sin((float)(-theta_g * Mathf.Deg2Rad)) + xyz_Axle[1] * Mathf.Cos((float)(-theta_g * Mathf.Deg2Rad));
        XYZ_Axle[2] = xyz_Axle[2];

        //θG = θ0 + 1.002737909∆T
        //X x*cos(-0g) - y * sin(-0g)
        //Y x*sin(-0g) + y * cos(-0g)
        //Z z

        //座標出力
        orbit.OrbitVector3 = new Vector3((float)XYZ_Axle[0], (float)XYZ_Axle[1], (float)XYZ_Axle[2]);
        //高度出力
        orbit.Altitude = a - 6371;
        
        return orbit;

    }




}/* END_TimeControllerクラス */
