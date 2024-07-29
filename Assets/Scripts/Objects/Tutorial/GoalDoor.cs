using CustomUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ���������� ����� �Ǵ� ���̴�.
/// ���� ��� ������Ʈ���� ��ȣ�ۿ��� �Ϸ�Ǿ�� ����� �����ϴ�.
/// </summary>
public class GoalDoor : ExpandObject
{
    ProgressObjectManager _progressManager;

    bool activated = false;//������ �Ϸ�Ǿ����� ��Ÿ����.
    
    //���� ��ǥ
    public Transform StartPosition;

    public override bool Func(string name = null)
    {
        if (_progressManager.ActionObject() && !activated)//��� ���� ������Ʈ���� �̺�Ʈ�� �Ϸ�Ǿ���.
        {
            //��ǥ ������Ʈ �̸��� ���ٸ�?
            if (name.Equals(TargetObjectName))
            {
                //���� ����
                StartCoroutine(OpenDoorCroutine());

                StartCoroutine(FuncCroutine());
                return true;
            }
            else
            {
                _progressManager.ShowNextAction();
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    IEnumerator OpenDoorCroutine()
    {
        float t = 0;
        float targetY = transform.localRotation.eulerAngles.y + 90.0f;

        while (t < 1.0f)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Euler(0,Mathf.Lerp(transform.localRotation.eulerAngles.y, targetY,t),0);
            yield return null;
        }
    }

    IEnumerator FuncCroutine()
    {
        activated = true;

        //�׷� ������ ����
        PlayerEvent.instance.DestroyGrabedItem();

        //�÷��̾� �ִϸ��̼� ����
        PlayerEvent.instance.StopPlayerAnimation();

        //��� ���� �Ұ�
        GameManager.Instance.DontMove = true;

        //����
        AudioManager.instance.PlayEffectiveSound("DoorOpen_Common", 1.0f);

        //ȭ�� ������
        Fade.FadeSetting(true, 1.0f, Color.black);
        yield return new WaitForSeconds(1.1f);
        yield return new WaitForSeconds(2.0f);

        //��Ʈ�� ����
        GameManager.Instance.SetIntroStep(0);

        //ȭ�� ���̵� �ƿ�
        Fade.FadeSetting(false, 1.0f, Color.black);

        //��� ���� ����
        GameManager.Instance.DontMove = false;

        PlayerEvent.instance.GrabOff();
        PlayerEvent.instance.SetPostition(StartPosition.position);
        PlayerEvent.instance.SetRotation(Quaternion.Euler(0, 180.0f, 0));

        //Ʃ�丮�� ����
        Destroy(FindObjectOfType<Tutorial>().gameObject);
    }

    protected override void Init()
    {
        base.Init();

        _progressManager = GetComponent<ProgressObjectManager>();
    }
}
