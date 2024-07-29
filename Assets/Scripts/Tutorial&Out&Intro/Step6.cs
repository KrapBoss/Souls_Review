using CustomUI;
using System.Collections;
using UnityEngine;

/// <summary>
/// ��Ŀ�� �÷��̾ ��Ĩ�ϴ�.
/// </summary>
public class Step6 : MonoBehaviour
{
    //public GameObject Character;
    //public Transform Neck;
    //public Vector3[] NeckRotZ = new Vector3[2];

    //public AudioSource source;

    //[Header("�ι�° ��ȹ")]
    //public GameObject go_Joker;
    //public AudioClip clip_Fx;

    //public Rigidbody rig_Hat;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //���ڸ� ����Ʈ��
            //rig_Hat.gameObject.SetActive(true);
            //rig_Hat.AddForce(Vector3.forward, ForceMode.Impulse);

            //�����
            //source.volume = DataSet.Instance.SettingValue.Volume * 0.7f;
            //source.Play();

            //���� ���
            GetComponent<BoxCollider>().enabled = false;

            //������ �ǽ�
            StartCoroutine(RotateCroutine());

            /*
            Character.SetActive(true);
            PlayerEvent.instance.StopPlayerAnimation();

            success = true;

            AudioManager.instance.PlayEffectiveSound("DeepAmbient", 1.0f,true);
            source.Play();

            //ī�޶� ���������� ���Ұ��ϰ� �����
            if (PlayerEvent.instance.cameraEquipState) FindObjectOfType<CameraController>().CameraOnOff("CameraUnequip", 0f, false);

            PlayerEvent.instance.isDead = true;//������ �Ұ��ϰ� ����� ����.

            StartCoroutine(RotateCroutine());

            eventCollider.enabled = false;
            */
        }
    }

    IEnumerator RotateCroutine()
    {
        //��� ����
        yield return new WaitForSeconds(2.0f);

        //��� : �ƿ�...�� ���� ����.?
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro6_Line"), 7);
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro6-1_Line"), 7);
        UI.staticUI.ShowLine();
        AudioManager.instance.PlayEffectiveSound("Sigh", 1.0f);

        //��� ��� -> ������ Ȯ��
        yield return new WaitForSeconds(10.0f);

        //�˸� ����ֱ�
        UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro6-1_Line_Tip"),true, 15.0f);//����
        AudioManager.instance.PlayEffectiveSound("ScaryImpact1", 1.0f);

        //���� ����
        GameManager.Instance.SetIntroStep(7);

        //��� �Ҹ�
        //source.PlayOneShot(clip_Fx, DataSet.Instance.SettingValue.Volume);

        //�ð��� ����
        //yield return new WaitForSeconds(2.0f);

        /*
        //�⺻ ��ġ�� ����
        PlayerEvent.instance.SetDefault();

        //������ �Ұ��ϰ� ����� ����.
        PlayerEvent.instance.isDead = true;

        //��Ŀ Ȱ��
        go_Joker.SetActive(true);

        //��Ŀ�� �÷��̾� �ڷ� �̵�
        go_Joker.transform.position = PlayerEvent.instance.GetPosition() +
            (-PlayerEvent.instance.transform.forward * .25f);

        //��Ŀ�� �÷��̾ �ٶ󺸰� �ϱ�
        Vector3 joker_dir = PlayerEvent.instance.GetPosition() - go_Joker.transform.position;
        go_Joker.transform.rotation = Quaternion.LookRotation(joker_dir);

        //���� �÷��̾��� ���� ����
        Vector2 currentRot = PlayerEvent.instance.GetRotationCameraView();

        //����� ���� ������ �����ɴϴ�.
        Vector3 dir = go_Joker.transform.position - PlayerEvent.instance.GetPosition();
        Quaternion look = Quaternion.LookRotation(dir);

        //��ǥ ȸ�� ������
        Vector2 targetRot = new Vector2(look.eulerAngles.x, look.eulerAngles.y);

        //ȸ��
        float t = 0;
        while (t < 1.0f)
        {
            PlayerEvent.instance.SetCameraView(Vector2.Lerp(currentRot, targetRot, t));
            t += Time.deltaTime * 5;
            yield return null;
        }
        PlayerEvent.instance.SetCameraView(Vector2.Lerp(currentRot, targetRot, 1));

        //ȿ����
        AudioManager.instance.PlayEffectiveSound("ScaryImpact2", .8f);


        //�� ����
        AudioManager.instance.PlayEffectiveSound("HitTheHuman", 1.5f);

        //���� �Ҹ�
        AudioManager.instance.PlayEffectiveSound("Tinnitus", 1.0f);

        //ȭ�� �����δ�.
        Fade.FadeSetting(true, 2.0f, new Color(0.4f, 0, 0));

        //�ð���
        yield return new WaitForSeconds(1.0f);

        //������ �Ұ��ϰ� ����� ����.
        PlayerEvent.instance.isDead = false;

        //��Ʈ�� ����
        GameManager.Instance.SetIntroStep(7);
        */


        /*
        float t = 0;
        Vector2 currentRot = PlayerEvent.instance.GetRotationCameraView();
        Vector2 targetRot = new Vector2(0, 180.0f);

        AudioManager.instance.PlayEffectiveSound("ScaryImpact1", 1.2f);
        AudioManager.instance.PlayEffectiveSound("DeepAmbient", 1.0f);
        EventManager.instance.CameraShake(0.5f, 0.1f);

        while (t < 1.0f)
        {
            PlayerEvent.instance.SetCameraView(Vector2.Lerp(currentRot, targetRot, t));
            t += Time.deltaTime * 5;
            yield return null;
        }

        PlayerEvent.instance.SetCameraView(Vector2.Lerp(currentRot, targetRot, 1));

        AudioManager.instance.PlayEffectiveSound("ScaryImpact1", 1.2f);

        Character.transform.localPosition = PlayerEvent.instance.GetPosition() + PlayerEvent.instance.transform.forward * 4f;
        Neck.position = PlayerEvent.instance.GetPosition() + PlayerEvent.instance.transform.forward * 3.8f;
        Neck.position = new Vector3(Neck.position.x, PlayerEvent.instance.GetViewHeight() + 0.1f, Neck.position.z);
        Neck.transform.localRotation = Quaternion.Euler(NeckRotZ[0]);

        EventManager.instance.CameraShake(4.8f, 0.025f);
        PlayerEvent.instance.isDead = false;//���� �����ϵ��� ����


        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", "Tutorial2"), 5);
        UI.staticUI.ShowLine();

        yield return new WaitForSeconds(5.0f);

        PlayerEvent.instance.StopPlayerAnimation();
        PlayerEvent.instance.isDead = true;//������ �Ұ��ϰ� ����� ����.
        //���� �ʱ�ȭ
        t = 0;
        currentRot = PlayerEvent.instance.GetRotationCameraView();
        targetRot = new Vector2(0, 180.0f);


        EventManager.instance.CameraShake(0.5f, 0.1f);

        while (t < 1.0f)
        {
            PlayerEvent.instance.SetCameraView(Vector2.Lerp(currentRot, targetRot, t));
            t += Time.deltaTime * 5;
            yield return null;
        }

        AudioManager.instance.PlayEffectiveSound("ScaryImpact2", 1.2f);
        AudioManager.instance.PlayEffectiveSound("DeepAmbient", 1.0f);
        AudioManager.instance.PlayEffectiveSound("Gasp", 1.0f);
        PlayerEvent.instance.SetCameraView(Vector2.Lerp(currentRot, targetRot, 1));

        Character.transform.localPosition = PlayerEvent.instance.GetPosition() + PlayerEvent.instance.transform.forward * 0.5f;
        Neck.position = PlayerEvent.instance.GetPosition() + PlayerEvent.instance.transform.forward * 0.35f;
        Neck.position = new Vector3(Neck.position.x, PlayerEvent.instance.GetViewHeight(), Neck.position.z);
        Neck.transform.localRotation = Quaternion.Euler(NeckRotZ[1]);

        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.SetIntroStep(7);
        */
    }
}