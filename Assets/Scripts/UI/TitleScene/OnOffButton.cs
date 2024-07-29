using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnOffButton : MonoBehaviour
{
    public GameObject go_Panel;

    [Tooltip("다른 OnOffButton클래스를 가진 버튼을 비활성화합니다.")]
    public OnOffButton[] cs_Deactive;

    //특정 오브젝트에 붙어있는 버튼의 경우
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
