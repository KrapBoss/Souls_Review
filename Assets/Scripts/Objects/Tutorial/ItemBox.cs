using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
/// <summary>
/// ���� : �������� ��� ��� ��� �Ǹ� ì�ܼ� ������ �Ǵ� ��
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
    //��ƾ� �Ǵ� ���ǵ��� �̸��̴�.
    [SerializeField]BoxItem[] items;

    public GameObject Cover;

    //�����ִ� �ڽ��� �ö��̴�.
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

    //�浹�� ������Ʈ�� �Ǵ��Ͽ� �������� Ȱ��ȭ��Ų��.
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

                    //������ ���� �Ҹ�

                }
            }
        }

        if (IsCompleted())//��� ������Ʈ�� Ȱ��ȭ �Ǿ��ִ°�?
        {
            Debug.Log("��� ������Ʈ�� ��ҽ��ϴ�.");
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

        //���� �̼ǿϷḦ �˸��� ���� �̼��� �����ݴϴ�.
        _progressObject.ActionObject();

        //������ �ö��̴��� �����ϸ�, ����ȿ���� Ȱ��ȭ��ŵ�ϴ�.
        BoxCollider[] _coll = GetComponents<BoxCollider>();
        for(int i =0; i< _coll.Length; i++)
        {
            _coll[i].enabled = false;
        }
        coll_closedBox.enabled = true;
        Rigidbody.isKinematic = false;

        //���̾ ������ ���ڸ� �� �� �ְ� ���ݴϴ�.
        gameObject.layer = gameObject.layer = DataSet.Instance.Layers.GetLayerTypeToLayer(MyLayer);

        //���� ������ �Ҹ�
    }

    //��� ������Ʈ�� Ȱ��ȭ �Ǿ��ִ��� �Ǵ��Ѵ�.
    bool IsCompleted()
    {
        for(int i=0;i< items.Length;i++)
        {
            if (!items[i].obj.activeSelf) return false;
        }
        return true;
    }
}
