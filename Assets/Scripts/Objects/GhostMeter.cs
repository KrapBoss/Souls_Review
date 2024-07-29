using CustomUI;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// �ֺ� Layer : Ghost�� Ž���Ѵ�.
/// Ž�� ������ �⺻������ ��û �а�, �ͽŵ��� ��ġ�� ��ħ�� ǥ���� �ش�.
/// �ͽŵ��� ��ġ�� UI�� Ȱ��ȭ ��Ű��, �� ��ġ�� ȭ��ǥ�� ǥ�����ش�.
/// </summary>
public class GhostMeter : EquipAutoObject
{
    public GameObject Needle; // ���ͱ� ��ħ

    public Light _light;

    public LayerMask layerMask;

    float time;                 //�ð� ����
    float from,to;              //�ٴ��� �ִ� ������    

    float time_needle = 1.5f;   //�ٴ� �ð� ���� �� ->�� / ��-> �� �����̴� �� �ð�/s
    short needleCount = 0;      //�ٴ��� ������ Ƚ��
    bool searched = false;      //Ž���� �Ϸ��ߴ��� �Ǵ�.

    public float radius = 20.0f;

    //�� �ͽŵ� ���̿� ������ ���� ����
    List<float> direction = new List<float>();


    short ActivationCount = 0;
    //UI ����
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
        //�� �����ִ� Ƚ�� ����
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

        //���ӽð��� 0.5f ���� ������ ��� �׷����� ���Ѵ�.
        if (applyDuration < duration * 0.5f)
        {
            AudioManager.instance.PlayEffectiveSound("Beep", 1.0f);
            return false;
        }


        //���� ����
        from = -38;
        to = 38;
        time = 0.0f;
        needleCount = 0;

        //�� Ȱ��ȭ
        _light.enabled = true;

        //���� UI ǥ��
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
        if (GRAB) //�׷��� ���
        {
            //���ӽð��� ���ҽ�Ų��.
            //applyDuration -= Time.deltaTime;

            // 1.���ͱ� ��ħ�� 1�ʰ����� �¿�� �����̸� �����Ѵ�.
            time += Time.deltaTime;
            float needle_z = Mathf.Lerp(from, to, (time / time_needle));
            Needle.transform.localRotation = Quaternion.Euler(0, 0, needle_z);
            if (needle_z == to)
            { float temp = from; from = to; to = temp; time = 0.0f; needleCount++; searched = false; }


            if (needleCount % 2 == 0 && !searched)
            { 
                searched = true;    // �ߺ� Ž���� ���� ����.

                needleCount = 0;    //���� ���� �ʱ�ȭ

                //������ �ͽŰ� ������ ����
                GetDirection(Physics.OverlapSphere(transform.position, radius, layerMask));

                UI.semiStaticUI.GhostMeterDirection(direction);

                AudioManager.instance.PlayEffectiveSound("Detector", 1.0f);
            }
        }
    }

    Vector3 v1_to, v2_from;
    Vector3 dic;
    //�� �ͽŵ��� �Ÿ��� ������ �����Ѵ�.
    void GetDirection(Collider[] coll)
    {
        direction.Clear();

        if (coll.Length == 0) return;

        //���� �÷��̾� ��ġ
        v2_from.Set(PlayerEvent.instance.transform.position.x,0, PlayerEvent.instance.transform.position.z);

        foreach (var _col in coll)
        {
            //��� ��ġ
            v1_to.Set(_col.transform.position.x,0, _col.transform.position.z);

            dic = (v1_to - v2_from).normalized;

            direction.Add(Quaternion.FromToRotation(PlayerEvent.instance.transform.forward, dic).eulerAngles.y);
        }

    }
}
