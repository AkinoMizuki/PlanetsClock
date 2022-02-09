///<summary>
///
/// GetSatellite
/// rev：
/// 
/// ver.0.0.0.0
/// 2022/02/08
/// 
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public partial class GetSatellite : MonoBehaviour
{/*=== GetSatellite ===*/

    //人工衛星データ配列
    public string[,,] Stations = new string[1, 1, 1];

    public IEnumerator GetWeb_Satellitet()
    {/*=== 人工衛星データ取得 ===*/

        //web接続確認
        UnityWebRequest www_Google = UnityWebRequest.Get("https://www.google.com");
        yield return www_Google.SendWebRequest();

        if (www_Google.isNetworkError || www_Google.isHttpError)
        {/* web接続確認 */

            Debug.Log(www_Google.error);

        }
        else
        {/* webにつながってる */

            //人工衛星データ取得
            UnityWebRequest www_Orbit = UnityWebRequest.Get("http://www.celestrak.com/NORAD/elements/stations.txt");
            yield return www_Orbit.SendWebRequest();

            if (www_Orbit.isNetworkError || www_Orbit.isHttpError)
            {/*=== 人工衛星データ取得 ===*/

                //取得失敗
                Debug.Log(www_Orbit.error);
            }
            else
            {

                /*=== データフォルダー作成 ===*/
                var stationsData_Folde = "./StationsData";

                if (!Directory.Exists(stationsData_Folde))
                {/*=== stationsData_Foldeのフォルダー作成 ===*/
                    Directory.CreateDirectory(stationsData_Folde);
                }/*=== END_stationsData_Foldeのフォルダー作成 ===*/
                else
                {/*=== stationsData_Foldeのフォルダーの再作成 ===*/
                    Debug.Log("deleted of 'StationsData' folder.");
                    //ファイル全消去
                    Delete(stationsData_Folde);
                    //ファイル作成
                    Directory.CreateDirectory(stationsData_Folde);
                }/*=== END_stationsData_Foldeのフォルダーの再作成 ===*/


                // 結果をテキストとして表示します
                Debug.Log(www_Orbit.downloadHandler.text);
                string txt = www_Orbit.downloadHandler.text;
                StreamWriter sw = new StreamWriter("./StationsData/stations.txt", false);// TextData.txtというファイルを新規で用意
                sw.WriteLine(txt);// ファイルに書き出したあと改行
                sw.Flush();// StreamWriterのバッファに書き出し残しがないか確認
                sw.Close();// ファイルを閉じる

            }/*=== END_人工衛星データ取得 ===*/


        }/* END_web接続確認 */

        //ファイルOpen
        Stations = OpenStations();

    }/*=== END_人工衛星データ取得 ===*/



    /* 人工衛星データ読み込みキャッシュ */
    public string[,,] OpenStations()
    {/* 人工衛星データ読み込みキャッシュ */

        
        string[,,] Index_Stations;
        Index_Stations = new string[1, 1, 1];

        //例
        //ISS(ZARYA)
        //1 25544U 98067A   22038.37836806  .00007168  00000+0  13432-3 0  9993
        //2 25544  51.6416 259.4336 0006553 105.8037 251.0375 15.49806680325044

        //配列構成
        //Index_Stations[0,0,0] = "ISS(ZARYA)";
        //Index_Stations[0,0,1] = null;
        //Index_Stations[0,0,2] = null;
        //Index_Stations[0,0,3] = null;
        //Index_Stations[0,0,4] = null;
        //Index_Stations[0,0,5] = null;
        //Index_Stations[0,0,6] = null;
        //Index_Stations[0,0,7] = null;
        //Index_Stations[0,0,8] = null;

        //Index_Stations[0,1,0] = "1";
        //Index_Stations[0,1,1] = "25544U";
        //Index_Stations[0,1,2] = "98067A";
        //Index_Stations[0,1,3] = "22038.37836806";
        //Index_Stations[0,1,4] = ".00007168";
        //Index_Stations[0,1,5] = "00000+0";
        //Index_Stations[0,1,6] = "13432-3";
        //Index_Stations[0,1,7] = "0";
        //Index_Stations[0,1,8] = "9993";

        //Index_Stations[0,2,0] = "2";
        //Index_Stations[0,2,1] = "25544";
        //Index_Stations[0,2,2] = "51.6416";
        //Index_Stations[0,2,3] = "259.4336";
        //Index_Stations[0,2,4] = "0006553";
        //Index_Stations[0,2,5] = "105.8037";
        //Index_Stations[0,2,6] = "251.0375";
        //Index_Stations[0,2,7] = "15.49806680325044";
        //Index_Stations[0,2,8] = null;


        /*=== データフォルダー確認 ===*/
        var stationsData_Folde = "./StationsData";

        if (Directory.Exists(stationsData_Folde))
        {/*=== データフォルダー ===*/

            if (File.Exists(stationsData_Folde + "/stations.txt"))
            {/* ==== ファイルが既に存在する ==== */

                string[] lines = File.ReadAllLines(stationsData_Folde + "/stations.txt");
                Index_Stations = new string[lines.Length, 3,9];

                int StationsCount = -1;
                int StationsFast = 0;
                int StationsSecond = 0;

                //foreach (string line in lines)
                for(int SubCount = 0; lines.Length - 2 >= SubCount; SubCount++)
                {

                    string first = lines[SubCount].Substring(0, 2);
                    if (first == "1 ")
                    {/* 配列スイッチ */
                        StationsFast = 1;
                        lines[SubCount] = lines[SubCount].Replace("   ", " ");
                        lines[SubCount] = lines[SubCount].Replace("  ", " ");
                        lines[SubCount] = lines[SubCount].Replace(" ", ",");
                    }
                    else if (first == "2 ")
                    {
                        StationsFast = 2;
                        lines[SubCount] = lines[SubCount].Replace("   ", " ");
                        lines[SubCount] = lines[SubCount].Replace("  ", " ");
                        lines[SubCount] = lines[SubCount].Replace(" ", ",");
                    }
  
                    else
                    {
                        StationsCount++;
                        StationsFast = 0;
                    }/* END_配列スイッチ */

                    /*1文字目が1の場合*/
                    /*1文字目が2の場合*/
                    if (StationsFast == 1 || StationsFast == 2)
                    {
                        if (lines[SubCount].IndexOf(',') == -1)
                            continue;
                        string[] parts = lines[SubCount].Split(',');
                        if (parts.Length < 2)
                            continue;
                        
                        for(int SubCount_2 = 0; parts.Length - 1 >= SubCount_2; SubCount_2++)
                        {
                            /* Debug.Log("Index_Stations[" + StationsCount + "," + StationsFast + "," + SubCount_2 + "]"); */
                            Index_Stations[StationsCount, StationsFast, SubCount_2] = parts[SubCount_2];
                            /* Debug.Log(Index_Stations[StationsCount, StationsFast, SubCount_2].ToString()); */


                        }
                    }
                    else
                    {
                        Index_Stations[StationsCount, StationsFast, 0] = lines[SubCount];
                        /* Debug.Log(Index_Stations[StationsCount, StationsFast, 0].ToString()); */
                    }


                }
            }
            else
            {

                Debug.Log("Don't Get file 'stations.txt'");
                Index_Stations[0, 0, 0] = "stations.txt";

            }/* ==== END_ファイルが既に存在する ==== */

        }
        else
        {

            Debug.Log("Don't Get folder 'StationsData'");
            Index_Stations[0, 0, 0] = "StationsData";

        }/*=== END_データフォルダー ===*/

        return Index_Stations;

    }/* END_人工衛星データ読み込みキャッシュ */

    public string[,,] SendStations()
    {/* 人工衛星配列データを要求 */

        if (Stations[0, 0, 0] == null)
        {
            Stations[0, 0, 0] = "StationsData";
        }
        return Stations;
    }/* END_人工衛星配列データを要求 */


    /*********************************************************************
    *   ファイルの消去
    *********************************************************************/
    public void Delete(string targetDirectoryPath)
    {/*=== 指定したディレクトリとその中身を全て削除する ===*/

        try
        {
            if (!Directory.Exists(targetDirectoryPath))
            {
                return;
            }

            /*=== ディレクトリ以外の全ファイルを削除 ===*/
            string[] filePaths = Directory.GetFiles(targetDirectoryPath);
            foreach (string filePath in filePaths)
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
                Debug.Log("Delete : \"" + filePath + "\"");
                File.Delete(filePath);
            }

            /*=== ディレクトリの中のディレクトリも再帰的に削除 ===*/
            string[] directoryPaths = Directory.GetDirectories(targetDirectoryPath);
            foreach (string directoryPath in directoryPaths)
            {
                Debug.Log("Delete : \"" + directoryPath + "\"");
                Delete(directoryPath);
            }

            /*=== 中が空になったらディレクトリ自身も削除 ===*/
            Debug.Log("Delete : \"" + targetDirectoryPath + "\"");
            Directory.Delete(targetDirectoryPath, false);
        }
        catch
        {
            Debug.Log("Error : Not delete \"" + targetDirectoryPath + "\"");
            return;
        }

    }/*=== 指定したディレクトリとその中身を全て削除する ===*/

}/*=== END_GetSatellite ===*/
