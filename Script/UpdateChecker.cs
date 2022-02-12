///<summary>
///
/// UpdateChecker
/// rev：Unity
/// 
/// ver.1.0.0.0
/// 2022/02/12
/// 
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Ping = System.Net.NetworkInformation.Ping;

public partial class UpdateChecker : MonoBehaviour
{/*=== 更新チェッカー  ===*/

    private class ini_Settings
    {/*=== 設定用変数 ===*/

        /*********************************************************************
        *   定義
        *********************************************************************/

        /*=== 設定ファイル ===*/
        public static string ConfFile = "Updater.config";
        /*=== ログファイル名 ===*/
        public string LogFile = "UpdateError.log";
        /*=== アクセス先がwebサーバーかチェック ===*/
        public static bool WebServer = false;
        /*=== サーバーのURL ===*/
        public static string IpAddress = "";
        /*=== サーバーのダウンロードディレクトリー ===*/
        public static string FilePass = "";
        /*=== Uploadするソフトの名前 ===*/
        public static string ExeNeme = "";
        /*=== Revision管理ファイルの名前 ===*/
        public static string NewRevFile = "";
        /*=== 消去除外フォルダー ===*/
        public static string KeepFolder = "";
        /*=== Uploadするソフトの再起動確認 ===*/
        public static bool SoftReStart = false;
        /*=== pingウェイト用_ms ===*/
        public static int PingWait = 0;


        /*=== 設定無し ===*/
        /*=== 通信確認用 ===*/
        public static bool IPContest = false;
        /*=== バージョン確認用 ===*/
        public static bool VerCheckResult = false;//まだ未使用


        public void GetConfig(string UrlAddress)
        {/*=== インポート開始 ===*/

            try
            {

                UrlAddress += @"\";

                if (File.Exists(UrlAddress + ConfFile))
                {/*=== 設定ファイル読み込み開始 ===*/

                    /* XmlSerializerオブジェクトを作成 */
                    System.Xml.Serialization.XmlSerializer serializer =
                        new System.Xml.Serialization.XmlSerializer(typeof(UpdaterConfig.configuration));
                    /* 読み込むファイルを開く */
                    StreamReader sr = new StreamReader(
                        ConfFile, new System.Text.UTF8Encoding(false));
                    /* XMLファイルから読み込み、逆シリアル化する */
                    UpdaterConfig.configuration obj = (UpdaterConfig.configuration)serializer.Deserialize(sr);
                    /* ファイルを閉じる */
                    sr.Close();

                    /*=== アクセス先がwebサーバーかチェック ===*/
                    WebServer = obj.WebServer;
                    /*=== サーバーのURL ===*/
                    IpAddress = obj.IpAddress;
                    /*=== サーバーのダウンロードディレクトリー ===*/
                    FilePass = obj.FilePass;
                    /*=== Uploadするソフトの名前 ===*/
                    ExeNeme = obj.ExeNeme;
                    /*=== Revision管理ファイルの名前 ===*/
                    NewRevFile = obj.NewRevFile;
                    /*=== 消去除外フォルダー ===*/
                    KeepFolder = obj.KeepFolder;
                    /*=== Uploadするソフトの再起動確認 ===*/
                    SoftReStart = obj.SoftReStart;
                    /*=== pingウェイト用_ms ===*/
                    PingWait = obj.PingWait;

                }//*=== END_設定ファイル読み込み開始 ===*/
                else
                {/*=== 設定ファイルが無い ===*/

                    Debug.Log("Error : \"" + UrlAddress + ConfFile + "\" could not be found.");

                }/*=== END_設定ファイルが無い ===*/

            }
            catch (Exception)
            {
                Debug.Log("Error : Failed to import \"" + UrlAddress + ConfFile + "\"."
                    + Environment.NewLine
                    + "Please take a look at ConfFile.");

            }

        }/*=== END_インポート開始 ===*/

    }/*=== END_設定用変数 ===*/

    public class CheckProgram
    {/*=== 更新チェッカー ===*/

        /********************************************************************
        *  オブジェクト作成
        ********************************************************************/
        /*=== 設定用変数 ===*/
        ini_Settings ini_Settings = new ini_Settings();
        Logger logger = new Logger();

