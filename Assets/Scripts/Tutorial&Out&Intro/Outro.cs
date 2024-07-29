using Cinemachine;
using CustomUI;
using System;
using System.Collections;
using UnityEngine;

using Random = UnityEngine.Random;

//������ ���� ��
public class Outro : MonoBehaviour
{
    [SerializeField] Animator animator_Ghost;
    [SerializeField] Animator animator_Door;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] GameObject hat;

    //������
    [SerializeField] Transform demon;

    [SerializeField] GameObject[] particles;

    //�ƿ�Ʈ�� ���� �� ������ ������Ʈ
    [SerializeField] GameObject[] destroyItem;

    [SerializeField] AudioClip Devil1;
    [SerializeField] AudioClip CloseDoor;
    [SerializeField] AudioClip Joker1;
    [SerializeField] AudioClip BGM;
    AudioSource source;

    EndingDance cs_EndingDance;

    private void Start()
    {
        hat.SetActive(false);
        demon.gameObject.SetActive(false);

        source = GetComponent<AudioSource>();

        for (int i = 0; i < particles.Length; i++) { particles[i].SetActive(false); }

        cs_EndingDance = FindObjectOfType<EndingDance>();
        cs_EndingDance.gameObject.SetActive(false);
    }

    public void OutroStart(Action endAction = null)
    {
        //������ �� ī�޶� ������ ����
        PlayerEvent.instance.Init();

        //Fade
        Fade.FadeSetting(true, 4.0f, Color.black);

        //���� Ȱ��
        demon.gameObject.SetActive(true);

        //��� �ͽ� ��Ȱ��ȭ
        EventManager.instance.DeActiveGhost();

        //���� �� Ȱ��ȭ�մϴ�.
        cs_EndingDance.gameObject.SetActive(true);
        cs_EndingDance.Init();

        StartCoroutine(OutroCoutine(endAction));
    }

    IEnumerator OutroCoutine(Action endAction)
    {
        if (!GameConfig.IsPc())
        {
            UI.mobileControllerUI.SetActive(false);
        }

        yield return new WaitUntil(() => Fade.b_faded);
        //ī�޶� ����
        GameManager.Instance.CameraChange(3);


        //���̵� �ƿ� ���
        Fade.FadeSetting(false, 1.0f, Color.black);

        for (int i = 0; i < destroyItem.Length; i++)
        {
            Destroy(destroyItem[i]);
        }

        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", "Outtro1"), 5.0f);
        UI.staticUI.ShowLine();


        //������ ������ ����
        EventManager.instance.DestroyMusicboxSpawner();

        source.volume = DataSet.Instance.SettingValue.Volume* 0.7f;
        source.clip = BGM;
        source.Play();

        //�ִϸ��̼� ����ð� �ʱ�ȭ
        animator_Ghost.Rebind();
        animator_Ghost.SetTrigger("Step1");
        Debug.Log("Ending : Step1 : ������ �����Ѵ�.");

        particles[0].SetActive(true);
        // �ִϸ��̼�  ���
        yield return new WaitUntil(() => (animator_Ghost.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f));


        source.PlayOneShot(Joker1, DataSet.Instance.SettingValue.Volume*1.5f);

        //�ִϸ��̼� ����ð� �ʱ�ȭ
        animator_Ghost.Rebind();
        animator_Ghost.SetTrigger("Step2");
        Debug.Log("Ending : Step2 : ���� ���´�.");

        // �ִϸ��̼�  ���
        yield return new WaitUntil(() => (animator_Ghost.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f));
        particles[1].SetActive(true);
        yield return new WaitForSeconds(0.5f);


        //�ִϸ��̼� ����ð� �ʱ�ȭ
        animator_Ghost.Rebind();
        animator_Ghost.SetTrigger("Step3");
        Debug.Log("Ending : Step3 : ���� ���´�.");
        // �ִϸ��̼�  ���
        yield return new WaitUntil(() => (animator_Ghost.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f));
        yield return new WaitForSeconds(1.0f);


        animator_Door.Rebind();
        animator_Door.SetTrigger("Close");
        Debug.Log("Ending : Close Door");
        source.PlayOneShot(CloseDoor, DataSet.Instance.SettingValue.Volume* 1.5f);
        yield return new WaitUntil(() => (animator_Door.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f));

        //�ִϸ��̼� ��Ȱ��ȭ
        animator_Door.enabled = false;
        animator_Ghost.enabled = false;

        //��ƼŬ ��Ȱ��ȭ
        for (int i = 0; i < particles.Length; i++) { particles[i].SetActive(false); }

        //������ ��ġ ����
        hat.SetActive(true);
        virtualCamera.transform.parent = transform;
        demon.localPosition = new Vector3(1.09000003f, -0.901530862f, 0.899999976f);
        virtualCamera.transform.localPosition = new Vector3(1.24199963f, 0.947000027f, 0.381999969f);
        virtualCamera.transform.localRotation = Quaternion.Euler(21.4896736f, 286.150482f, 6.06662893f);
        hat.GetComponent<Rigidbody>().AddForce(Vector3.right * 8.0f, ForceMode.Impulse);
        hat.GetComponent<Rigidbody>().AddTorque(Vector3.right * 20.0f, ForceMode.Impulse);
        source.PlayOneShot(Devil1, DataSet.Instance.SettingValue.Volume * 1.4f);
        yield return new WaitForSeconds(4.0f);

        //�÷��̾� ���� ��ȯ
        GameManager.Instance.CameraChange(0);
        PlayerEvent.instance.isDead = false;
        GameManager.Instance.DontMove = false;
        
        //������ ���
        AudioManager.instance.PlayEffectiveSound("Sigh", 1.5f);
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", "End"), 7.0f);
        UI.staticUI.ShowLine();

        //����� ��Ʈ�ѷ� Ȱ��ȭ
        if (!GameConfig.IsPc())
        {
            UI.mobileControllerUI.SetActive(true);
        }

        //������ ���� ����
        UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", "SoulsClear"),true, 15.0f);

        //���ǲ���
        source.Stop();

        //������ ����
        if (endAction != null) endAction();
    }

    IEnumerator Shaking(float n)
    {
        Vector3 m_position = virtualCamera.gameObject.transform.position;

        WaitForSeconds waitTime = new WaitForSeconds(0.1f);
        float time = 0.0f;
        while (time < n)
        {
            float t = Random.Range(-0.1f, 0.1f);
            virtualCamera.transform.position = m_position + new Vector3(-t, t, t);
            yield return waitTime;

            time += 0.1f;
        }
    }
}
