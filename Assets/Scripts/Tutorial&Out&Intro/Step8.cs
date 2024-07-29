using CustomUI;
using StarterAssets;
using System.Collections;
using UnityEngine;


//����� ����.
//����

public class Step8 : MonoBehaviour
{
    public GameObject go_Air;
    public GameObject go_Target;

    public GameObject go_HeartBeat;

    public GameObject go_Block;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Step 8 Start");

            go_Block.SetActive(true);

            GetComponent<BoxCollider>().enabled = false;

            go_Air.GetComponent<AudioSource>().volume = DataSet.Instance.SettingValue.Volume*0.7f;

            StartCoroutine(ActionCroutine());
        }
    }


    IEnumerator ActionCroutine()
    {
        //����� �Բ� �ڵ��� ���ϴ�.

        //�⺻ ��ġ�� ����
        PlayerEvent.instance.SetDefault();

        //������ �Ұ��ϰ� ����� ����.
        PlayerEvent.instance.isDead = true;

        //����� Ȱ��ȭ
        go_Air.SetActive(true);
        //���� �÷��̾��� ���� ����
        Vector2 currentRot = PlayerEvent.instance.GetRotationCameraView();
        //����� ���� ������ �����ɴϴ�.
        Vector3 dir = go_Air.transform.position - PlayerEvent.instance.GetPosition() - new Vector3(0, -0.7f, 0);
        Quaternion look = Quaternion.LookRotation(dir);
        //��ǥ ȸ�� ������
        Vector2 targetRot = new Vector2(look.eulerAngles.x, look.eulerAngles.y);
        //ȸ��
        float t = 0;
        while (t < 1.0f)
        {
            t += Time.deltaTime * 5;
            PlayerEvent.instance.SetCameraView(Vector2.Lerp(currentRot, targetRot, t));
            yield return null;
        }
        //ī�޶� ����
        EventManager.instance.CameraShake(5.0f, 0.05f);
        //ȿ����
        AudioManager.instance.PlayEffectiveSound("ScaryImpact2", 1.0f);

        //��� : ��..����!! ���� tlqkf ���� ������..
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro8_Line"), 5, true);
        //��� : ���̾�!! ������ �̰�...
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro8-1_Line"), 7);
        UI.staticUI.ShowLine();

        //--------------------------------------�÷��̾��� �ӵ� ����!!!--------------------------------
        //FindObjectOfType<FirstPersonController>().MoveSpeed *= 1.3f;

        //���� On
        go_HeartBeat.SetActive(true);
        go_HeartBeat.GetComponent<AudioSource>().volume = DataSet.Instance.SettingValue.Volume * 0.8f;
        go_HeartBeat.GetComponent<AudioSource>().pitch = 1.5f;

        yield return new WaitForSeconds(1.0f);

        currentRot = PlayerEvent.instance.GetRotationCameraView();
        //����� ���� ������ �����ɴϴ�.
        dir = go_Target.transform.position - PlayerEvent.instance.GetPosition();
        look = Quaternion.LookRotation(dir);
        //��ǥ ȸ�� ������
        targetRot = new Vector2(look.eulerAngles.x, look.eulerAngles.y);
        //ȸ��
        t = 0;
        while (t < 1.0f)
        {
            PlayerEvent.instance.SetCameraView(Vector2.Lerp(currentRot, targetRot, t));
            t += Time.deltaTime * 5;
            yield return null;
        }
        go_Air.SetActive(false);

        PlayerEvent.instance.isDead = false;

        //���� ����
        GameManager.Instance.SetIntroStep(9);
    }
}
