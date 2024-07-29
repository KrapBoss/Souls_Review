using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUI
{
    public class ActivityUI : MonoBehaviour
    {

        [Header("�����ۿ� Ŀ���� �� �ִٴ� �� ��Ÿ����.")]
        public TMP_Text Text_Interaction;    //��ȣ�ۿ� ������ UI
        public Image img_MobileCursor;       //����� ��ȣ�ۿ� ��ǥ


        [Header("���Ͱ� �ֺ��� ���� ��� �ǵ�� UI")]
        public Image aroundMonster_Image;       // �ǵ�� �̹���
        [Range(0, 255)] public float aroundMonster_maxAlpha = 25;    // �ִ� ���İ�

        [Header("���� UI")]
        public Image black_Image;                                   // �ǵ�� �̹���
        [Range(0, 255)] public float black_maxAlpha = 25;   // �ִ� ���İ�


        [Space]
        [Header("������ ������")]
        public BreathGage breathGage;

        private void Awake()
        {
            UI.activityUI = this;
        }

        private void Start()
        {
            //���İ� ���߱�
            aroundMonster_maxAlpha = aroundMonster_maxAlpha / 255;
            black_maxAlpha =black_maxAlpha / 255;

            //Action
            EventManager.instance.Action_AroundMonster += AroundMonster;

            //Ŀ�� ��Ȱ��ȭ
            PlayerEvent.instance.Action_Init += DeActiveCursor;
            PlayerEvent.instance.Action_Init += ()=>AroundBlack(0);

            //����� Ŀ�� ��Ȱ��ȭ
            img_MobileCursor.enabled = false;
            Text_Interaction.enabled = false;

            //Init
            AroundMonster(0);
            AroundBlack(0);
        }

        //Throwing = 0,      //�ܼ��� ������ ������Ʈ
        //Get,                //ȹ���ϴ� ������Ʈ
        //Self,               //������ ��ȣ�ۿ��� ������. // musicBox
        //Expand,             //�ٸ� ������Ʈ��� ��ȣ�ۿ� ���� //���� �� �����ϴ�.
        //Continues,          //�������� ��ȣ�ۿ��� �ؾ� �Ǵ� ������Ʈ
        //EquipAuto,           //������ ���¿��� �������� �ڵ����� �۵��Ѵ�.
        //None                //����� �� ���� ���°� �Ǿ��� �� ǥ������ �ʴ´�.
        // ���� ǥ��
        public void SetInteractionUI(Vector3 _position, bool _isExpand = false, short _type = -1)  // ��ȣ�ۿ� ǥ�ø� ��Ÿ���ش�.
        {
            //����� Ŀ��
            if (!GameConfig.IsPc())
            {
                if (_type >= 0)
                {
                    img_MobileCursor.enabled = true;
                }
                else
                {
                    img_MobileCursor.enabled = false;
                }
            }
            else
            {
                switch (_type)
                {
                    case (short)InteractionType.Throwing:
                    case (short)InteractionType.EquipAuto:
                        Text_Interaction.text = "[ Click ]";
                        break;

                    case (short)InteractionType.Self:
                    case (short)InteractionType.Get:
                    case (short)InteractionType.Continues:
                        Text_Interaction.text = "[ E ]";
                        break;

                    case (short)InteractionType.Expand:
                        if (_isExpand)
                        {
                            //Debug.Log(" Expand :: ������ ������Ʈ �̹Ƿ� ��ȣ�ۿ��� �����մϴ�.");
                            Text_Interaction.text = "[ E ]";
                        }
                        else
                        {
                            //Debug.Log(" Expand :: ������ ������Ʈ�� �ƴϱ⿡ �Ұ����մϴ�.");
                            Text_Interaction.text = "[ X ]";
                        }
                        break;
                    default:
                        //X ǥ��
                        Text_Interaction.text = "[ X ]";
                        break;
                }


                if (_type >= 0)// Ȱ��ȭ ���Ѷ�
                {
                    Text_Interaction.enabled = true;
                    Text_Interaction.rectTransform.anchoredPosition = Camera.main.WorldToScreenPoint(_position) - new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);
                }
                else        //��Ȱ��ȭ ���Ѷ�
                {
                    Text_Interaction.enabled = false;
                }
            }
        }

        public void DeActiveCursor()
        {
            Text_Interaction.enabled = false;
            img_MobileCursor.enabled = false;
        }

        //���� �ֺ��� �� ��� ������ �ǵ�� �̺�Ʈ\
        public void AroundMonster(float ratio)
        {
            if (aroundMonster_Image.gameObject.activeSelf)
            {
                Color _c = aroundMonster_Image.color;
                _c.a = ratio * aroundMonster_maxAlpha;
                if (_c.a != aroundMonster_Image.color.a)    // ���İ��� �ٸ� ��쿡�� ������ �Ͼ��. float �񱳿����ڴ� ����ġ�� �̿��Ѵ�.
                {
                    aroundMonster_Image.color = _c;
                }
            }
        }

        //���̰� ������ ���� ���
        public void AroundBlack(float ratio)
        {
            if (black_Image.gameObject.activeSelf)
            {
                Color _c = black_Image.color;
                _c.a = ratio * black_maxAlpha;
                if (_c.a != black_Image.color.a)    // ���İ��� �ٸ� ��쿡�� ������ �Ͼ��. float �񱳿����ڴ� ����ġ�� �̿��Ѵ�.
                {
                    black_Image.color = _c;
                }
            }
        }


        //���� ������ ������ �Ѱܹ޾� �������� �����Ѵ�.
        public void SetBreathGage(float t, bool can)
        {
            breathGage.SetGage(t, can);
        }
    }
}

