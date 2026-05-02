using CustomUI;
using System.Collections;
using UnityEngine;


//우물 오브젝트를 정의한다.
//우물에 갇힌 영혼이 나오는 것으로 영혼이 나온다면 바로 성불이 가능한 상태가 된다.

public class Well : ExpandObject
{
    //이벤트를 진행할 오브젝트를 받아온다.
    public Transform itemSpawnPoint;

    public Soul _Soul;

    public float upperY = 1.5f;

    bool OntheDish= false;
    GameObject go_Dish;

    private void Update()
    {
        if (OntheDish)
        {
            Debug.Log("Hole : 도서관 이벤트를 대기 중입니다.");
            if (LibraryEvent.Activation)
            {
                Interaction();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (DataSet.Instance.Layers.IsObject(collision.gameObject.layer))
        {
            if (TargetObjectName.Equals(collision.gameObject.GetComponent<InteractionObjects>()?.NAME))
            {
                if (!LibraryEvent.Activation)
                {
                    OntheDish = true;
                    go_Dish = collision.gameObject;

                    Debug.Log("도서관 이벤트가 활성화되지 않았습니다.");

                    //지문 표시
                    string line = LocalLanguageSetting.Instance.GetLocalText("Tip", "LibraryNotActivation");
                    UI.topUI.ShowNotice(line, false);

                    return;
                }

                //이미 도서관 이벤트가 활성화 되어 있는 경우
                go_Dish = collision.gameObject;
                Interaction();

                return;
            }
        }

        //지정 장소 스폰
        collision.transform.position = itemSpawnPoint.position;
    }

    void Interaction()
    {
        OntheDish = false;

        Debug.Log("우물 동작 확인");
        AudioManager.instance.PlayEffectiveSound("DeepAmbient", 1.0f, true);

        Destroy(go_Dish);
        StartCoroutine(SoulUpperCoroutine());
    }

    public override bool Func(string name = null)
    {
        Debug.Log("우물 동작 확인2");

        return true;
    }

    IEnumerator SoulUpperCoroutine()
    {
        float ratio = 0.0f;
        Vector3 pos = _Soul.transform.localPosition;
        Vector3 offset = new Vector3(0, upperY,0);

        _Soul.ActiveMusicbox(null, true);

        while (ratio < 1.0f)
        {
            ratio += Time.deltaTime * 0.5f;
            _Soul.transform.localPosition = Vector3.Lerp(pos, pos + offset, ratio);

            yield return null;
        }

        Debug.Log("영혼 등장");

        this.enabled = false;
    }
}

