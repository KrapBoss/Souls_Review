using CustomUI;
using System.Collections;
using UnityEngine;

//영혼을 찾습니다. 찾은 영혼을 보고 대사를 합니다.
//이상한 소리를 듣고 도망치는데, 여기서 모든 소리가 닫힙니다.
public class Step7 : MonoBehaviour
{
    //서재 뒤 영혼의 소리
    public AudioSource source_Soul;
    bool active = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !active)
        {
            Debug.Log("Step 7 Start");

            StartCoroutine(ActionCroutine());

            active = true;

            //this.gameObject.SetActive(false);
        }
    }

    IEnumerator ActionCroutine()
    {
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro7_Line"), 7);
        UI.staticUI.ShowLine();

        GetComponent<AudioSource>().volume = DataSet.Instance.SettingValue.Volume;

        yield return new WaitForSeconds(5.0f);

        //배경음과 오디오를 끕니다.
        AudioManager.instance.StopBGMFadeOut();
        GetComponent<AudioSource>().Stop();
        source_Soul.Stop();

        // 대사
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro7-1_Line"), 7);
        UI.staticUI.ShowLine();

        yield return new WaitForSeconds(2.0f);

        //사운드
        //AudioManager.instance.PlayEffectiveSound("ScaryImpact1", 1.0f);
        //이 저택을 빠져 나가세요.
        UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro7_Line_Tip"),true, 10.0f);


        //다음 스텝
        GameManager.Instance.SetIntroStep(8);
    }
}
