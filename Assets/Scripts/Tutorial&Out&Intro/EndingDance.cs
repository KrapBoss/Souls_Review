using TMPro;
using UnityEngine;

public class EndingDance : MonoBehaviour
{
    public GameObject[] deactivationObject;

    //�� ��
    public Light light_Spot;

    public Light light_Point;

    public GameObject go_Characters;
    public Animator[] anim_souls;

    public GameObject Colliders;

    [Header("���̵��� ���� Ȱ��ȭ �Ǵ� ���� �޶����ϴ�.")]
    public GameObject MuseBox;

    public void Init()
    {
        //���̵��� ���� ������ ��� Ȱ��ȭ ���� �ʴ´�.
        if(DataSet.Instance.SettingValue.CurrentDiff < 1) { MuseBox.SetActive(false); }

        // �ʿ� ������Ʈ ��Ȱ��ȭ
        foreach (GameObject obj in deactivationObject)
        {
            obj.SetActive(false);
        }
    }

    //����� �ִϸ��̼��� �����մϴ�.
    public void PlayAnimation(string name)
    {
        foreach(var anim in anim_souls)
        {
            anim.SetBool(name,true);
        }
    }

    //������ ���� �÷��� �����ϴ� ȿ��
    public void ChangeColor(Color color)
    {
        light_Point.color = color;
    }
}
