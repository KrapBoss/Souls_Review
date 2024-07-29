using CustomUI;
using System.Collections;
using UnityEngine;

using Random = UnityEngine.Random;

//���������� �̺�Ʈ�� �߻��ϰ� ��ȥ�� �ع�� �Ǳ��� �ع��� �̷������.
public class LibraryEvent : MonoBehaviour
{
    //������ �̺�Ʈ�� Ȱ��ȭ�� ���������� ��Ÿ���ϴ�.
    public static bool Activation;

    public GameObject BookShelf;
    
    public int ActiveTotemNum;//���� Ȱ��ȭ�� ������ �����Ѵ�.

    //�̺�Ʈ�� �����ؾ� �Ǵ� �ð��� �����մϴ�.
    public float Duration = 60.0f;
    float applyDuration = 0;

    //�̺�Ʈ�� ���� ���ΰ���?
    public bool isActivation;

    //Ŭ��
    public AudioClip SoulsScream;

    //�ͽ�
    public GameObject ghost;
    // �Ǳ� ���� ��ġ
    public Transform GhostSpawnPosition;
    //��ƼŬ
    public GameObject openParticle;

    int ActivatedTotem;//Ȱ��ȭ�� ������ ������ ���Ƹ���.

    public GameObject[] go_totems;

    public Transform tf_totemLamp;
    public GameObject go_totem;

    public AudioClip clip_GasLeak;
    AudioSource source;

    //�� ��� ���� ��
    public Door[] door;

    //���� ȿ��
    public GameObject go_FxSmoke;
    public GameObject go_FxHard;

    public GameObject go_Paper;

    bool first, second;

    bool clear;

    short deadCount =0;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        Init();
        ghost.SetActive(false);
        openParticle.SetActive(false);
        go_Paper.SetActive(false);

        //�׽���
        //Activation = true;