        private static HttpClient client = new HttpClient();

        /*********************************************************************
        *   アップデートチェック
        *********************************************************************/
        public bool UpdateCheck(string UpdateUrl, string version)
        {/*=== アップデートチェック ===*/

            /*=== エラーログ定義 ===*/
            var LogFolde = "./UpdateCheckLog/";

            if (!Directory.Exists(LogFolde))
            {/*=== OutputLogのフォルダー作成 ===*/
                Directory.CreateDirectory(LogFolde);
            }/*=== END_OutputLogのフォルダー作成 ===*/
            else
            {/*=== OutputLogのフォルダーの再作成 ===*/
                Delete(LogFolde);
                Directory.CreateDirectory(LogFolde);
            }/*=== END_OutputLogのフォルダーの再作成 ===*/

            ini_Settings.GetConfig(UpdateUrl);

            /*=== ソフトの現状確認 ===*/

            logger.log("");

            /*=== サーバーとの通信確認 ===*/
            logger.log("Process : Server connection check");
            logger.log("");
            if (ServerPing() == false)
            {
                logger.log("Message : Not connected to network.");
                logger.log("Process : The program has been terminated.");
                return false;
            }

            logger.log("Message : The network was able to confirm the connection.");
            logger.log("");
            /*=== END_サーバーとの通信確認 ===*/


            /*=== ソフトの最新ver確認 ===*/
            if (ini_Settings.WebServer == true)
            {

                Uri New_FilePass = new Uri("http://" + ini_Settings.IpAddress + "/" + ini_Settings.FilePass + "/" + ini_Settings.NewRevFile);
                Task<string> webTask = GetWebPageAsync(New_FilePass);
                webTask.Wait();
                var New_version = webTask.Result;

                if (New_version == null)
                {
                    New_version = "Not";
                }

                if (New_version == "")
                {
                    New_version = "Not";
                }

                if (New_version == "Not")
                {
                    logger.log("Process : The program has been terminated.");
                    return false;
                }

                /*=== ソフトの比較 ===*/
                if (false == isNew(version, New_version))
                {
                    logger.log("Result : Ver. \"" + version + "\"is the new.");
                    logger.log("Message : Finish checking for updates.");
                    logger.log("Process : The program has been terminated.");
                    return false;
                }

                logger.log("Message : Update because there is the new ver." + New_version);
                logger.log("Do you want to update ?");
                logger.log("");
            }
            else
            {
                var New_version = NweVersion();
                if (New_version == "Not")
                {
                    logger.log("Process : The program has been terminated.");
                    return false;
                }

                /*=== ソフトの比較 ===*/
                if (false == isNew(version, New_version))
                {
                    logger.log("Result : Ver. \"" + version + "\"is the new.");
                    logger.log("Message : Finish checking for updates.");
                    logger.log("Process : The program has been terminated.");
                    return false;
                }

                logger.log("Message : Update because there is the new ver." + New_version);
                logger.log("Do you want to update ?");
                logger.log("");

            }

#if !DEBUG
         /*=== エラーログの消去 ===*/
           //Directory.Delete(LogFolde, true);
#endif

            return true;

        }/*=== END_アップデートチェック ===*/


        /*********************************************************************
        *   ソフトの現状確認
        *********************************************************************/
        public string ExeAsmCheck(string LogURL)
        {/*=== ソフトの現状確認 ===*/

            LogURL += @"\";

            // ファイル名を指定して、そのファイルのバージョンを取得する方法
            var filename = LogURL + ini_Settings.ExeNeme + ".exe";
            if (File.Exists(filename))
            {
                FileVersionInfo filever = FileVersionInfo.GetVersionInfo(filename);
                logger.log(ini_Settings.ExeNeme + " : ver." + filever.FileVersion);
                return filever.FileVersion;
            }
            else
            {
                return "";
            }
        }/*=== END_ソフトの現状確認 ===*/

        /*********************************************************************
        *   ローカル通信確認
        *********************************************************************/

