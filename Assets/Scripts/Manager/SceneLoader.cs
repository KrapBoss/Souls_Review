using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

//���� ���� ������ ������ �ִ´�.
public class SceneInformation
{
    public string SceneName = null;          //�ε��� ���� �̸�
    public Action SceneEndAction = null;     //�� �ε� �� ���� �� ��
    public bool SceneLoading = false;         //�� �ε� �Ϸ� ����
}

public class SceneLoader : MonoBehaviour
{
    public static SceneInformation sceneInformation = new SceneInformation();

    public TMP_Text txt_tip;
    public Image img_progress;
    public Animation anim_dot;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        StartCoroutine(SceneLoad());

        Setting.Action_SoundChanger = null;
        Debug.LogWarning("���� ��ȯ�Ǿ� ���� ü������ �������");
    }

    IEnumerator SceneLoad()
    {
        Debug.LogWarning("���� �ҷ����� ��.....");

        //ȭ�� ���̱�
        Fade.FadeSetting(false,1.0f,Color.white);
        //ȭ�� ���̴� ���� ���
        yield return new WaitUntil(() => Fade.b_faded);


       // Debug.LogWarning("�� �ε� 1");

        //���� �ִ� ����
        txt_tip.text = LocalLanguageSetting.Instance.GetLocalText("Tip", $"Loading{Random.Range(1, 3)}");
        anim_dot.Play();


        //Debug.LogWarning("�� �ε� 2");

        //�� �ε��� ���� �Լ�
        AsyncOperation _operation = SceneManager.LoadSceneAsync(sceneInformation.SceneName);
        _operation.allowSceneActivation = false;


        //Debug.LogWarning("�� �ε� 3");

        //�ε� ���൵ ǥ��
        float _progress = 0.0f;
        while (_progress < 0.89f)
        {
            if (_progress > _operation.progress) { continue; }

            _progress += Time.unscaledDeltaTime * 0.333f;

            img_progress.fillAmount = _progress;

            yield return null;
        }
        
        //1�� �����Ѵ�.
        float time = 0;
        while (time < 1.0f)
        {
            time += Time.unscaledDeltaTime;
            img_progress.fillAmount = Mathf.Lerp(0.9f, 1.0f, time);
        }


        //Debug.LogWarning("�� �ε� 4");

        //���� �ε��Ų��.
        _operation.allowSceneActivation = true;
        //�۾��� ���������� ��� ��Ų��.
        while (!_operation.isDone)
        {
            Debug.Log("���� �ҷ����� �ֽ��ϴ�.");
            yield return null;
        }

        //FadeOut
        Fade.FadeSetting(false, 1.0f, Color.white);


        //Debug.LogWarning("�� �ε� 5");

        //�� �ϸ鼭 ������ ������ �����մϴ�.
        if (sceneInformation.SceneEndAction != null)
        {
            sceneInformation.SceneEndAction();
            sceneInformation.SceneEndAction = null;
        }

        Debug.Log("���� �ε忡 �����߽��ϴ�. : " + sceneInformation.SceneName);

        //���Ḧ �˸�
        sceneInformation.SceneLoading = false;
        Time.timeScale = 1.0f;

        //���� �ε� ������Ʈ ����
        Destroy(gameObject);
    }


    //�� �ε��� ���� �����Լ�
    public static void LoadLoaderScene(string _name, Action _action=null)
    {
        SceneManager.LoadScene("LoadScene");
        SceneLoader.sceneInformation.SceneName = _name;
        SceneLoader.sceneInformation.SceneEndAction = _action;
        SceneLoader.sceneInformation.SceneLoading = false;
    }
}
