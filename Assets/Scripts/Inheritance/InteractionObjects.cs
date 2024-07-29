using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum InteractionType
{
    Throwing = 0,      //단순히 던지는 오브젝트
    Get,                //획득하는 오브젝트
    Self,               //스스로 상호작용이 가능함. // musicBox
    Expand,             //다른 오브젝트들과 상호작용 가능 //잡을 수 없습니다.
    Continues,          //지속적인 상호작용을 해야 되는 오브젝트
    EquipAuto,           //소지한 상태에서 지속적인 자동으로 작동한다.
    None = -1                //사용할 수 없는 상태가 되었을 때 표시하지 않는다.
}


//기본 오브젝트 설정에 관하여
public abstract class InteractionObjects : MonoBehaviour
{
    [Header("현재 오브젝트 이름을 넣어라")]
    public string NAME = "EnterTheItemName";             //아이템의 이름

    [Header("현재 오브젝트 상호작용 타입")]
    protected InteractionType TYPE;    //아이템의 이벤트 타입
    [Header("현재 오브젝트가 그랩된 상태인가?")]
    public bool GRAB;               //그랩을 당하고 있다.
    [Header("그랩 시 오브젝트 위치값")]
    public Vector3 offset;//해당 오브젝트의 그랩 시 적용되는 y값 오프셋
    [Header("그랩 시 오브젝트 Rotation")]
    public Vector3 offset_rotation;//해당 오브젝트의 그랩 시 적용되는 rotation
    [Header("그랩시 레이어를 같이 적용시킬 오브젝트")]
    public GameObject[] Childs = null;
    [Header("기본 레이어를 지정")]
    public LayerType MyLayer = LayerType.Object;
    [Header("부모를 별도로 지정할 경우 해당 부모 아래에 지정됩니다.")]
    public Transform Parent;

    //[Header("상호작용 오브젝트 중심 표현 좌표 오브젝트")]
    Transform PivotObject = null;
    //[Header("현재 오브젝트의 리지드")]
    protected Rigidbody Rigidbody = null;     //현재의 오브젝트 리지드 바디

    protected ProgressObject _progressObject = null;//진행에 제한을 가하는 오브젝트인경우 null이 아님

    private void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        Rigidbody = GetComponent<Rigidbody>();

        //하위에 존재하는 피벗오브젝트를 찾는다.
        PivotObject = transform.Find("Pivot")?.transform;
        gameObject.layer = DataSet.Instance.Layers.GetLayerTypeToLayer(MyLayer);  //오버레이 레이어로 변경
        if (Childs != null)
        {
            for(int i = 0; i < Childs.Length; i++)
            {
                Childs[i].layer = DataSet.Instance.Layers.GetLayerTypeToLayer(MyLayer);
            }
        }
    }

    //상호작용 키에 대한 것
    public virtual bool Func() { return false; }        //True일 경우 아이템을 사용하면 그랩을 해제한다.
    public virtual bool Func(string name = null) { return false; }
    public virtual void Func(float v) {
        // Debug.Log("값 입력 없는 기본 함수");
                                       }

    public InteractionType GetInteractionType() { return TYPE; }

    public virtual bool GrabOn(Transform parent)        // 플레이어에게 잡혔다.
    {
        //확장형 또는 비활성화 아이템일 경우 그랩이 불가능하다.
        if (TYPE.Equals(InteractionType.Expand) || TYPE.Equals(InteractionType.None)) return false;

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
        gameObject.layer = DataSet.Instance.Layers.OverlayObject;  //오버레이 레이어로 변경
        if (Childs != null)
        {
            for (int i = 0; i < Childs.Length; i++)
            {
                Childs[i].layer = DataSet.Instance.Layers.OverlayObject;
            }
        }

        return true;
    }

    public virtual bool GrabOff(Vector3 position, Vector3 force)                       //플레이어가 아이템을 놓았다.
    {
        GRAB = false;

        if (Rigidbody) // 리지드가 존재할 경우
        {
            Rigidbody.isKinematic = false;
            Rigidbody.velocity = Vector3.zero;

            if(force != Vector3.zero)Rigidbody.AddForce(force, ForceMode.Impulse);
        }

        //Debug.Log($"Position : {position}");

        //오브젝트 정렬
        if(Parent) transform.parent = Parent;
        else transform.parent = MapManager.instance.DefaultObjectParent;

        transform.position = position;
        gameObject.layer = DataSet.Instance.Layers.GetLayerTypeToLayer(MyLayer);  //오버레이 레이어로 변경
        if (Childs != null)
        {
            for (int i = 0; i < Childs.Length; i++)
            {
                Childs[i].layer = DataSet.Instance.Layers.GetLayerTypeToLayer(MyLayer);
            }
        }
        return true;
    }

    //게임 오브젝트의 중심 좌표를 받아온다.
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
        //Debug.Log($"{transform.name} 을 획득[사용] 하셨습니다.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position + new Vector3(offset.x, -offset.y, offset.z), 0.1f);
    }
}

//오브젝트 정보를 가지고 있는다.
public struct ObjectInfo
{
    public string Name;
    public Transform Trans;
}

//전역 사운드 오브젝트에 관한 정보
public abstract class GlobalSound : InteractionObjects
{
    static int NUMBERING;   //고유 넘버링
    protected AudioSource _source;

    protected override void Init()
    {
        base.Init();
        //기본값 설정
        TYPE = InteractionType.Self;
        _source = GetComponent<AudioSource>();
        Rigidbody = GetComponent<Rigidbody>();
        NAME = NAME + (NUMBERING++).ToString();//중복을 막기 위함.

    }

    protected ObjectInfo GetObjectInfo()
    {
        return new ObjectInfo() { Name = this.NAME, Trans = this.transform };
    }
}

//다른 오브젝트와 상호작용을 위한 오브젝트 
//고정형
public abstract class ExpandObject : InteractionObjects
{
    [Header("상호작용에 필요한 대상 오브젝트 이름을 넣어라")]
    public string TargetObjectName = "Self";
    protected override void Init()
    {
        base.Init();
        //기본값 설정
        TYPE = InteractionType.Expand;
    }

    //해당 오브젝트가 플레이어 손에 있는 오브젝트와 상호작용이 가능한 상태를 표시해주기 위한 것
    public bool IsInteract(string _targetName)
    {
        //Debug.Log($"업 캐스팅 되어 타겟과의 상호작용을 확인합니다. {_targetName}");
        if(TargetObjectName.Equals("Self")) { return true; }  //오브젝트가 스스로 상호작용할 수 있음을 나타낸다.
        if (_targetName == null) return false;                //넘어온 오브젝트이름이 없다면 상호작용 실패
        return TargetObjectName.Equals(_targetName);
    }


    public abstract override bool Func(string name = null);
}

//착용상태에서 스스로 작동하는 오브젝트
public abstract class EquipAutoObject : InteractionObjects
{

    [Header("사용 가능 시간")]
    public float duration = 15.0f;
    protected float applyDuration;

    protected override void Init()
    {
        base.Init();
        //기본값 설정
        TYPE = InteractionType.EquipAuto;
        Rigidbody = GetComponent<Rigidbody>();

        applyDuration = duration;

        gameObject.SetActive(false);
        //Debug.Log($"EquipAutoObject {transform.name} :: INIT()");
    }
    public override bool Func() // 자동이므로 필요가 없음.
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
        //기본값 설정
        TYPE = InteractionType.Get;
        Rigidbody = GetComponent<Rigidbody>();
       // Debug.Log("GetObject :: INIT()");
    }
    public override abstract bool Func();
}