        public bool ServerPing()
        {/*=== 通信確認 ===*/

            try
            {

                logger.log("IP Address : " + ini_Settings.IpAddress);
                logger.log("Ping Send Wait Time : " + ini_Settings.PingWait + "ms");
                Ping sender = new Ping();

                PingReply reply = sender.Send(ini_Settings.IpAddress);
                if (reply.Status == IPStatus.Success)
                {
                    logger.IP_TTLlog("Reply from {0}: bytes={1} time={2}ms TTL={3}",
                        reply.Address,
                        reply.Buffer.Length,
                        reply.RoundtripTime,
                        reply.Options.Ttl);

                    if (reply.Options.Ttl >= 1)
                    {
                        ini_Settings.IPContest = true;
                    }
                    else
                    {
                        ini_Settings.IPContest = false;
                    }

                }
                else
                {
                    ini_Settings.IPContest = false;
                    logger.IPlog(reply.Status);
                }


            }
            catch
            {
                logger.log("Exception : Make sure the IP or network connection is correct.");
                ini_Settings.IPContest = false;
            }

            return ini_Settings.IPContest;

        }/*=== END_通信確認 ===*/

        /*********************************************************************
        *   web通信確認(まだ変えていない)
        *********************************************************************/
        public bool web_ServerPing()
        {/*=== web_通信確認 ===*/

            try
            {

                logger.log("IP Address : " + ini_Settings.IpAddress);
                logger.log("Ping Send Wait Time : " + ini_Settings.PingWait + "ms");
                Ping sender = new Ping();

                PingReply reply = sender.Send(ini_Settings.IpAddress);
                if (reply.Status == IPStatus.Success)
                {
                    logger.IP_TTLlog("Reply from {0}: bytes={1} time={2}ms TTL={3}",
                        reply.Address,
                        reply.Buffer.Length,
                        reply.RoundtripTime,
                        reply.Options.Ttl);

                    if (reply.Options.Ttl >= 1)
                    {
                        ini_Settings.IPContest = true;
                    }
                    else
                    {
                        ini_Settings.IPContest = false;
                    }

                }
                else
                {
                    ini_Settings.IPContest = false;
                    logger.IPlog(reply.Status);
                }


            }
            catch
            {
                logger.log("Exception : Make sure the IP or network connection is correct.");
                ini_Settings.IPContest = false;
            }

            return ini_Settings.IPContest;

        }/*=== END_web_通信確認 ===*/

        /*********************************************************************
        *   最新バージョン情報の取得
        *********************************************************************/
        public string NweVersion()
        {/*=== 最新バージョン情報の取得 ===*/

            var New_FilePass = @"\\" + ini_Settings.IpAddress + @"\" + ini_Settings.FilePass + @"\";
            var Text = string.Empty;

            /*=== フォルダーの確認 ===*/
            /* 元パスの存在確認 */
            if (Directory.Exists(New_FilePass))
            {/* 元パスは存在する */

                /*=== 最新バージョンリストの確認 ===*/
                if (File.Exists(New_FilePass + ini_Settings.NewRevFile))
                {/* 元フォルダは存在する */

                    using (var reader = new StreamReader(New_FilePass + ini_Settings.NewRevFile))
                    {
                        Text = reader.ReadToEnd();
                    }

                    if (Text == null)
                    {
                        Text = "Not";
                    }

                    if (Text == "")
                    {
                        Text = "Not";
                    }


                }/* END_元フォルダは存在する */
                else
                {/* 元フォルダが無い */

                    logger.log("Error : Missing file. \"" + ini_Settings.NewRevFile + "\"");
                    Text = "Not";

                }/* END_元フォルダが無い */

            }/* END_元パスは存在する */
            else
            {/* 元パスが無い */

                logger.log("Error : Missing folder path. \"" + New_FilePass + "\"");
                Text = "Not";

            }/* END_元パスが無い */

            return Text;
        }/*=== END_最新バージョン情報の取得 ===*/

        /*********************************************************************
        *   web版_最新バージョン情報の取得
        *********************************************************************/

