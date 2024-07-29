using CustomUI;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.LookDev;

[System.Serializable]
public class PaperBackgroundInfo{

    public Sprite Sprite;
    public Color Color = new Color(1,1,1,1);
    [Header("위치값  : -400 ~ 400 , y: -400 ~ 400")]
    public Vector2 RectPosition;
    public float RotateZ;
    public float SizeX;
    public float SizeY;
}

public class Paper : GetObject
{
    [Header("내용을 입력해 주세요. Contact By CSS")]
    [TextArea] public string textArea;

    public Color _color = Color.white;
    GameObject go_light;

    //Localization Story/Name
    public string Name;

    //다음 텍스트 지문을 보여줄 횟수
    public short iteratorNumber =2;
    short applyIteratorNumber=0;

    //다음 지문을 보여주기 위함
    public string nextLineKey;

    public string noticeKey;

    protected override void Init()
    {
        base.Init();

        //라이트를 담을 게임오브젝트 추가
        //go_light = new GameObject("LightClone");
        //go_light.transform.parent = transform;
        //go_light.transform.localPosition = new Vector3(0,0.4f,0);


        //라이트 속성 설정
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

        //기본 텍스트 설정
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

    public virtual void FrozenOff()//그랩해제가 되지 않을 경우를 대비한 방법
    {
        UI.semiStaticUI.ShowPaper(null);
        GRAB = false;

        if (applyIteratorNumber<iteratorNumber && !nextLineKey.Equals(""))
        {
            UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", nextLineKey), 6.0f);
            UI.staticUI.ShowLine();
            applyIteratorNumber++;
        }

        //Notice 표시
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
