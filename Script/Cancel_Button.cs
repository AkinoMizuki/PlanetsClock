using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cancel_Button : MonoBehaviour
{/*=== キャンセルボタン ===*/

    /*=== アップデートウィンドウ ===*/
    public GameObject Update_Transform;         //アップデートウィンドウ

    public void On_Click()
    {/*=== アップデート通知を閉じる ===*/

        //アクティブ変更
        Update_Transform.SetActive(false);

    }/*=== END_アップデート通知を閉じる ===*/

}/*=== END_キャンセルボタン ===*/
