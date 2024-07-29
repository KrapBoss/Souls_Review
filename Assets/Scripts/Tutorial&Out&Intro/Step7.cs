using CustomUI;
using System.Collections;
using UnityEngine;

//��ȥ�� ã���ϴ�. ã�� ��ȥ�� ���� ��縦 �մϴ�.
//�̻��� �Ҹ��� ��� ����ġ�µ�, ���⼭ ��� �Ҹ��� �����ϴ�.
public class Step7 : MonoBehaviour
{
    //���� �� ��ȥ�� �Ҹ�
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

        //������� ������� ���ϴ�.
        AudioManager.instance.StopBGMFadeOut();
        GetComponent<AudioSource>().Stop();
        source_Soul.Stop();

        // ���
        UI.staticUI.EnterLine(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro7-1_Line"), 7);
        UI.staticUI.ShowLine();

        yield return new WaitForSeconds(2.0f);

        //����
        //AudioManager.instance.PlayEffectiveSound("ScaryImpact1", 1.0f);
        //�� ������ ���� ��������.
        UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Main", "Intro7_Line_Tip"),true, 10.0f);


        //���� ����
        GameManager.Instance.SetIntroStep(8);
    }
}
