using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;

/// <summary>
/// 알림에 대한 매니저
/// 알림 진행 후 현재 진행해야 되는 목표에 대해 리스트를 표기한다.
/// </summary>
public class NoticeManager
{
    private static NoticeManager instance;
    public static NoticeManager Instance
    {
        get
        {
            if(instance == null) instance = new NoticeManager();
            return instance;
        }
    }

    List<string> noticeTexts = new List<string>();
    List<string> noticeTime = new List<string>();

    /// <summary> 새로운 공지가 추가된 경우 </summary>
    public bool IsNew;

    /// <summary> 제일 마지막에 들어온 배열을 받아옵니다. </summary>
    /// <returns></returns>
    public string Peek()
    {
        return noticeTexts.Count > 0 ? noticeTexts[noticeTexts.Count - 1] : "";
    }

    /// <summary> 현재 저장된 모든 알림 창 반환 </summary>
    /// <returns>아무 데이터도 없으면 Null</returns>
    public string[] GetAllNotice()
    {
        IsNew = false;
        return noticeTexts.Count > 0 ? noticeTexts.ToArray() : null;
    }
    public string[] GetAllNoticeTime()
    {
        return noticeTime.Count > 0 ? noticeTime.ToArray() : null;
    }

    /// <summary> 나왔던 알림을 추가 </summary>
    /// <param name="notice"></param>
    public void AddNotice(string notice)
    {
        //기존 알림에 있는 경우
        if (noticeTexts.Contains(notice)) return;

        //노티스 저장
        noticeTexts.Add(notice);
        IsNew = true;

        //시간 저장
        noticeTime.Add(DateTime.Now.ToString("HH : mm : ss"));
    }
}
