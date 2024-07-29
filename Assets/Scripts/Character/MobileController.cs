using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUI
{
    public class MobileControllerUI : MonoBehaviour
    {
        //�ٸ� ������ Ȱ��ȭ�� ��찡 ���� ���� ����.
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
        public Sprite texture_Drop; //ī�޶� ��� ���� ���� ���� �ٲ�� ����

        private void Awake()
        {
            //����� �� ��� Ȱ��ȭ��Ŵ.
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

            //�⺻������ ��Ȱ��ȭ

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

        //�÷��̾� ���� �ʱ�ȭ
        public void SetDefault()
        {
            ClickStateSet(MobileClickState.Grab);
            CurrentState = MobileClickState.Grab;

            //����ϱ� ���̴� �� ����
            Func.gameObject.SetActive(false);
        }

        //����ϱ� E ��ư�� ���̴��� ����
        public void ShowFunc(bool active)
        {
            if (active != Func.gameObject.activeSelf)
            {
                Func.gameObject.SetActive(active);
            }
        }

        //ī�޶� ������ �̺�Ʈ
        public void CameraEquip(bool equip)
        {
            if (equip) ClickStateSet(MobileClickState.Take);
            else ClickStateSet(MobileClickState.Grab);
        }

        //��Ȱ��ȭ�� Ȱ��ȭ
        public void SetActive(bool active)
        {
            this.gameObject.SetActive(active);
        }

        public void TapFunc()
        {
            inputSystem.TapInput(true);
        }


        //�ش� UI�� Ȱ��ȭ �մϴ�.
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
                    Debug.Log("�Է��� ��ư�� ã�� �� �����ϴ�.");
                    break;
            }
        }

        //��� ������Ʈ �������� ��Ȱ��ȭ�մϴ�.
        public void AllDeactiveItem()
        {
            V.gameObject.SetActive(false);
            One.gameObject.SetActive(false);
            Two.gameObject.SetActive(false);
        }
    }
}
