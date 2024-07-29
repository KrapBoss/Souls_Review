using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//필수 아이템 장착에 대한 처리
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

        //기본적인 비활성화 상태
        ItemActive(1, false);
        ItemActive(2, false);
        _equipItem = -1;


        //아이템 비활성화
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

        //Debug.Log($"아이템을 장착합니다. {num}");

        //영상을 보고 있을 경우 종료합니다.
        if (PlayerEvent.instance.showVideo)
        {
            PlayerEvent.instance.ShowVideo(VideoType.None);
        }

        //동일한 아이템일 경우
        if (num == _equipItem)
        {
            //장착 해제
            anim.SetTrigger("UnEquip");
            //오브젝트 비활성화
            StartCoroutine(DeActiveDelayCroutine(num));

            //Debug.Log($"동일한 아이템 장착으로 다른 아이템의 장착을 해제합니다. 장착하려는 아이템 : {num}");

            return true;
        }
        //다른 아이템일 경우 이전 다른 하나의 아이템의 장착을 해제합니다.
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


        //Debug.Log($"새로운 아이템을 장착합니다. {num}");
        //오브젝트 활성화
        ItemActive(num, true);
        //아이템 장착 애니메이션
        anim.SetTrigger("Equip");
        //현재 활성 오브젝트 변경
        _equipItem = num;

        //Debug.Log($"아이템이 활성화 되었나요? 미터기 : {ghostMeter.gameObject.activeSelf} || 고스트볼 : {ghostCrystalball.gameObject.activeSelf}");

        return true;
    }

    void ItemActive(short index, bool active)
    {
        //Debug.Log($"아이템 활성 정보. {index} // {active}");
        //미터기
        if (index  == 1)
        {
            ghostMeter.gameObject.SetActive(active);

            //활성
            if (active)
            {
                ghostMeter.GrabOn(null);
            }
            else
            {
                ghostMeter.GrabOff(Vector3.zero, Vector3.zero);
            }
        }else 
        //보주
        if(index == 2)
        {
            ghostCrystalball.gameObject.SetActive(active);
            if (active)
            {//활성화
                ghostCrystalball.GrabOn(null);
            }
            else
            {//비활성화
                ghostCrystalball.GrabOff(Vector3.zero, Vector3.zero);
            }
        }
    }

    //아이템을 비활성화합니다.
    IEnumerator DeActiveDelayCroutine(short index)
    {
        yield return new WaitForSeconds(0.15f);

        //아이템을 비활성화합니다.
        //Debug.Log($"아이템 활성 정보. {index}");

        //아이템 비활성화
        ItemActive(index, false);

        //활성 오브젝트 변경
        _equipItem = -1;
    }

    public int EquipItemNumber()
    {
        return _equipItem;
    }
}
