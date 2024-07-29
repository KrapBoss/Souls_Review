using Cinemachine;
using CustomUI;
using StarterAssets;
using System.Collections;
using TMPro;
using UnityEngine;

//Ʃ�丮�� ���۰� ��� �Ѵ�.
public class Tutorial : MonoBehaviour
{
    
    public Transform SpawnPosition;

    Animator animator;

    CinemachineVirtualCamera VirtualCamera;

    public AudioClip clip_WakeUp;

    public GameObject canvas;
    public TMP_Text txt;



    public void TutorialStart()
    {
        Debug.Log("Tutorial Start");
        PlayerEvent.instance.SetPostition(SpawnPosition.position);
        PlayerEvent.instance.SetRotation(SpawnPosition.rotation);

        canvas.SetActive(true);

        //* ī�޶� ����
        VirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        VirtualCamera.Priority = 10;
        animator= VirtualCamera.gameObject.GetComponent<Animator>();

        StartCoroutine(TutorialDelayCroutine());
    }

    StarterAssetsInputs _inputs;
        
    IEnumerator TutorialDelayCroutine()
    {
        txt.text = LocalLanguageSetting.Instance.GetLocalText("Story", "Synopsis") + "\n\n\n [E] Or [Click] is Skip.... ";
        skip = false;

        _inputs = FindObjectOfType<StarterAssetsInputs>();

        yield return new WaitForSeconds(1.0f);

        if (GameConfig.IsPc())
        {
            GameManager.Instance.CursorShow();
        }
        
        //Ʃ�丮���� ����մϴ�.
        yield return new WaitUntil(() => skip || _inputs.funtion);
        _inputs.funtion = false;

        RenderSettings.ambientLight = DataSet.Instance.Color_Evening;

        if (GameConfig.IsPc())
        {
            GameManager.Instance.CursorHide();
        }

        Debug.LogWarning("Ʃ�丮�� ��� �ð� ����");

        canvas.SetActive(false);

        AudioSource audioSource= GetComponent<AudioSource>();
        audioSource.volume = DataSet.Instance.SettingValue.Volume * 1.3f;
        audioSource.Play();

        //�ִϸ��̼� ����
        animator.Rebind();
        animator.SetTrigger("WakeUp");
        yield return new WaitUntil(() => (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f));

        //1. ī�޶� ���� �� �ʿ���°� ����
        VirtualCamera.Priority = -1;
        Destroy(VirtualCamera.gameObject);
        animator = null;

        //2. ��� ����
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main","Tutorial1"), 7.0f);
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main","Tutorial2"), 7.0f);
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main","Tutorial3"), 7.0f);
        UI.staticUI.ShowLine();

        GetComponent<AudioSource>().PlayOneShot(clip_WakeUp, DataSet.Instance.SettingValue.Volume*0.8f);

        //���� ���
        PlayerEvent.instance.isDead = false;
    }

    //���丮�� �����ִ� �ܰ�
    bool skip;
    public void SkipButton()
    {
        skip = true;
    }

    public void Skip()
    {
        Destroy(gameObject);
    }
}