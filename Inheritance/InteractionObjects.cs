using CustomUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum InteractionType
{
    Throwing = 0,      //�ܼ��� ������ ������Ʈ
    Get,                //ȹ���ϴ� ������Ʈ
    Self,               //������ ��ȣ�ۿ��� ������. // musicBox
    Expand,             //�ٸ� ������Ʈ��� ��ȣ�ۿ� ���� //���� �� �����ϴ�.
    Continues,          //�������� ��ȣ�ۿ��� �ؾ� �Ǵ� ������Ʈ
    EquipAuto,           //������ ���¿��� �������� �ڵ����� �۵��Ѵ�.
    None = -1                //����� �� ���� ���°� �Ǿ��� �� ǥ������ �ʴ´�.
}


//�⺻ ������Ʈ ������ ���Ͽ�
public abstract class InteractionObjects : MonoBehaviour
{
    [Header("���� ������Ʈ �̸��� �־��")]
    public string NAME = "EnterTheItemName";             //�������� �̸�

    [Header("���� ������Ʈ ��ȣ�ۿ� Ÿ��")]
    protected InteractionType TYPE;    //�������� �̺�Ʈ Ÿ��
    [Header("���� ������Ʈ�� �׷��� �����ΰ�?")]
    public bool GRAB;               //�׷��� ���ϰ� �ִ�.
    [Header("�׷� �� ������Ʈ ��ġ��")]
    public Vector3 offset;//�ش� ������Ʈ�� �׷� �� ����Ǵ� y�� ������
    [Header("�׷� �� ������Ʈ Rotation")]
    public Vector3 offset_rotation;//�ش� ������Ʈ�� �׷� �� ����Ǵ� rotation
    [Header("�׷��� ���̾ ���� �����ų ������Ʈ")]
    public GameObject[] Childs = null;
    [Header("�⺻ ���̾ ����")]
    public LayerType MyLayer = LayerType.Object;
    [Header("�θ� ������ ������ ��� �ش� �θ� �Ʒ��� �����˴ϴ�.")]
    public Transform Parent;
    [Header("�ش� Ư�� �����۸� ��� ���� �� ���� ������ ��")]
    public bool anchor = false;

    //[Header("��ȣ�ۿ� ������Ʈ �߽� ǥ�� ��ǥ ������Ʈ")]
    Transform PivotObject = null;
    //[Header("���� ������Ʈ�� ������")]
    protected Rigidbody Rigidbody = null;     //������ ������Ʈ ������ �ٵ�

    protected ProgressObject _progressObject = null;//���࿡ ������ ���ϴ� ������Ʈ�ΰ�� null�� �ƴ�

    private void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        Rigidbody = GetComponent<Rigidbody>();

        //������ �����ϴ� �ǹ�������Ʈ�� ã�´�.
        PivotObject = transform.Find("Pivot")?.transform;
        gameObject.layer = DataSet.Instance.Layers.GetLayerTypeToLayer(MyLayer);  //�������� ���̾�� ����
        if (Childs != null)
        {
            for(int i = 0; i < Childs.Length; i++)
            {
                Childs[i].layer = DataSet.Instance.Layers.GetLayerTypeToLayer(MyLayer);
            }
        }
    }

    //��ȣ�ۿ� Ű�� ���� ��
    public virtual bool Func() { return false; }        //True�� ��� �������� ����ϸ� �׷��� �����Ѵ�.
    public virtual bool Func(string name = null) { return false; }
    public virtual void Func(float v) {
        // Debug.Log("�� �Է� ���� �⺻ �Լ�");
                                       }

    public InteractionType GetInteractionType() { return TYPE; }

    public virtual bool GrabOn(Transform parent)        // �÷��̾�� ������.
    {
        //Ȯ���� �Ǵ� ��Ȱ��ȭ �������� ��� �׷��� �Ұ����ϴ�.
        if (TYPE.Equals(InteractionType.Expand) || TYPE.Equals(InteractionType.None) || anchor) return false;

        ShowText();
        GRAB = true;

        AudioManager.instance.PlayEffectiveSound("GrabItem", 1.0f);

        if (Rigidbody)
        {
            Rigidbody.isKinematic = true;
        }

        transform.parent = parent;
        transform.localPosition = -offset;
        transform.localRotation = Quaternion.Euler(offset_rotation);
        gameObject.layer = DataSet.Instance.Layers.OverlayObject;  //�������� ���̾�� ����
        if (Childs != null)
        {
            for (int i = 0; i < Childs.Length; i++)
            {
                Childs[i].layer = DataSet.Instance.Layers.OverlayObject;
            }
        }

        return true;
    }

    public virtual bool GrabOff(Vector3 position, Vector3 force)                       //�÷��̾ �������� ���Ҵ�.
    {
        GRAB = false;

        if (Rigidbody) // �����尡 ������ ���
        {
            Rigidbody.isKinematic = false;
            Rigidbody.linearVelocity = Vector3.zero;

            if(force != Vector3.zero)Rigidbody.AddForce(force, ForceMode.Impulse);
        }

        //Debug.Log($"Position : {position}");

        //������Ʈ ����
        if(Parent) transform.parent = Parent;
        else transform.parent = MapManager.instance.DefaultObjectParent;

        transform.position = position;
        gameObject.layer = DataSet.Instance.Layers.GetLayerTypeToLayer(MyLayer);  //�������� ���̾�� ����
        if (Childs != null)
        {
            for (int i = 0; i < Childs.Length; i++)
            {
                Childs[i].layer = DataSet.Instance.Layers.GetLayerTypeToLayer(MyLayer);
            }
        }
        return true;
    }

    //���� ������Ʈ�� �߽� ��ǥ�� �޾ƿ´�.
    public Vector3 GetPivotPosition()
    {
        if (PivotObject != null) return PivotObject.position;
        else return transform.position;
    }

    public void SetProgress(ProgressObject _po)
    {
        _progressObject = _po;
    }

    public void ShowText()
    {
        Debug.Log($"{transform.name} �� ȹ��[���] �ϼ̽��ϴ�.");
        UI.activityUI.SetItemText(LocalLanguageSetting.Instance.GetLocalText(LocalizationTable.ItemTable, NAME.Contains("MusicBox") ? "MusicBox":NAME ));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position + new Vector3(offset.x, -offset.y, offset.z), 0.1f);
    }
}

