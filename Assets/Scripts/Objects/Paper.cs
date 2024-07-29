using CustomUI;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.LookDev;

[System.Serializable]
public class PaperBackgroundInfo{

    public Sprite Sprite;
    public Color Color = new Color(1,1,1,1);
    [Header("��ġ��  : -400 ~ 400 , y: -400 ~ 400")]
    public Vector2 RectPosition;
    public float RotateZ;
    public float SizeX;
    public float SizeY;
}

public class Paper : GetObject
{
    [Header("������ �Է��� �ּ���. Contact By CSS")]
    [TextArea] public string textArea;

    public Color _color = Color.white;
    GameObject go_light;

    //Localization Story/Name
    public string Name;

    //���� �ؽ�Ʈ ������ ������ Ƚ��
    public short iteratorNumber =2;
    short applyIteratorNumber=0;

    //���� ������ �����ֱ� ����
    public string nextLineKey;

    public string noticeKey;

    protected override void Init()
    {
        base.Init();

        //����Ʈ�� ���� ���ӿ�����Ʈ �߰�
        //go_light = new GameObject("LightClone");
        //go_light.transform.parent = transform;
        //go_light.transform.localPosition = new Vector3(0,0.4f,0);


        //����Ʈ �Ӽ� ����
        //Light l = go_light.AddComponent<Light>();
        //l.color = _color;
        //l.intensity = 0.1f;
        //l.range = 0.8f;
        //l.cullingMask = (1 << DataSet.Instance.Layers.Object);

        StartCoroutine(Delay());
    }


    IEnumerator Delay()
    {
        while (true)
        {
            if (LocalLanguageSetting.Instance.LoadEnd()) break;
            yield return null;
        }

        //�⺻ �ؽ�Ʈ ����
        textArea = LocalLanguageSetting.Instance.GetLocalText("Story", Name);
    }

    public override bool GrabOn(Transform parent)
    {
        return false;
    }

    public override bool Func()
    {
        if(!GRAB)
        {
            GameManager.Instance.FrozenOnGame(FrozenOff);
            UI.semiStaticUI.ShowPaper(textArea);
            AudioManager.instance.PlayEffectiveSound("Paper", 1.0f);

            GRAB = true;
        }
        else
        {
            GameManager.Instance.FrozenOffGame();
        }
        return true;
    }

    public virtual void FrozenOff()//�׷������� ���� ���� ��츦 ����� ���
    {
        UI.semiStaticUI.ShowPaper(null);
        GRAB = false;

        if (applyIteratorNumber<iteratorNumber && !nextLineKey.Equals(""))
        {
            UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", nextLineKey), 6.0f);
            UI.staticUI.ShowLine();
            applyIteratorNumber++;
        }

        //Notice ǥ��
        if (!noticeKey.Equals(""))
        {
            UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", noticeKey),true,15.0f);

            noticeKey = "";
        }

        AudioManager.instance.PlayEffectiveSound($"Sigh{Random.Range(0,4)}",1.0f);

        GameManager.Instance.CursorHide();
    }

    public void ChangeLanguage()
    {
        textArea = LocalLanguageSetting.Instance.GetLocalText("Story", Name);

        applyIteratorNumber = iteratorNumber;
    }
}
