using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnOffButton : MonoBehaviour
{
    public GameObject go_Panel;

    [Tooltip("�ٸ� OnOffButtonŬ������ ���� ��ư�� ��Ȱ��ȭ�մϴ�.")]
    public OnOffButton[] cs_Deactive;

    //Ư�� ������Ʈ�� �پ��ִ� ��ư�� ���
    public bool cascade;

    // Start is called before the first frame update
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnOff);

        if(!cascade)go_Panel.SetActive(false);
    }

    public void OnOff()
    {
        go_Panel.SetActive(!go_Panel.activeSelf);
        if (go_Panel.activeSelf && cs_Deactive.Length>0)
        {
            foreach (var cs in cs_Deactive)
            {
                cs.Off();
            }
        }
    }

    public void Off()
    {
        go_Panel.SetActive(false);
    }
}
