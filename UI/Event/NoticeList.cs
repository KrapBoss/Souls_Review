using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 여태까지 나온 알림들을 보여줍니다.
/// </summary>
public class NoticeList : MonoBehaviour
{
    public Transform contentParent;
    public NoticeListUnit noticeUnit;
    public List<GameObject> noticeList = new List<GameObject>();

    public int limited = 10;

    public int lastIndex;//생성된 유닛 배열의 항상 마지막 위치

    public int startIndex; // 노티스 문자열들의 마지막 위치


    private void Awake()
    {
        for(int i = 0; i < limited; i++)
        {
            GameObject go = Instantiate(noticeUnit.gameObject, contentParent);
            go.SetActive(false);
            noticeList.Add(go);
        }

        startIndex = 0;
        lastIndex = 0;
    }

    private void OnEnable()
    {
        _debug.Log("안내 리스트 활성화");

        Initialize();
    }

    /// <summary> 기존 알림에 없는 새로운 알림에 대해서 무조건 생성 </summary>
    public void Initialize()
    {
        if (NoticeManager.Instance.IsNew) // 새로운 입력이 있는 경우
        {
            var notices = NoticeManager.Instance.GetAllNotice();
            var noticeTimes = NoticeManager.Instance.GetAllNoticeTime();

            if(notices == null)
            {
                _debug.LogWarning("Notice 값 없음");
                return;
            }

            for(int i = startIndex; i < notices.Length; i++) // 생성 제한
            {
                //생성
                var unit = noticeList[lastIndex];
                unit.SetActive(true);

                //가장 처음 배열로 저장
                unit.transform.SetAsFirstSibling();

                var csUnit = unit.GetComponent<NoticeListUnit>();

                //데이터 삽입
                csUnit.content.text = notices[i];
                csUnit.date.text = noticeTimes[i];

                //하나 사용했으니 마지막 위치 저장
                lastIndex = (lastIndex + 1) % limited;
            }

            //마지막 알림 텍스트 위치 저장
            startIndex = notices.Length;
        }
    }


    public void OnBack()
    {
        gameObject.SetActive(false);
    }
}