        private static async Task<string> GetWebPageAsync(Uri new_FilePass)
        {/*=== 最新バージョン情報の取得 ===*/

            // タイムアウトをセット（オプション）
            client.Timeout = TimeSpan.FromSeconds(5);

            try
            {
                // Webページを取得する
                return await client.GetStringAsync(new_FilePass);
            }
            catch (HttpRequestException e)
            {
                // 404エラーや、名前解決失敗など
                Console.WriteLine("An exception such as 404 occurred!");
                // InnerExceptionも含めて、再帰的に例外メッセージを表示する
                Exception ex = e;
                while (ex != null)
                {
                    Console.WriteLine("Exception message: {0} ", ex.Message);
                    ex = ex.InnerException;
                }
            }
            catch (TaskCanceledException e)
            {
                // タスクがキャンセルされたとき（一般的にタイムアウト）
                Console.WriteLine("Time out");
                Console.WriteLine("Exception message: {0} ", e.Message);
            }

            return null;

        }/*=== END_最新バージョン情報の取得 ===*/

        /*********************************************************************
        *   最新バージョン情報の取得
        *********************************************************************/
        public bool isNew(string current, string target)
        {/*=== バージョン確認 ===*/
            var ca = current.Split('.');
            var ta = target.Split('.');
            var len = Math.Min(ca.Length, ta.Length);

            for (var i = 0; i < len; i++)
            {
                int ci, ti;
                if (!int.TryParse(ca[i], out ci) | !int.TryParse(ta[i], out ti))
                {
                    return false;
                }

                if (ci < ti)
                {
                    return true;
                }
                if (ci > ti)
                {
                    return false;
                }
            }

            return ca.Length < ta.Length;
        }/*=== END_バージョン確認 ===*/

        /*********************************************************************
        *   ログ
        *********************************************************************/
        interface ILogger
        {/*=== ログインターフェイス ===*/
            void log(String msg);
        }/*=== END_ログインターフェイス ===*/

        public class Logger : ILogger
        {/*=== ログ ===*/

            /*=== プログラム処理を生成 ===*/
            ini_Settings ini_Settings = new ini_Settings();

            public static String path = "";
            public static String userProfilePath = "";
            public const String encodingName = "UTF-8";
            public StreamWriter sw = null;

            public Logger()
            {
                /*=== エラーログ定義 ===*/
                var LogFolde ="./UpdateCheckLog/";
                userProfilePath = LogFolde;
                String name = ini_Settings.LogFile;
                path = userProfilePath + "\\" + name;

            }

            public void log(String msg)
            {
                if (Directory.Exists(userProfilePath))
                {

                    sw = new StreamWriter(path, true, System.Text.Encoding.GetEncoding(encodingName));

                    Console.WriteLine(msg);
                    sw.WriteLine(msg);
                    sw.Close();

                }
                else
                {
                    Console.WriteLine(msg);
                }
            }

            internal void IPlog(IPStatus status)
            {
                string IPstatus = status.ToString();
                log(status.ToString());

            }

            internal void IP_TTLlog(string v, IPAddress address, int length, long roundtripTime, int ttl)
            {

                string TTLlog = "Reply from " + address
                    + ": bytes=" + length
                    + " time=" + roundtripTime + "ms"
                    + " TTL=" + ttl;

                log(TTLlog);
            }
        }/*=== END_ログ ===*/


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
                    logger.log("Delete : \"" + filePath + "\"");
                    File.Delete(filePath);
                }

                /*=== ディレクトリの中のディレクトリも再帰的に削除 ===*/
                string[] directoryPaths = Directory.GetDirectories(targetDirectoryPath);
                foreach (string directoryPath in directoryPaths)
                {
                    logger.log("Delete : \"" + directoryPath + "\"");
                    Delete(directoryPath);
                }

                /*=== 中が空になったらディレクトリ自身も削除 ===*/
                logger.log("Delete : \"" + targetDirectoryPath + "\"");
                Directory.Delete(targetDirectoryPath, false);
            }
            catch
            {
                logger.log("Error : Not delete \"" + targetDirectoryPath + "\"");
                return;
            }

        }/*=== 指定したディレクトリとその中身を全て削除する ===*/



    }/*=== END_ 更新チェッカー ===*/




}/*=== END_更新チェッカー  ===*/
