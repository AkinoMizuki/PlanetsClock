
using UnityEngine;
using UnityEngine.UI;
using static ChangeButton;


public class ChangeButton : MonoBehaviour
{/*=== 表示変更ボタン ===*/

    [SerializeField]
    //On_ボタンCollar
    private Color32 On_Color = new Color32(0, 100, 100, 255);
    [SerializeField]
    //Off_ボタンCollar
    private Color32 Off_Color = new Color32(0, 0, 100, 255);

    //管理フラグ
    public bool ChangeFlag;// { get; set; }
    //表示切替オブジェクト
    public GameObject[] Change_obj;  
    
    //ボタンオブジェクト
    public Button Button_obj;

    void Start()
    {/* スタート関数 */
    
        //表示の初期化
        //ChangeFlag = true;

        //ボタンの色変更
        ChangeButtonCollar(ChangeFlag);

    }/* スタート関数 */
    
    public void OnClick_ChangeButton()
    {/*=== 表示変更実行 ===*/
    
        ChangeFlag = !ChangeFlag;
    
        //ボタンの色変更
        ChangeButtonCollar(ChangeFlag);
    
    }/*=== END_表示変更実行 ===*/
    
    private void ChangeButtonCollar(bool CollarFlag)
    {/*=== ボタンの色変更 ===*/

        if (!(Change_obj.Length == null))
        {//オブジェクト変更

            for (int Change_Count = 0; Change_Count < Change_obj.Length; Change_Count++)
            {
                //アクティブ変更
                Change_obj[Change_Count].SetActive(ChangeFlag);
            }

        }//END_オブジェクト変更

        if (CollarFlag == true)
        {//ONカラーに変更

            Button_obj.image.color = On_Color;
        }
        else
        {//OFFカラーに変更

            Button_obj.image.color = Off_Color;

        }//END_カラー変更

    }/*=== ボタンの色変更 ===*/

}/*=== END_表示変更ボタン ===*/

