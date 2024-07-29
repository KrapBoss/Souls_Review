using CustomUI;
using StarterAssets;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using Random = UnityEngine.Random;

/// <summary>
/// �輱 �ڽ� UI�� ���� ��ũ��Ʈ
/// </summary>
public class WireBox : MonoBehaviour
{
    //���� Ȱ�� ����
    //public static bool Active;
    //���� ���� ����
    public static short SuccessCount =0;

    //������ ���� ����
    public Image img_Boundary;
    //�����̴� ��
    public Image img_dot;
    //���������� ��� �ִ� �θ�
    public RectTransform rect_GageParent;
    //���� ȸ���ϴ� �ӵ�
    public float dotSpeed = 3.0f;
    private float _applyDotSpeed;

    //����
    public AudioClip clip_CheckIn;
    public AudioClip clip_CheckOut;
    public AudioSource source;

    //��ư
    public Button btn;

    public float offsetBoundary = 20.0f;

    public GameObject Icon_Text;

    //���������� ȸ�� ��ų �ݰ�
    private float _randomAngle;
    //������ ���� ����
    private float _randomRange;
    //�����̴� ���� ����
    private float _dotAngle;
    //���� Ƚ��
    private int _successCount = 0;
    //���� Ƚ��
    private int _failedCount = 0;

    private float[] rangeBoundary = new float[] { 0.18f, 0.12f, 0.07f };

    StarterAssetsInputs _inputs;

    //�������� �������� ��� ������ �̺�Ʈ�� ������.
    Action<bool> _successAction;

    private void OnEnable()
    {
        //Active = true;

        //������� ��� ��Ʈ�� �г� ��Ȱ��ȭ
        if (!GameConfig.IsPc())
        {
            UI.mobileControllerUI.SetActive(false);

            //��ư �̺�Ʈ Ȱ��ȭ
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(ButtonAction);

            Icon_Text.SetActive(false);
        }
        else
        {
            //���콺 �����ֱ�
            //GameManager.Instance.CursorShow();

            //��ư ��Ȱ��ȭ
            btn.gameObject.SetActive(false);

            Icon_Text.SetActive(true);
        }

        //�ʱ�ȭ
        _successCount = 0;
        _failedCount = 0;
        dotSpeed = DataSet.Instance.GameDifficulty.WireBoxDotSpeed;
       _applyDotSpeed = dotSpeed;
        RandomBoundary();


        if (_inputs == null)
            _inputs = FindObjectOfType<StarterAssetsInputs>();
    }

    private void OnDisable()
    {
        //Active = false;
        //���콺 �����
        GameManager.Instance.CursorHide();

        //Ȱ��ȭ
        if (!GameConfig.IsPc())
        {
            UI.mobileControllerUI.SetActive(true);
        }
    }

    private void Update()
    {
        DotRotate();

        // ����
        if (_inputs.Grab)
        {
            ButtonAction();
            _inputs.Grab = false;
        }
    }

    public void ButtonAction()
    {
        if (CheckInRange())
        {
            Debug.Log("������ ���� ����");
            CheckInSuccess();
        }
        else
        {
            Debug.Log("������ ���� ����");
            CheckInFailed();
        }
    }


    //���� ������ �������� �����մϴ�.
    void RandomBoundary()
    {
        _randomAngle = Random.Range(80.0f, 360.0f);
        _randomRange = rangeBoundary[_successCount];
        _dotAngle = Random.Range(0.0f, 360.0f);

        img_Boundary.fillAmount = _randomRange;
        rect_GageParent.rotation = Quaternion.Euler(0, 0, _randomAngle);

        float x = ((rect_GageParent.rect.width * .3f) + offsetBoundary) * Mathf.Cos(Mathf.Deg2Rad * _dotAngle) + Screen.width * 0.5f;
        float y = ((rect_GageParent.rect.height * .3f) + offsetBoundary) * Mathf.Sin(Mathf.Deg2Rad * _dotAngle) + Screen.height * 0.5f;
        img_dot.rectTransform.position = new Vector3(x, y, 0);
    }

