using Cinemachine;
using CustomUI;
using StarterAssets;
using System.Collections;
using TMPro;
using UnityEngine;

//튜토리얼 시작과 제어를 한다.
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

        //* 카메라 설정
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
        
        //튜토리얼을 대기합니다.
        yield return new WaitUntil(() => skip || _inputs.funtion);
        _inputs.funtion = false;

        RenderSettings.ambientLight = DataSet.Instance.Color_Evening;

        if (GameConfig.IsPc())
        {
            GameManager.Instance.CursorHide();
        }

        Debug.LogWarning("튜토리얼 대기 시간 종료");

        canvas.SetActive(false);

        AudioSource audioSource= GetComponent<AudioSource>();
        audioSource.volume = DataSet.Instance.SettingValue.Volume * 1.3f;
        audioSource.Play();

        //애니메이션 실행
        animator.Rebind();
        animator.SetTrigger("WakeUp");
        yield return new WaitUntil(() => (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f));

        //1. 카메라 변경 및 필요없는거 제거
        VirtualCamera.Priority = -1;
        Destroy(VirtualCamera.gameObject);
        animator = null;

        //2. 대사 실행
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main","Tutorial1"), 7.0f);
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main","Tutorial2"), 7.0f);
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main","Tutorial3"), 7.0f);
        UI.staticUI.ShowLine();

        GetComponent<AudioSource>().PlayOneShot(clip_WakeUp, DataSet.Instance.SettingValue.Volume*0.8f);

        //조작 허용
        PlayerEvent.instance.isDead = false;
    }

    //스토리를 보여주는 단계
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