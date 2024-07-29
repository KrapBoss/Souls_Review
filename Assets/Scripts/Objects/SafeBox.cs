using CustomUI;
using System.Collections;
using UnityEngine;

public class SafeBox : ExpandObject
{
    public static bool Activation= false;

    public ActiveRandomObjects safeBox;
    public Transform Door;

    public GameObject fx;

    bool isWrong = false; // �߸��� ������ �Է����� ���
    AudioSource _source;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 15);
    }

    protected override void Init()
    {
        base.Init();

        fx.SetActive(false);
        _source = GetComponent<AudioSource>();
    }

    public override bool GrabOn(Transform parent)
    {
        return false;
    }

    public override bool Func(string name = null)
    {
        if(isWrong) return false;

        //������ �̺�Ʈ�� Ȱ������ ���� ���
        if (!LibraryEvent.Activation)
        {
            //����� �� ����
            UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", "LibraryNotActivation"), false);
            return false;
        }

        if (!GRAB)   //Ȱ��ȭ��Ų��.
        {
            AudioManager.instance.PlayUISound("Select", 1.0f);
            UI.semiStaticUI.ShowEnterPassword(CheckPassword);
            GRAB = true;
        }
        else
        {
            AudioManager.instance.PlayUISound("Check", 1.0f);
            UI.semiStaticUI.HideEnterPassword();
            GRAB = false;
        }

        return true;
    }

    public void CheckPassword(int[] pass)
    {
        Debug.Log($"�н����带 �Ǵ��մϴ� : {pass[0]} / {pass[1]}");

        if (pass[0] == safeBox.activeObjects[0].ActivatedNum && pass[1] == safeBox.activeObjects[1].ActivatedNum)
        {
            Activation = true;

            Debug.Log("�ݰ� ��ȣ�ۿ뿡 �����ϼ̽��ϴ�.");
            AudioManager.instance.PlayEffectiveSound("DeepAmbient", 1.0f);
            EffectManager.instance.ActiveParticle("ScreenDistortion", transform.position);
            gameObject.layer = DataSet.Instance.Layers.Default; //���̾ �����ؼ� ������ �Ұ����ϰ� �Ѵ�.

            AudioManager.instance.PlayEffectiveSound("DeepAmbient", 1.0f);
            EventManager.instance.EventSound(transform, 8.0f);

            fx.SetActive(true);

            UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story","SafeBoxClear"),10.0f);
            UI.staticUI.ShowLine();

            UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", "SafeboxClear"),true, 15.0f);

            StartCoroutine(Croutine());
        }
        else//�ݰ��� ���� �ٸ���.
        {
            isWrong = true;

            //�����
            _source.Play();

            EventManager.instance.EventSound(transform, 15.0f);

            //�ٽ� �Է� ������ ���·� ����
            GRAB = false;

            Debug.Log("�ݰ� :: �Է��� �߸��߽��ϴ�.");
            Invoke("WrongTimeOut", 3.0f);
        }
    }

    IEnumerator Croutine()//�ݰ��� ������.
    {
        float ratio = 3.0f;
        float t =0.0f;
        while (t<1.0f)
        {
            t += Time.deltaTime * ratio;
            Door.localRotation = Quaternion.Euler(new Vector3(0,Mathf.Lerp(0,-161.0f, t),0));

            if (t > 0.7f) ratio = 1;
            else if (t > 0.4f) ratio = 2;

            yield return null;
        }

        safeBox.SetDestroy();

        yield return new WaitForSeconds(10.0f);
        Destroy(fx);
    }

    void WrongTimeOut() { 
        
        isWrong =  false; 
        GRAB = false;

        Debug.Log("�ݰ� :: �߸��� �Է� �ʱ�ȭ");
    }
}
