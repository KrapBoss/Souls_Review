using CustomUI;
using System.Collections;
using UnityEngine;

public class SafeBox : ExpandObject
{
    public static bool Activation= false;

    public ActiveRandomObjects safeBox;
    public Transform Door;

    public GameObject fx;

    bool isWrong = false; // 잘못된 값들을 입력했을 경우
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

        //도서관 이벤트가 활성되지 않은 경우
        if (!LibraryEvent.Activation)
        {
            //사용할 수 없음
            UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", "LibraryNotActivation"), false);
            return false;
        }

        if (!GRAB)   //활성화시킨다.
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
        Debug.Log($"패스워드를 판단합니다 : {pass[0]} / {pass[1]}");

        if (pass[0] == safeBox.activeObjects[0].ActivatedNum && pass[1] == safeBox.activeObjects[1].ActivatedNum)
        {
            Activation = true;

            Debug.Log("금고 상호작용에 성공하셨습니다.");
            AudioManager.instance.PlayEffectiveSound("DeepAmbient", 1.0f);
            EffectManager.instance.ActiveParticle("ScreenDistortion", transform.position);
            gameObject.layer = DataSet.Instance.Layers.Default; //레이어를 변경해서 감지가 불가능하게 한다.

            AudioManager.instance.PlayEffectiveSound("DeepAmbient", 1.0f);
            EventManager.instance.EventSound(transform, 8.0f);

            fx.SetActive(true);

            UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story","SafeBoxClear"),10.0f);
            UI.staticUI.ShowLine();

            UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", "SafeboxClear"),true, 15.0f);

            StartCoroutine(Croutine());
        }
        else//금고의 값이 다르다.
        {
            isWrong = true;

            //경고음
            _source.Play();

            EventManager.instance.EventSound(transform, 15.0f);

            //다시 입력 가능한 상태로 변경
            GRAB = false;

            Debug.Log("금고 :: 입력을 잘못했습니다.");
            Invoke("WrongTimeOut", 3.0f);
        }
    }

    IEnumerator Croutine()//금고문이 열린다.
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

        Debug.Log("금고 :: 잘못된 입력 초기화");
    }
}
