using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUI
{
    public class ActivityUI : MonoBehaviour
    {

        [Header("아이템에 커서가 가 있다는 걸 나타낸다.")]
        public TMP_Text Text_Interaction;    //상호작용 관련한 UI
        public Image img_MobileCursor;       //모바일 상호작용 지표


        [Header("몬스터가 주변에 있을 경우 피드백 UI")]
        public Image aroundMonster_Image;       // 피드백 이미지
        [Range(0, 255)] public float aroundMonster_maxAlpha = 25;    // 최대 알파값

        [Header("블랙이 UI")]
        public Image black_Image;                                   // 피드백 이미지
        [Range(0, 255)] public float black_maxAlpha = 25;   // 최대 알파값


        [Space]
        [Header("숨참기 게이지")]
        public BreathGage breathGage;

        private void Awake()
        {
            UI.activityUI = this;
        }

        private void Start()
        {
            //알파값 맞추기
            aroundMonster_maxAlpha = aroundMonster_maxAlpha / 255;
            black_maxAlpha =black_maxAlpha / 255;

            //Action
            EventManager.instance.Action_AroundMonster += AroundMonster;

            //커서 비활성화
            PlayerEvent.instance.Action_Init += DeActiveCursor;
            PlayerEvent.instance.Action_Init += ()=>AroundBlack(0);

            //모바일 커서 비활성화
            img_MobileCursor.enabled = false;
            Text_Interaction.enabled = false;

            //Init
            AroundMonster(0);
            AroundBlack(0);
        }

        //Throwing = 0,      //단순히 던지는 오브젝트
        //Get,                //획득하는 오브젝트
        //Self,               //스스로 상호작용이 가능함. // musicBox
        //Expand,             //다른 오브젝트들과 상호작용 가능 //잡을 수 없습니다.
        //Continues,          //지속적인 상호작용을 해야 되는 오브젝트
        //EquipAuto,           //소지한 상태에서 지속적인 자동으로 작동한다.
        //None                //사용할 수 없는 상태가 되었을 때 표시하지 않는다.
        // 초점 표시
        public void SetInteractionUI(Vector3 _position, bool _isExpand = false, short _type = -1)  // 상호작용 표시를 나타내준다.
        {
            //모바일 커서
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
                            //Debug.Log(" Expand :: 동일한 오브젝트 이므로 상호작용이 가능합니다.");
                            Text_Interaction.text = "[ E ]";
                        }
                        else
                        {
                            //Debug.Log(" Expand :: 동일한 오브젝트가 아니기에 불가능합니다.");
                            Text_Interaction.text = "[ X ]";
                        }
                        break;
                    default:
                        //X 표시
                        Text_Interaction.text = "[ X ]";
                        break;
                }


                if (_type >= 0)// 활성화 시켜라
                {
                    Text_Interaction.enabled = true;
                    Text_Interaction.rectTransform.anchoredPosition = Camera.main.WorldToScreenPoint(_position) - new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);
                }
                else        //비활성화 시켜라
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

        //몬스터 주변에 올 경우 보여줄 피드백 이벤트\
        public void AroundMonster(float ratio)
        {
            if (aroundMonster_Image.gameObject.activeSelf)
            {
                Color _c = aroundMonster_Image.color;
                _c.a = ratio * aroundMonster_maxAlpha;
                if (_c.a != aroundMonster_Image.color.a)    // 알파값이 다를 경우에만 변경이 일어난다. float 비교연산자는 근접치를 이용한다.
                {
                    aroundMonster_Image.color = _c;
                }
            }
        }

        //블랙이가 주위에 있을 경우
        public void AroundBlack(float ratio)
        {
            if (black_Image.gameObject.activeSelf)
            {
                Color _c = black_Image.color;
                _c.a = ratio * black_maxAlpha;
                if (_c.a != black_Image.color.a)    // 알파값이 다를 경우에만 변경이 일어난다. float 비교연산자는 근접치를 이용한다.
                {
                    black_Image.color = _c;
                }
            }
        }


        //현재 숨쉬기 비율을 넘겨받아 게이지를 설정한다.
        public void SetBreathGage(float t, bool can)
        {
            breathGage.SetGage(t, can);
        }
    }
}