    //������ ���� �ȿ� ���� �����ϴ� ��츦 �Ǵ�.
    bool CheckInRange()
    {
        float s =_randomAngle ;
        float e = _randomAngle - 360.0f * _randomRange;

        float angle = (_dotAngle +  90.0f) % 360.0f;
        if (s >= angle && e <= angle)
            return true;
        else
           return false;
    }

    void DotRotate()
    {
        _dotAngle = (_dotAngle + Time.unscaledDeltaTime * _applyDotSpeed) % 360.0f;
        float x = ((rect_GageParent.rect.width * 0.35f) + offsetBoundary) * Mathf.Cos(Mathf.Deg2Rad*_dotAngle) + Screen.width * 0.5f;
        float y = ((rect_GageParent.rect.height * 0.35f) + offsetBoundary) * Mathf.Sin(Mathf.Deg2Rad * _dotAngle) + Screen.height * 0.5f;
        img_dot.rectTransform.position = new Vector3(x, y, 0);
    }

    void CheckInSuccess()
    {
        _successCount++;
        //�� 3�� ����
        if(_successCount > 2)
        {
            Debug.Log("���� ������ �Ϸ�Ǿ����ϴ�");

            //���� ���� Ƚ�� ����
            SuccessCount++;

            //������ ���� �̺�Ʈ ����
            if (_successAction != null) _successAction(true);

            //���� ���� ����
            GameManager.Instance.FrozenOffGame();
        }
        else
        {
            //������ ȿ��
            StartCoroutine(GageScaleChangeCroutine(_successCount));

            //���� �� ����`
            source.PlayOneShot(clip_CheckIn);

            //�ٽ� ��ġ
            RandomBoundary();

            //���� ���� �ѱ��
            EventManager.instance.EventSound(PlayerEvent.instance.transform, 99.9f);

            //�ӵ� ����
            _applyDotSpeed *= 1.5f;
        }
    }

    //Time�� ���ѵ��� �ʴ� ������ ���� ����Ʈ
    IEnumerator GageScaleChangeCroutine(int repeat)
    {
        for(int i=0;i<repeat; i++)
        {
            float timer = 0.0f;

            rect_GageParent.localScale = new Vector3(0.8f, 0.8f);

            while (timer < 0.03f)
            {
                timer  += Time.unscaledDeltaTime;
                yield return null;
            }
            timer = 0;

            rect_GageParent.localScale = new Vector3(1.2f, 1.2f); 

            while (timer < 0.03f)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            timer = 0;

            rect_GageParent.localScale = new Vector3(0.9f, 0.9f);

            while (timer < 0.03f)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            timer = 0;

            rect_GageParent.localScale = new Vector3(1.0f, 1.0f);

            while (timer < 0.03f)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
        }
    }

    //�������� �������� ���
    void CheckInFailed()
    {
        _failedCount++;

        Fade.FadeSetting(false, 0.3f, new Color(0.7f, 0.0f ,0.0f));

        // ġ��  ���� ����
        source.PlayOneShot(clip_CheckOut);

        //3�� �����ߴ°�?
        if (_failedCount > 2)
        {
            EventManager.instance.EventSound(PlayerEvent.instance.transform, 50.0f);

            //���� ���� Ǯ��
            GameManager.Instance.FrozenOffGame();

            if (_successAction != null) _successAction(false);

            //ȭ�� �Ӱ�
            Fade.FadeSetting(true, 0.5f, new Color(0.3f, .3f, 1.0f));

            //���� �Ҹ�
            AudioManager.instance.PlayEffectiveSound("Tinnitus", 1.0f);

            //����
            PlayerEvent.instance.PlayerFaint(ShowText);
        }
        else
        {
            RandomBoundary();
            StartCoroutine(GageScaleChangeCroutine(_failedCount *3));
        }
    }

    public void ShowText()
    {
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Tip", $"WireBox{Random.Range(0, 3)}"), 3.0f);
        UI.staticUI.ShowLine();
    }

    //���� â�� �ݽ��ϴ�.
    public void Close()
    {
        Debug.Log("�������� �ݽ��ϴ�.");
        this.gameObject.SetActive(false);
    }

    public void SetEvent(Action<bool> action)
    {
        _successAction = action;
    }
}
