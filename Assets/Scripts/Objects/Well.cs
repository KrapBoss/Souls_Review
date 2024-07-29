using CustomUI;
using System.Collections;
using UnityEngine;


//�칰 ������Ʈ�� �����Ѵ�.
//�칰�� ���� ��ȥ�� ������ ������ ��ȥ�� ���´ٸ� �ٷ� ������ ������ ���°� �ȴ�.

public class Well : ExpandObject
{
    //�̺�Ʈ�� ������ ������Ʈ�� �޾ƿ´�.
    public Transform itemSpawnPoint;

    public Soul _Soul;

    public float upperY = 1.5f;

    bool OntheDish= false;
    GameObject go_Dish;

    private void Update()
    {
        if (OntheDish)
        {
            Debug.Log("Hole : ������ �̺�Ʈ�� ��� ���Դϴ�.");
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

                    Debug.Log("������ �̺�Ʈ�� Ȱ��ȭ���� �ʾҽ��ϴ�.");

                    //���� ǥ��
                    string line = LocalLanguageSetting.Instance.GetLocalText("Tip", "LibraryNotActivation");
                    UI.topUI.ShowNotice(line, false);

                    return;
                }

                //�̹� ������ �̺�Ʈ�� Ȱ��ȭ �Ǿ� �ִ� ���
                go_Dish = collision.gameObject;
                Interaction();

                return;
            }
        }

        //���� ��� ����
        collision.transform.position = itemSpawnPoint.position;
    }

    void Interaction()
    {
        OntheDish = false;

        Debug.Log("�칰 ���� Ȯ��");
        AudioManager.instance.PlayEffectiveSound("DeepAmbient", 1.0f, true);

        Destroy(go_Dish);
        StartCoroutine(SoulUpperCoroutine());
    }

    public override bool Func(string name = null)
    {
        Debug.Log("�칰 ���� Ȯ��2");

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

        Debug.Log("��ȥ ����");

        this.enabled = false;
    }
}

