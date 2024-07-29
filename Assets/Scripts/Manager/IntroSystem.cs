using CustomUI;
using UnityEngine;

enum InputNames
{
    Sit,
    HoldOnBreath,
    Jump,
    Grab,
    Throwing,
    Func,
    CameraView
}

[System.Serializable]
public struct ObjectsActiveInfo
{
    public GameObject obj;
    public bool active;
}
[System.Serializable]
public class StepObjectInfo // �ܰ躰 ������Ʈ Ȱ�� ��Ȱ�� ������ ��´�.
{
    public ObjectsActiveInfo[] StepObejcts;

    public void SetObjects()
    {
        for (int i = 0; i < StepObejcts.Length; i++)
        {
            StepObejcts[i].obj.SetActive(StepObejcts[i].active);
        }
    }
}

public class IntroSystem : MonoBehaviour
{
    public StepObjectInfo[] StepObejcts;//�ܰ躰 ������Ʈ Ȱ��ȭ������ ����

    public GameObject[] obj_delete; // ������ ������Ʈ�� ������.

    public static bool Active = false; // ��Ʈ�ΰ� Ȱ��ȭ���� ��Ÿ��.

    public void IntroStep(int step)
    {
        if (step > StepObejcts.Length - 1)
        {
            Debug.Log("Intro :: ���� �ܰ谡 �����ϴ�.");
            step = -1;
        }

        Debug.Log($"Intro�� �����մϴ�.:: ���� �ܰ谡 �����ϴ�. {step}");

        Active = true;

        //�����̳� �ش� ������ �غ��Ѵ�.
        switch (step)
        {
            case 0://�÷��̾� Ʈ������ �ʱ�ȭ �� ī�޶� �� ������ Ȱ��ȭ
                Debug.LogWarning("Intro Step 0 :: �÷��̾� Ȱ��ȭ �� ī�޶� �ݱ�");

                //������ ����
                RenderSettings.ambientLight = DataSet.Instance.GetDefaultColor();
                AudioManager.instance.PlayEffectiveSound("Sigh", 1.0f, true);

                //������ ����
                AudioManager.instance.PlayBGMSound("AmbientScary");
                UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", "EnterTheMansion"),true);
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro1"), 3);
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro2"), 5);
                UI.staticUI.ShowLine();
                StepObejcts[0].SetObjects();
                break;
            case 1://���� ����� �ȴ�.
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro3"), 5);
                UI.staticUI.ShowLine();
                StepObejcts[1].SetObjects();
                break;
            case 2://��ġ�� ������ �����Ѵ�.
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro4"), 5);
                UI.staticUI.ShowLine();
                StepObejcts[2].SetObjects();
                break;
            case 3://���þ����� ���� ������ ġ�� �ܰ�
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro5"), 5);
                UI.staticUI.ShowLine();
                StepObejcts[3].SetObjects();
                break;
            case 4:
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro6"), 5);
                UI.staticUI.ShowLine();
                StepObejcts[4].SetObjects();
                break;
            case 5:
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro7"), 5);
                UI.staticUI.ShowLine();
                StepObejcts[5].SetObjects();
                break;
            case 6:
                StepObejcts[6].SetObjects();
                break;
            case 7:
                StepObejcts[7].SetObjects();
                break;
            case 8:
                StepObejcts[8].SetObjects();
                break;
            case 9:
                StepObejcts[9].SetObjects();
                break;

            case -1://��� ���� �� 
                for (int i = 0; i < obj_delete.Length; i++)
                {
                    Destroy(obj_delete[i]);
                }

                //��Ʈ�ΰ� ������ ��� ������Ʈ�� �����մϴ�.
                GameManager.Instance.SkipTutorial();
                break;
        }
    }

    //��ŵ�� �� ���
    public void Skip()
    {
        //���ε��� Ȱ��ȭ
        StepObejcts[4].SetObjects();
        Active = true;
        Destroy(gameObject);
    }
}
