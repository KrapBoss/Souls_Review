using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
/// <summary>
/// 목적 : 아이템을 담고 모두 담게 되면 챙겨서 나가야 되는 것
/// </summary>
/// 

[System.Serializable]
class BoxItem
{
    public string Name;
    public GameObject obj;
}

public class ItemBox : GetObject
{
    //담아야 되는 물건들의 이름이다.
    [SerializeField]BoxItem[] items;

    public GameObject Cover;

    //닫혀있는 박스의 컬라이더.
    public BoxCollider coll_closedBox;

    bool completed;

    protected override void Init()
    {
        base.Init();
        for(int i = 0; i < items.Length; i++) { items[i].obj.SetActive(false); }

        gameObject.layer = DataSet.Instance.Layers.Default;
        Rigidbody.isKinematic = true;
        coll_closedBox.enabled = false;
    }

    public override bool Func()
    {
        return false;
    }

    //충돌된 오브젝트를 판단하여 아이템을 활성화시킨다.
    private void OnTriggerEnter(Collider other)
    {
        if(completed) return;

        if(other.gameObject.layer == DataSet.Instance.Layers.Object)
        {
            InteractionObjects _interObj = other.GetComponent<InteractionObjects>();

            for (int i = 0; i < items.Length; i++)
            {
                if (_interObj.NAME.Equals(items[i].Name) && !_interObj.GRAB)
                {
                    items[i].obj.SetActive(true);

                    Destroy(_interObj.gameObject);

                    //아이템 놓는 소리

                }
            }
        }

        if (IsCompleted())//모든 오브젝트가 활성화 되어있는가?
        {
            Debug.Log("모든 오브젝트를 담았습니다.");
            completed = true;
            StartCoroutine(CoverCloseCroutine());
        }
    }

    IEnumerator CoverCloseCroutine()
    {
        float t = 0;
        float x = Cover.transform.localRotation.eulerAngles.x;
        //Debug.LogWarning($"{Cover.transform.localRotation.eulerAngles}");

        while (t<1.0f)
        {
            t += Time.deltaTime;
            Cover.transform.localRotation = Quaternion.Euler(Mathf.Lerp(x, 359.9f,t),0,0);
            //Debug.Log($"{Cover.transform.rotation.eulerAngles}");

            yield return null;
        }

        //다음 미션완료를 알리고 다음 미션을 보여줍니다.
        _progressObject.ActionObject();

        //상자의 컬라이더를 변경하며, 물리효과를 활성화시킵니다.
        BoxCollider[] _coll = GetComponents<BoxCollider>();
        for(int i =0; i< _coll.Length; i++)
        {
            _coll[i].enabled = false;
        }
        coll_closedBox.enabled = true;
        Rigidbody.isKinematic = false;

        //레이어를 변경해 상자를 들 수 있게 해줍니다.
        gameObject.layer = gameObject.layer = DataSet.Instance.Layers.GetLayerTypeToLayer(MyLayer);

        //상자 닫히는 소리
    }

    //모든 오브젝트가 활성화 되어있는지 판단한다.
    bool IsCompleted()
    {
        for(int i=0;i< items.Length;i++)
        {
            if (!items[i].obj.activeSelf) return false;
        }
        return true;
    }
}
