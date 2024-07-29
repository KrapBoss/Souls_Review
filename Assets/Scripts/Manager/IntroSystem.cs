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
public class StepObjectInfo // 단계별 오브젝트 활성 비활성 정보를 담는다.
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
    public StepObjectInfo[] StepObejcts;//단계별 오브젝트 활성화정보를 지정

    public GameObject[] obj_delete; // 삭제할 오브젝트를 지정함.

    public static bool Active = false; // 인트로가 활성화됨을 나타냄.

    public void IntroStep(int step)
    {
        if (step > StepObejcts.Length - 1)
        {
            Debug.Log("Intro :: 다음 단계가 없습니다.");
            step = -1;
        }

        Debug.Log($"Intro를 실행합니다.:: 다음 단계가 없습니다. {step}");

        Active = true;

        //제한이나 해당 스텝을 준비한다.
        switch (step)
        {
            case 0://플레이어 트랜스폼 초기화 및 카메라 및 가림막 활성화
                Debug.LogWarning("Intro Step 0 :: 플레이어 활성화 및 카메라 줍기");

                //밤으로 변경
                RenderSettings.ambientLight = DataSet.Instance.GetDefaultColor();
                AudioManager.instance.PlayEffectiveSound("Sigh", 1.0f, true);

                //아이템 놓기
                AudioManager.instance.PlayBGMSound("AmbientScary");
                UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", "EnterTheMansion"),true);
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro1"), 3);
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro2"), 5);
                UI.staticUI.ShowLine();
                StepObejcts[0].SetObjects();
                break;
            case 1://문을 열어야 된다.
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro3"), 5);
                UI.staticUI.ShowLine();
                StepObejcts[1].SetObjects();
                break;
            case 2://망치를 가지러 가야한다.
                UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro4"), 5);
                UI.staticUI.ShowLine();
                StepObejcts[2].SetObjects();
                break;
            case 3://저택안으로 들어가고 번개가 치는 단계
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

            case -1://모두 삭제 후 
                for (int i = 0; i < obj_delete.Length; i++)
                {
                    Destroy(obj_delete[i]);
                }

                //인트로가 끝나고 모든 오브젝트를 삭제합니다.
                GameManager.Instance.SkipTutorial();
                break;
        }
    }

    //스킵을 할 경우
    public void Skip()
    {
        //메인도어 활성화
        StepObejcts[4].SetObjects();
        Active = true;
        Destroy(gameObject);
    }
}
