using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUI
{
    public class MobileControllerUI : MonoBehaviour
    {
        //다른 곳에서 활성화할 경우가 생길 때를 위해.
        //public static MobileControllerUI instance;

        StarterAssetsInputs inputSystem;

        public Button One;
        public Button Two;
        public Button Func;
        public Button Esc;
        public Button Shift;
        public Button V;
        public Button Click;
        public Button Sit;
        public Image img_Click;

        public Sprite texture_Grab;
        public Sprite texture_Picture;
        public Sprite texture_Drop; //카메라 들고 있을 때는 놓고 바뀌도록 변경

        private void Awake()
        {
            //모바일 일 경우 활성화시킴.
            if (GameConfig.IsPc())
            {
                Destroy(gameObject);
                gameObject.SetActive(false);
            }
            else gameObject.SetActive(true);

            if (UI.mobileControllerUI == null) { UI.mobileControllerUI = this; }
            else { Destroy(UI.mobileControllerUI); UI.mobileControllerUI = this; }
        }

        // Start is called before the first frame updatew
        void Start()
        {
            if (UI.mobileControllerUI == null) { return; }

            inputSystem = FindObjectOfType<StarterAssetsInputs>();

            if (inputSystem != null)
            {
                One.onClick.AddListener(() => inputSystem.OneInput(true));
                Two.onClick.AddListener(() => inputSystem.TwoInput(true));
                Func.onClick.AddListener(() => inputSystem.FuntionInput(true));
                Esc.onClick.AddListener(() => inputSystem.ESCInput(true));
                V.onClick.AddListener(() => inputSystem.CameraViewInput(true));
                Click.onClick.AddListener(() => inputSystem.GrabInput(true));
                //Sit.onClick.AddListener(() => inputSystem.HoldOnBreathInput(true));
            }

            //기본적으로 비활성화

            PlayerEvent.instance.Action_Init += SetDefault;

            SetDefault();

            EventManager.instance.Action_CameraEquip += CameraEquip;
        }

        public void ShiftFunc(bool press)
        {
            Debug.Log($"Shift {press}");
            inputSystem.RunInput(press);
        }

        public void CtrlFunc(bool press)
        {
            Debug.Log($"Ctrl {press}");
            inputSystem.HoldOnBreathInput(press);
        }

        public enum MobileClickState
        {
            Grab, Drop, Take
        }

        MobileClickState CurrentState;
        public void ClickStateSet(MobileClickState state)
        {
            if (CurrentState.Equals(state)) { return; }

            switch (state)
            {
                case MobileClickState.Grab:
                    img_Click.sprite = texture_Grab;
                    break;
                case MobileClickState.Drop:
                    img_Click.sprite = texture_Drop;
                    break;
                case MobileClickState.Take:
                    img_Click.sprite = texture_Picture;
                    break;
            }

            CurrentState = state;
        }

        //플레이어 상태 초기화
        public void SetDefault()
        {
            ClickStateSet(MobileClickState.Grab);
            CurrentState = MobileClickState.Grab;

            //사용하기 보이는 것 제거
            Func.gameObject.SetActive(false);
        }

        //사용하기 E 버튼이 보이는지 결정
        public void ShowFunc(bool active)
        {
            if (active != Func.gameObject.activeSelf)
            {
                Func.gameObject.SetActive(active);
            }
        }

        //카메라 장착시 이벤트
        public void CameraEquip(bool equip)
        {
            if (equip) ClickStateSet(MobileClickState.Take);
            else ClickStateSet(MobileClickState.Grab);
        }

        //비활성화와 활성화
        public void SetActive(bool active)
        {
            this.gameObject.SetActive(active);
        }

        public void TapFunc()
        {
            inputSystem.TapInput(true);
        }


        //해당 UI를 활성화 합니다.
        public void ActiveIcon(int num, bool active)
        {
            switch (num)
            {
                case 0:
                    V.gameObject.SetActive(active);
                    break;
                case 1: // meter
                    One.gameObject.SetActive(active);
                    break;
                case 2://ball
                    Two.gameObject.SetActive(active);
                    break;
                default:
                    Debug.Log("입력한 버튼을 찾을 수 없습니다.");
                    break;
            }
        }

        //모든 오브젝트 아이콘을 비활성화합니다.
        public void AllDeactiveItem()
        {
            V.gameObject.SetActive(false);
            One.gameObject.SetActive(false);
            Two.gameObject.SetActive(false);
        }
    }
}
