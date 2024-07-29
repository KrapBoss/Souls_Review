using CustomUI;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// 주변 Layer : Ghost를 탐색한다.
/// 탐색 범위가 기본적으로 엄청 넓고, 귀신들의 위치를 시침에 표현해 준다.
/// 귀신들의 위치는 UI에 활성화 시키며, 각 위치를 화살표로 표시해준다.
/// </summary>
public class GhostMeter : EquipAutoObject
{
    public GameObject Needle; // 미터기 시침

    public Light _light;

    public LayerMask layerMask;

    float time;                 //시간 측정
    float from,to;              //바늘의 최대 움직임    

    float time_needle = 1.5f;   //바늘 시간 간격 좌 ->우 / 우-> 좌 움직이는 각 시간/s
    short needleCount = 0;      //바늘의 움직임 횟수
    bool searched = false;      //탐색을 완료했는지 판단.

    public float radius = 20.0f;

    //각 귀신들 사이에 각도를 가질 변수
    List<float> direction = new List<float>();


    short ActivationCount = 0;
    //UI 참조
    //Semi_StaticUI ui;

    protected override void Init()
    {
        base.Init();

        from = -38;
        to = 38;
        time = 0.0f;


        _light.enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public override bool GrabOn(Transform parent)
    {
        //팁 보여주는 횟수 제한
        if (ActivationCount < 1)
        {
            ActivationCount++;
            if (GameConfig.IsPc())
            {
                UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", "TapIsTip"), false);
            }
            else
            {
                UI.semiStaticUI.ShowVideo(VideoType.METER);
            }
        }

        //지속시간이 0.5f 비율 이하일 경우 그랩하지 못한다.
        if (applyDuration < duration * 0.5f)
        {
            AudioManager.instance.PlayEffectiveSound("Beep", 1.0f);
            return false;
        }


        //각도 지정
        from = -38;
        to = 38;
        time = 0.0f;
        needleCount = 0;

        //빛 활성화
        _light.enabled = true;

        //장착 UI 표시
        UI.semiStaticUI.GhostMeterEquip(true);

        GRAB = true;

        return true;
    }

    public override bool GrabOff(Vector3 position, Vector3 force)
    {
        _light.enabled = false;

        UI.semiStaticUI.GhostMeterEquip(false);

        GRAB = false;

        return true;
    }

    private void OnDrawGizmos()
    {
        if (GRAB)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }

    private void Update()
    {
        if (GRAB) //그랩일 경우
        {
            //지속시간을 감소시킨다.
            //applyDuration -= Time.deltaTime;

            // 1.미터기 시침을 1초간격을 좌우로 움직이며 측정한다.
            time += Time.deltaTime;
            float needle_z = Mathf.Lerp(from, to, (time / time_needle));
            Needle.transform.localRotation = Quaternion.Euler(0, 0, needle_z);
            if (needle_z == to)
            { float temp = from; from = to; to = temp; time = 0.0f; needleCount++; searched = false; }


            if (needleCount % 2 == 0 && !searched)
            { 
                searched = true;    // 중복 탐색을 막기 위함.

                needleCount = 0;    //변수 누적 초기화

                //측정된 귀신과 방향을 저장
                GetDirection(Physics.OverlapSphere(transform.position, radius, layerMask));

                UI.semiStaticUI.GhostMeterDirection(direction);

                AudioManager.instance.PlayEffectiveSound("Detector", 1.0f);
            }
        }
    }

    Vector3 v1_to, v2_from;
    Vector3 dic;
    //각 귀신들의 거리의 방향을 측정한다.
    void GetDirection(Collider[] coll)
    {
        direction.Clear();

        if (coll.Length == 0) return;

        //현재 플레이어 위치
        v2_from.Set(PlayerEvent.instance.transform.position.x,0, PlayerEvent.instance.transform.position.z);

        foreach (var _col in coll)
        {
            //상대 위치
            v1_to.Set(_col.transform.position.x,0, _col.transform.position.z);

            dic = (v1_to - v2_from).normalized;

            direction.Add(Quaternion.FromToRotation(PlayerEvent.instance.transform.forward, dic).eulerAngles.y);
        }

    }
}