        go_FxHard.SetActive(false);
    }

    private void Update()
    {
        //Ȱ��ȭ �Ǿ� �ִٸ� ���� �ӵ��� ����
        if (isActivation)
        {
            applyDuration += Time.deltaTime;

            float ratio = (applyDuration / Duration);

            source.pitch = 0.8f + ratio;

            if(!first && ratio > 0.5f)
            {
                first = true;
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", "LibraryLine1"), 4.0f);
                UI.staticUI.ShowLine();
            }
            else
                if (!second && ratio > 0.95f)
            {
                second = true;
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", "LibraryLine2"), 4.0f);
                UI.staticUI.ShowLine();
            }
            else
                if(ratio > 1.0f)
            {
                StartCoroutine(GameOverCroutine());
            }
        }
    }

    //������ �̺�Ʈ ���� ������ �ʱ� ��ä�� �����.
    public void Init()
    {
        go_FxSmoke.SetActive(false);

        //
        Activation = false;

        source.pitch = 1;

        first = false;
        second = false;

        //ghost.SetActive(false);
        //���� Ȱ��ȭ
        go_totem.transform.position = (tf_totemLamp.position);
        go_totem.transform.localRotation =  Quaternion.Euler(0, 0, 0);


        //�ʹ� ��ġ ����
        BookShelf.transform.localRotation = Quaternion.Euler(Vector3.zero);

        //���� Ȱ��ȭ�� ���� ����
        ActivatedTotem = 0;

        //��� ��Ȱ��ȭ
        for (int i = 0; i < go_totems.Length; i++)
        {
            go_totems[i].GetComponent<Totem>().LightOFF();
            go_totems[i].SetActive(false);
        }

        //ù��°�� Ȱ��ȭ
        go_totems[0].SetActive(true);

        //Ȱ��ȭ�� ��ȥ�� ���� ������
        ActiveTotemNum = EventManager.instance.GetActivationSouls();

        //������ ������ŭ Ȱ��ȭ��ŵ�ϴ�.
        for (int i = 1; i < ActiveTotemNum; i++)
        {
            while (true)
            {
                int activeNum = Random.Range(0, go_totems.Length);
                //�ּ� 2�� �� ������ ��� ������
                if (!go_totems[activeNum].activeSelf)
                {
                    go_totems[activeNum].SetActive(true);
                    break;
                }
            }
        }
    }

    //������ Ȱ��ȭ �� ��� �Ѱܹް� �ȴ�.
    public void ActiveTotem()
    {
        ActivatedTotem++;

        //Ȱ��ȭ�� ������ ������ Ȱ��ȭ�� ������ ������ ���ٸ�, ���� ����.
        if(ActiveTotemNum == ActivatedTotem)
        {
            clear = true;

            //���� �� ȿ��
            AudioManager.instance.PlayEffectiveSound("DeepAmbient", 1.0f);
            EffectManager.instance.ActiveParticle("ScreenDistortion", PlayerEvent.instance.GetPosition());

            PlayerEvent.instance.Action_Init();
            PlayerEvent.instance.StopPlayerAnimation();

            source.pitch = 1;

            StartCoroutine(OpenBookShelfCroutine());
        }
    }

    //���� �����մϴ�.
    IEnumerator OpenBookShelfCroutine()
    {
        //���� ���ſ� ���� ����
        source.Stop();
        isActivation = false;

        //�������� �Ұ����ϵ��� ����
        GameManager.Instance.DontMove = true;

        //ȭ�� ������
        Fade.FadeSetting(true, 0.5f, Color.black);
        yield return new WaitForSeconds(0.6f);

        //Ŀ�� ����
        UI.activityUI.DeActiveCursor();

        //ī�޶� ����
        GameManager.Instance.CameraChange(2);

        //ȭ�� ���̱�
        Fade.FadeSetting(false, 0.5f, Color.black);
        yield return new WaitForSeconds(0.6f);

        //���� �� ������ ȿ��
        EventManager.instance.CameraShake(3.0f, 0.025f);
        source.Stop();


        //��ȥ ���������� ��ƼŬ Ȱ��ȭ�� ���� ȿ��
        openParticle.SetActive(true);
        source.PlayOneShot(SoulsScream, DataSet.Instance.SettingValue.Volume *1.3f);

        float t = 0;
        while(t < 1.0f)
        {
            BookShelf.transform.localRotation = Quaternion .Euler(new Vector3(0,Mathf.Lerp(0,95,t),0));
            t += Time.deltaTime*0.333f;
            yield return null;
        }

        Debug.LogWarning("Library �̺�Ʈ�� �Ϸ�Ǿ����ϴ�.");


        Debug.LogWarning("-----------��ȥ�� �Ǹ��� Ȱ��ȭ�մϴ�.-------------");

        openParticle.SetActive(true);
        ghost.SetActive(false);

        yield return new WaitForSeconds(2.0f);

        //�������� �����ϵ��� ����
        GameManager.Instance.DontMove = false;
        GameManager.Instance.CameraChange(0);

        //UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", "LibraryLine3"), 4.0f);
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Tip", "Shift"), 5.0f);
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", "LibraryClear"), 10.0f);
        UI.staticUI.ShowLine();

        yield return new WaitForSeconds(1.0f);
        EventManager.instance.ActiveSouls(GhostSpawnPosition.position);

        //���� ��ǥ ����
        UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", "LibraryClear"),true, 15.0f);

        //�� ��� ����
        DoorLock(false);
        go_FxSmoke.SetActive(false);
        //AudioManager.instance.PlayEffectiveSound("JokerLaugh3", 1.0f);

        //��ȥ ���� Ȱ��ȭ
        go_Paper.SetActive(true);

        //�ϵ� ��� �� ���� ���� ����
        if (DataSet.Instance.SettingValue.CurrentDiff == 2)
        {
            go_FxHard.SetActive(true);
            source.PlayOneShot(clip_GasLeak, DataSet.Instance.SettingValue.Volume * 1.2f);
        }

        Activation = true;
        this.enabled = false;
    }

    //������ �̺�Ʈ�� ������ �˸��ϴ�.
    public void EventStart()
    {
        //Ŭ���� ���� ��� ���� �� ��.
        if (clear) return;

        isActivation = true;

        //�Ȱ� Ȱ��ȭ
        go_FxSmoke.SetActive(true);

        //1. ���� ����
        source.pitch = 1.0f;
        source.volume = DataSet.Instance.SettingValue.Volume;
        source.Play();

        //AudioManager.instance.PlayEffectiveSound("JokerLaugh3", 1.0f);
        AudioManager.instance.PlayEffectiveSound("JokerLaugh3", 0.8f);
        source.PlayOneShot(clip_GasLeak, DataSet.Instance.SettingValue.Volume*1.2f); 
        AudioManager.instance.PlayEffectiveSound("DoorClose_Metal", 1.5f);
        EventManager.instance.CameraShake(1.5f, 0.1f);

        //�����
        DoorLock(true);

        //2. ��� ����
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", $"LibraryStart{Random.Range(0,3)}"), 4.0f);
        UI.staticUI.ShowLine();
    }

    short maxLife = 3;
    //�÷��̾ �̺�Ʈ ���� ���߿� �׾��� ��쿡 �ʱ�ȭ�� ����.
    IEnumerator GameOverCroutine()
    {
        if (isActivation)
        {
            isActivation = false;

            //Ÿ�̸� �ʱ�ȭ
            applyDuration = 0;

            //���� ����
            source.Stop();

            PlayerEvent.instance.isDead = true;

            /*
            //������ ��ȯ�մϴ�.
            PlayerEvent.instance.SetCameraView(new Vector2(-30.0f,0));
            EventManager.instance.CameraShake(0.5f, 0.1f);
            ghost.SetActive(true);
            ghost.transform.position = PlayerEvent.instance.GetPosition() + PlayerEvent.instance.transform.forward * 0.5f;
            ghost.transform.rotation = Quaternion.LookRotation(PlayerEvent.instance.GetPosition() - ghost.transform.position);
            AudioManager.instance.PlayEffectiveSound("JokerLaugh1", 0.8f);
            AudioManager.instance.PlayEffectiveSound("HitTheHuman", 1.2f);
            yield return new WaitForSeconds(0.2f);
            */

            //������ ����� ��� ������ ��縦 ǥ��
            if(deadCount == maxLife)
            {
                //�������� ���� ���
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", "LibraryFinally"), 4.0f);
                UI.staticUI.ShowLine();
            }
            else
            {
                //�������� ���� ���
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", "LibraryPaint"), 4.0f);
                UI.staticUI.ShowLine();
            }


            //���� �Ҹ�
            AudioManager.instance.PlayEffectiveSound("Tinnitus", 0.6f);

            //ȭ���� �Ӱ� �����δ�.
            Fade.FadeSetting(true, 2.0f, new Color(1.0f, 0, 0));

            //�÷��̾� �ִϸ��̼� ���¸� Idle�� �����մϴ�.
            PlayerEvent.instance.StopPlayerAnimation();

            yield return new WaitForSeconds(1.0f);

            //3�� �׾��� ��� Ÿ��Ʋ ������ �̵�
            if (deadCount == maxLife)
            {
//MobileOption
                SceneLoader.LoadLoaderScene("TitleScene");
            }
            else
            {
                //���� ȿ��
                PlayerEvent.instance.PlayerFaint(DeadDelay);

                //�̺�Ʈ ���� �ʱ�ȭ
                Init();



                //�� ��� ����
                DoorLock(false);


                //���� Ƚ�� ����
                deadCount++;
            }
        }
    }

    public void DeadDelay()
    {
        switch (deadCount)
        {
            //LibraryDead1-1
            case 1:
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", "LibraryDead1-1"), 3.0f);
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Tip", "Lamp"), 5.0f);
                UI.staticUI.ShowLine();
                break;

            case 2:
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", "LibraryDead2-1"), 3.0f);
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Tip", "Candle"), 5.0f);
                UI.staticUI.ShowLine();
                break;
            case 3:
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", "LibraryDead3-1"), 3.0f);
                UI.staticUI.ShowLine();
                break;
            default:
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Story", "LibraryFinally"), 5.0f);
                UI.staticUI.ShowLine();
                break;
        }

        ghost.SetActive(false);
    }

    //���� ���
    void DoorLock(bool Lock)
    {
        //���� ��� ���
        if (Lock)
        {
            foreach(var d in door)
            {
                //���� �����ִ� ���
                if (d.OpenCloseCheck)
                {
                    d.Func();
                }
                d.isLocked = true;
            }
        }
        //���� �� ���
        else
        {
            foreach (var d in door)
            {
                d.isLocked = false;
            }
        }
    }
    private void OnDestroy()
    {
        Activation = false;
    }
}
