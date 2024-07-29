using TMPro;
using UnityEngine;

public class EndingDance : MonoBehaviour
{
    public GameObject[] deactivationObject;

    //빛 들
    public Light light_Spot;

    public Light light_Point;

    public GameObject go_Characters;
    public Animator[] anim_souls;

    public GameObject Colliders;

    [Header("난이도에 따라 활성화 되는 것이 달라집니다.")]
    public GameObject MuseBox;

    public void Init()
    {
        //난이도가 보통 이하일 경우 활성화 하지 않는다.
        if(DataSet.Instance.SettingValue.CurrentDiff < 1) { MuseBox.SetActive(false); }

        // 필요 오브젝트 비활성화
        foreach (GameObject obj in deactivationObject)
        {
            obj.SetActive(false);
        }
    }

    //저장된 애니메이션을 실행합니다.
    public void PlayAnimation(string name)
    {
        foreach(var anim in anim_souls)
        {
            anim.SetBool(name,true);
        }
    }

    //지정된 빛의 컬러를 변경하는 효과
    public void ChangeColor(Color color)
    {
        light_Point.color = color;
    }
}
