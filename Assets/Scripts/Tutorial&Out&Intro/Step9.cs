
using CustomUI;
using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// �� ������ ������ ���ϰ� ������� ���ȿ� ��ƵӴϴ�.
/// </summary>

public class Step9 : ExpandObject
{
    [Space]
    [Header("For Trigger")]
    public GameObject go_Target;

    public GameObject go_Goul1;
    public GameObject go_Goul2;

    public AudioClip clip_Locked;

    public BoxCollider col_TutorialDoor;

    private void OnEnable()
    {
        col_TutorialDoor.enabled = false;
        go_Goul1.SetActive(false);
        go_Goul2.SetActive(false);
    }

    IEnumerator ActionCroutine()
    {
        //���� ����
        source.volume = DataSet.Instance.SettingValue.Volume;
        source.Play();

        //��� : �Ͼ�... �λ� ��.....
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro9_Line"), 4, true);
        UI.staticUI.ShowLine();

        go_Goul1.SetActive(true);

        yield return new WaitForSeconds(4.0f);

        //ī�޶� ����
        EventManager.instance.CameraShake(6.0f, 0.1f);
        //ȿ����
        AudioManager.instance.PlayEffectiveSound("ScaryImpact2", 1.0f);

        ///�� ������
        PlayerEvent.instance.isDead = true;

        Vector3 currentRot = PlayerEvent.instance.GetRotationCameraView();
        //����� ���� ������ �����ɴϴ�.
        Vector3 dir = go_Target.transform.position - PlayerEvent.instance.GetPosition();
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

        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro9-1_Line"), 4, true);
        UI.staticUI.ShowLine();

        yield return new WaitForSeconds(1.5f);

        //�Ѽ� �޾Ƶ���.
        AudioManager.instance.PlayEffectiveSound("Sigh2", 1.0f);
        
        yield return new WaitForSeconds(1.0f);


        Fade.FadeSetting(true, 0.1f, Color.black);
        yield return new WaitForSeconds(0.15f);

        //������ �޷���ϴ�.
        go_Goul2.SetActive(true);
        go_Goul1.SetActive(false);

        //UI ��ȣ�ۿ� ���� ���ֱ�
        UI.activityUI.SetInteractionUI(Vector3.zero, false);
        
        Fade.FadeSetting(false, 0.1f, Color.black);

        //������ �޷���� �Բ� �׽��ϴ�.
        go_Goul2.GetComponent<NavMeshAgent>().SetDestination(PlayerEvent.instance.GetPosition());
        AudioManager.instance.PlayEffectiveSound("GhostScream",1.0f);

        yield return new WaitForSeconds(0.12f);

        Fade.FadeSetting(true, 1.0f, Color.black);

        yield return new WaitForSeconds(1.2f);

        GameManager.Instance.SetIntroStep(10);
    }

    AudioSource source;
    bool activation = false;
    public override bool Func(string name = null)
    {
        //���� // �� ��� �Ҹ�
        source = GetComponent<AudioSource>();
        source.PlayOneShot(clip_Locked, DataSet.Instance.SettingValue.Volume);

        //ī�޶� ����
        EventManager.instance.CameraShake(0.5f, 0.1f);

        //�̺�Ʈ ����
        if (activation) return true;

        activation = true;

        Debug.Log("Step 9 Start");

        StartCoroutine(ActionCroutine());

        return true;
    }
}
