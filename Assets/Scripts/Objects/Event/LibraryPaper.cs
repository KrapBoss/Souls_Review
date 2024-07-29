using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//������ �̺�Ʈ ���̸� �����մϴ�.
//���̸� �� �� �ٽ� ���� �ȴٸ� ������ �̺�Ʈ�� ������ �ǰ� ���� �ð��ȿ� �к��� �Ѿ� �˴ϴ�.
public class LibraryPaper : Paper
{
    LibraryEvent libraryEvent;
    public override void FrozenOff()
    {
        base.FrozenOff();

        if (GameManager.Instance.GameStarted)
        {
            //���� ����
            if (libraryEvent == null) libraryEvent = FindObjectOfType<LibraryEvent>();
            if (!libraryEvent.isActivation)
            {
                libraryEvent.EventStart();
            }
        }
    }
}
