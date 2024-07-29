using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�ʼ� ������ ������ ���� ó��
public class ItemPoint : MonoBehaviour
{

    [Space]
    [Header("Essential Item")]
    public EquipAutoObject ghostMeter;          //1
    public EquipAutoObject ghostCrystalball;    //2
    private short _equipItem = -1;

    Animator anim;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return null;

        anim = GetComponent<Animator>();

        //�⺻���� ��Ȱ��ȭ ����
        ItemActive(1, false);
        ItemActive(2, false);
        _equipItem = -1;


        //������ ��Ȱ��ȭ
        PlayerEvent.instance.Action_Init += () =>
        {
            ItemActive(1, false);
            ItemActive(2, false);
            _equipItem = -1;
        };
    }

    public bool Equip(short num)
    {
        StopAllCoroutines();

        //Debug.Log($"�������� �����մϴ�. {num}");

        //������ ���� ���� ��� �����մϴ�.
        if (PlayerEvent.instance.showVideo)
        {
            PlayerEvent.instance.ShowVideo(VideoType.None);
        }

        //������ �������� ���
        if (num == _equipItem)
        {
            //���� ����
            anim.SetTrigger("UnEquip");
            //������Ʈ ��Ȱ��ȭ
            StartCoroutine(DeActiveDelayCroutine(num));

            //Debug.Log($"������ ������ �������� �ٸ� �������� ������ �����մϴ�. �����Ϸ��� ������ : {num}");

            return true;
        }
        //�ٸ� �������� ��� ���� �ٸ� �ϳ��� �������� ������ �����մϴ�.
        else
        {
            if (num == 1)
            {
                ItemActive(2, false);
            }
            else if (num == 2)
            {
                ItemActive(1, false);
            }
        }


        //Debug.Log($"���ο� �������� �����մϴ�. {num}");
        //������Ʈ Ȱ��ȭ
        ItemActive(num, true);
        //������ ���� �ִϸ��̼�
        anim.SetTrigger("Equip");
        //���� Ȱ�� ������Ʈ ����
        _equipItem = num;

        //Debug.Log($"�������� Ȱ��ȭ �Ǿ�����? ���ͱ� : {ghostMeter.gameObject.activeSelf} || ��Ʈ�� : {ghostCrystalball.gameObject.activeSelf}");

        return true;
    }

    void ItemActive(short index, bool active)
    {
        //Debug.Log($"������ Ȱ�� ����. {index} // {active}");
        //���ͱ�
        if (index  == 1)
        {
            ghostMeter.gameObject.SetActive(active);

            //Ȱ��
            if (active)
            {
                ghostMeter.GrabOn(null);
            }
            else
            {
                ghostMeter.GrabOff(Vector3.zero, Vector3.zero);
            }
        }else 
        //����
        if(index == 2)
        {
            ghostCrystalball.gameObject.SetActive(active);
            if (active)
            {//Ȱ��ȭ
                ghostCrystalball.GrabOn(null);
            }
            else
            {//��Ȱ��ȭ
                ghostCrystalball.GrabOff(Vector3.zero, Vector3.zero);
            }
        }
    }

    //�������� ��Ȱ��ȭ�մϴ�.
    IEnumerator DeActiveDelayCroutine(short index)
    {
        yield return new WaitForSeconds(0.15f);

        //�������� ��Ȱ��ȭ�մϴ�.
        //Debug.Log($"������ Ȱ�� ����. {index}");

        //������ ��Ȱ��ȭ
        ItemActive(index, false);

        //Ȱ�� ������Ʈ ����
        _equipItem = -1;
    }

    public int EquipItemNumber()
    {
        return _equipItem;
    }
}
