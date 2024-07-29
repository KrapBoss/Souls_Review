
using CustomUI;
using System.Collections;
using UnityEngine;

/// <summary>
/// ���� ������ ĳ���Ͱ� ������ �ٶ󺾴ϴ�.
/// </summary>
public class Step5 : MonoBehaviour
{
    //�߿��� �̺�Ʈ ����
    public Door door_left;
    public Door door_right;

    public GameObject go_Target;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //�� �Ҹ�
            AudioManager.instance.PlayEffectiveSound("DoorClose_Metal", 1.5f);
            
            //ī�޶� ����
            EventManager.instance.CameraShake(0.7f, 0.25f);

            //ȭ�� ��� ȿ��
            Fade.FadeSetting(false, 0.5f, Color.black);

            //���� �����ִٸ�, �ݴ´�.
            if (door_left.OpenCloseCheck) door_left.Func();
            if(door_right.OpenCloseCheck) door_right.Func();

            StartCoroutine(RotateCroutine());
        }
    }

    IEnumerator RotateCroutine()
    {
        //������ �Ұ��ϰ� ����� ����.
        PlayerEvent.instance.isDead = true;

        ////��� ���
        yield return new WaitForSeconds(1.0f);

        //�⺻ ��ġ�� ����
        PlayerEvent.instance.SetDefault();

        //���� �÷��̾��� ���� ����
        Vector2 currentRot = PlayerEvent.instance.GetRotationCameraView();

        //����� ���� ������ �����ɴϴ�.
        Vector3 dir = go_Target.transform.position - (PlayerEvent.instance.GetPosition()+new Vector3(0,1.0f,0));
        Quaternion look = Quaternion.LookRotation(dir);

        //��ǥ ȸ�� ������
        Vector2 targetRot = new Vector2(look.eulerAngles.x, look.eulerAngles.y);

        //ȸ��
        float t = 0;
        while (t < 1.0f)
        {
            PlayerEvent.instance.SetCameraView(Vector2.Lerp(currentRot, targetRot, t));
            t += Time.deltaTime * 8;
            yield return null;
        }
        PlayerEvent.instance.SetCameraView(Vector2.Lerp(currentRot, targetRot, 1));

        //������ Ȱ��
        PlayerEvent.instance.isDead = false;

        //��縦 ���ϴ�.
        //���...? ��...����!!
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro5_Line"), 7);
        UI.staticUI.ShowLine();

        //���� ���� ����
        GameManager.Instance.SetIntroStep(6);
    }
}