//������Ʈ ������ ������ �ִ´�.
public struct ObjectInfo
{
    public string Name;
    public Transform Trans;
}

//���� ���� ������Ʈ�� ���� ����
public abstract class GlobalSound : InteractionObjects
{
    static int NUMBERING;   //���� �ѹ���
    protected AudioSource _source;

    protected override void Init()
    {
        base.Init();
        //�⺻�� ����
        TYPE = InteractionType.Self;
        _source = GetComponent<AudioSource>();
        Rigidbody = GetComponent<Rigidbody>();
        NAME = NAME + (NUMBERING++).ToString();//�ߺ��� ���� ����.

    }

    protected ObjectInfo GetObjectInfo()
    {
        return new ObjectInfo() { Name = this.NAME, Trans = this.transform };
    }
}

//�ٸ� ������Ʈ�� ��ȣ�ۿ��� ���� ������Ʈ 
//������
public abstract class ExpandObject : InteractionObjects
{
    [Header("��ȣ�ۿ뿡 �ʿ��� ��� ������Ʈ �̸��� �־��")]
    public string TargetObjectName = "Self";
    protected override void Init()
    {
        base.Init();
        //�⺻�� ����
        TYPE = InteractionType.Expand;
    }

    //�ش� ������Ʈ�� �÷��̾� �տ� �ִ� ������Ʈ�� ��ȣ�ۿ��� ������ ���¸� ǥ�����ֱ� ���� ��
    public bool IsInteract(string _targetName)
    {
        //Debug.Log($"�� ĳ���� �Ǿ� Ÿ�ٰ��� ��ȣ�ۿ��� Ȯ���մϴ�. {_targetName}");
        if(TargetObjectName.Equals("Self")) { return true; }  //������Ʈ�� ������ ��ȣ�ۿ��� �� ������ ��Ÿ����.
        if (_targetName == null) return false;                //�Ѿ�� ������Ʈ�̸��� ���ٸ� ��ȣ�ۿ� ����
        return TargetObjectName.Equals(_targetName);
    }


    public abstract override bool Func(string name = null);
}

//������¿��� ������ �۵��ϴ� ������Ʈ
public abstract class EquipAutoObject : InteractionObjects
{

    [Header("��� ���� �ð�")]
    public float duration = 15.0f;
    protected float applyDuration;

    protected override void Init()
    {
        base.Init();
        //�⺻�� ����
        TYPE = InteractionType.EquipAuto;
        Rigidbody = GetComponent<Rigidbody>();

        applyDuration = duration;

        gameObject.SetActive(false);
        //Debug.Log($"EquipAutoObject {transform.name} :: INIT()");
    }
    public override bool Func() // �ڵ��̹Ƿ� �ʿ䰡 ����.
    {
        //Debug.Log($"EquipAutoObject :: Empty Func() / {NAME}");
        return false;
    }
}

public abstract class GetObject : InteractionObjects
{
    protected override void Init()
    {
        base.Init();
        //�⺻�� ����
        TYPE = InteractionType.Get;
        Rigidbody = GetComponent<Rigidbody>();
       // Debug.Log("GetObject :: INIT()");
    }
    public override abstract bool Func();
}