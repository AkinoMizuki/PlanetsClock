
using UnityEngine;
using UnityEngine.UI;
using static ChangeButton;


public class ChangeButton : MonoBehaviour
{/*=== �\���ύX�{�^�� ===*/

    [SerializeField]
    //On_�{�^��Collar
    private Color32 On_Color = new Color32(0, 100, 100, 255);
    [SerializeField]
    //Off_�{�^��Collar
    private Color32 Off_Color = new Color32(0, 0, 100, 255);

    //�Ǘ��t���O
    public bool ChangeFlag;// { get; set; }
    //�\���ؑփI�u�W�F�N�g
    public GameObject[] Change_obj;  
    
    //�{�^���I�u�W�F�N�g
    public Button Button_obj;

    void Start()
    {/* �X�^�[�g�֐� */
    
        //�\���̏�����
        //ChangeFlag = true;

        //�{�^���̐F�ύX
        ChangeButtonCollar(ChangeFlag);

    }/* �X�^�[�g�֐� */
    
    public void OnClick_ChangeButton()
    {/*=== �\���ύX���s ===*/
    
        ChangeFlag = !ChangeFlag;
    
        //�{�^���̐F�ύX
        ChangeButtonCollar(ChangeFlag);
    
    }/*=== END_�\���ύX���s ===*/
    
    private void ChangeButtonCollar(bool CollarFlag)
    {/*=== �{�^���̐F�ύX ===*/

        if (!(Change_obj.Length == null))
        {//�I�u�W�F�N�g�ύX

            for (int Change_Count = 0; Change_Count < Change_obj.Length; Change_Count++)
            {
                //�A�N�e�B�u�ύX
                Change_obj[Change_Count].SetActive(ChangeFlag);
            }

        }//END_�I�u�W�F�N�g�ύX

        if (CollarFlag == true)
        {//ON�J���[�ɕύX

            Button_obj.image.color = On_Color;
        }
        else
        {//OFF�J���[�ɕύX

            Button_obj.image.color = Off_Color;

        }//END_�J���[�ύX

    }/*=== �{�^���̐F�ύX ===*/

}/*=== END_�\���ύX�{�^�� ===*/

