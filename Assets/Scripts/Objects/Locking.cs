using CustomUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//자물쇠 역할을 하며 해당 
public class Locking : ExpandObject
{
    public Animation[] step;// 애니메이션 단계
    public float[] playTime;

    public Door door; // 문을 열때 회전각

    AudioSource source;

    protected override void Init()
    {
        base.Init();

        door.isLocked = true;
        source = GetComponent<AudioSource>();
    }

    public override bool Func(string name = null)
    {
        if (!LibraryEvent.Activation)
        {
            Debug.Log("도서관 이벤트가 활성화되지 않았습니다.");

            //지문 표시
            string line = LocalLanguageSetting.Instance.GetLocalText("Tip", "LibraryNotActivation");
            UI.topUI.ShowNotice(line, false);

            return false;
        }

        //상호작용 오브젝트의 이름이 같다면? 상호작용을 합니다.
        if (TargetObjectName.Equals(name))
        {
            gameObject.layer = DataSet.Instance.Layers.Default;
            PlayerEvent.instance.DestroyGrabedItem();
            StartCoroutine(OpenCroutine());
            return true;
        }

        return false;
    }

    IEnumerator OpenCroutine()
    {
        source.Play();
        for (int i = 0; i <  step.Length; i++)
        {
            step[i].Play();
            yield return new WaitForSeconds(playTime[i]);
        }

        yield return new WaitForSeconds(1.0f);

        AudioManager.instance.PlayEffectiveSound("DeepAmbient",1.0f);

        door.isLocked = false;

        Destroy(step[0].gameObject);
        Destroy(step[1].gameObject);

        Destroy(this);
    }
}
