using CustomUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���࿡ �־� ������Ʈ���� �����ڷ� �� ������Ʈ�� ��ӹ����� �����ڰ� �ȴ�.
/// �����ڴ� ���� ������Ʈ�� ������ ��� �Ϸ� �Ǿ�� ������ �����ϴ�.
/// </summary>
public class ProgressObjectManager : MonoBehaviour
{
    public ProgressObject[] progressObjects;
    public bool isSequence = false; // ������ �����ϴ� ������Ʈ�ΰ�?

    //��� ������Ʈ�� ������ �Ϸ�Ǹ� 
    public bool completed;

    //��� �̼��� �Ϸ�ǰ� �� ���� �� ���� ���´�.
    public string str_key;


    private void Start()
    {
        Init();
    }

    //private :

    //�ʱ� ���� ������ �����ϴµ�, �ʼ������� ȣ���ؾ� �ȴ�.
    protected void Init()
    {
        foreach (var progressObject in progressObjects)
        {
            progressObject.SetManager(this);
            //������ ��� ������ ��� ���� �Ұ� ���·� �����Ѵ�.
            progressObject.canActive = isSequence ? false : true;
        }
        //���� ������� �����ؾ� �Ǵ� ������Ʈ��� 0��° ������Ʈ�� Ȱ��ȭ ��Ų��.
        if (isSequence) progressObjects[0].canActive = true;
    }

    //��� ���� ������Ʈ�� ���۵��� �Ϸ�� ���¸� ��Ÿ����.
    bool AllActionComplete()
    {
        //��� ���۵��� �Ϸ� �Ǿ����� �Ǵ��մϴ�.
        for (int i = 0; i < progressObjects.Length; i++)
        {
            if (!progressObjects[i].activated) return false;
        }
        return true;
    }



//public :

    //���� �ൿ�� ���� �˷��ش�.
    public void ShowNextAction()
    {
        for (int i = 0; i < progressObjects.Length; i++)
        {
            if (!progressObjects[i].activated)
            {
                progressObjects[i].ShowNotice();
                return;
            }
        }

        //������ ��� ������ �Ϸ��ϸ� ���� ��ǥ�� �����ش�.
        UI.topUI.ShowNotice(LocalLanguageSetting.Instance.GetLocalText("Tip", str_key), true);
    }

    //������Ʈ���� �׼��� �Ϸ��ߴٰ� ���� �޽��ϴ�.
    public void CompleteAction(in ProgressObject _progressObject)
    {
        //������ �����ϴ� ������Ʈ�� ��� ���� ������Ʈ�� Ȱ��ȭ �����ݴϴ�.
        if (isSequence)
        {
            for (int i = 0; i < progressObjects.Length; i++)
            {
                if (_progressObject == progressObjects[i])
                {
                    if (i < progressObjects.Length - 1)
                    {
                        //���� ������Ʈ�� ���� ������ ���·� �����Ѵ�.
                        progressObjects[i + 1].canActive = true;
                    }
                }
            }
        }

        //�۾��� �Ϸ� �Ǿ����� ���� ������ �ȳ����ݴϴ�.
        ShowNextAction();
    }

    // ���� ������Ʈ�� ����� �� ������ �����ϸ� true, �ȵǸ� false
    public bool ActionObject()
    {
        if (completed)
        {
            return true;
        }

        if (AllActionComplete())
        {
            Debug.Log($"��� �̺�Ʈ ������ �����Ͽ� �Ϸ��մϴ�.");
            completed = true;
            return true;
        }
        else
        {
            Debug.Log($"���� ���� �ܰ��� �̺�Ʈ�� �������� �ʾ� �۵����� �ʽ��ϴ�.");
            ShowNextAction();
            return false;
        }
    }
}
