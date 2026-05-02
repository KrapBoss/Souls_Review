using CustomUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//도서관 이벤트 종이를 설정합니다.
//종이를 본 후 다시 놓게 된다면 도서관 이벤트가 실행이 되고 빠른 시간안에 촛불을 켜야 됩니다.
public class LibraryPaper : Paper
{
    LibraryEvent libraryEvent;

    public override bool Func()
    {
        if (GameManager.Instance.Tutorial) // 튜토리얼 중에는 금지
        {
            return false;
        }

        if (!GRAB)
        {
            GameManager.Instance.FrozenOnGame(FrozenOff);
            UI.semiStaticUI.ShowPaper(textArea);
            AudioManager.instance.PlayEffectiveSound("Paper", 1.0f);

            GRAB = true;
        }
        else
        {
            GameManager.Instance.FrozenOffGame();
        }
        return true;
    }


    public override void FrozenOff()
    {
        base.FrozenOff();

        if (GameManager.Instance.GameStarted)
        {
            //게임 시작
            if (libraryEvent == null) libraryEvent = FindObjectOfType<LibraryEvent>();
            if (!libraryEvent.isActivation)
            {
                libraryEvent.EventStart();
            }
        }
    }
}
