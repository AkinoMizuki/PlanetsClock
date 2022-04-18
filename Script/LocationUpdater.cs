using UnityEngine;
using System.Collections;

public partial class LocationUpdater : MonoBehaviour
{
    private float IntervalSeconds = 1.0f;
    private static LocationServiceStatus Status { get; set; }
    private static LocationInfo Location { get; set; }

    public IEnumerator Start()
    {

        while (true)
        {
            Status = Input.location.status;
            if (Input.location.isEnabledByUser)
            {
                switch (Status)
                {
                    case LocationServiceStatus.Stopped:
                        Input.location.Start();
                        // Wait until service initializes
                        int maxWait = 20;
                        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
                        {
                            yield return new WaitForSeconds(1);
                            maxWait--;
                        }
                        break;
                    case LocationServiceStatus.Running:
                        Location = Input.location.lastData;
                        break;
                    default:
                        break;
                }
            }
            //else
            //{
            //    // FIXME 位置情報を有効にして!! 的なダイアログの表示処理を入れると良さそう
            //    Debug.Log("location is disabled by user");
            //}

            // 指定した秒数後に再度判定を走らせる
            yield return new WaitForSeconds(IntervalSeconds);
        }
    }

    public string[] GetLocation()
    {
        string[] locationInformation = new string[4];

        if (Status.ToString() != null)
        {
            locationInformation[0] = Status.ToString();
            locationInformation[1] = Location.latitude.ToString();
            locationInformation[2] = Location.longitude.ToString();
            locationInformation[3] = Location.altitude.ToString();
        }
        else
        {
            locationInformation[0] = "non";
            locationInformation[1] = "0";
            locationInformation[2] = "0";
            locationInformation[3] = "0";
        }

        return locationInformation;
    }
}