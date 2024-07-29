using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// ������ ������Ʈ �Ʒ��� �ִ� ������Ʈ �� ������ ������ŭ�� ������Ʈ�� �̸� Ȱ��ȭ��Ų��.
/// </summary>

[System.Serializable]
public struct ActiveObjects
{
    public string Name;             //Ȱ��ȭ��ų ������Ʈ �θ�
    public GameObject[] Objects;    //Ȱ��ȭ��ų ������Ʈ��
    public int ActivatedNum;        //Ȱ��ȭ�� ����
}
public class ActiveRandomObjects : MonoBehaviour
{
    public ActiveObjects[] activeObjects;

    private void Start()
    {
        InitObjects();

        ActiveObject();
    }

    void InitObjects()
    {
        for( int i = 0;i < activeObjects.Length;i++)
        {
            for(int j = 0; j< activeObjects[i].Objects.Length;j++)
            {
                activeObjects[i].Objects[j].SetActive(false);
            }
        }
    }

    void ActiveObject()
    {
        for (int i = 0; i < activeObjects.Length; i++)
        {
            //Ȱ��ȭ ��ų ����
            int _activeNum = Random.Range(0, activeObjects[i].Objects.Length);

            for (int j = 0; j <= _activeNum; j++)
            {
                while (true)
                {
                    //�������� Ȱ��ȭ��ų ������Ʈ�� ���� �� �ߺ��� �ƴҶ����� Ȱ��ȭ��Ų��.
                    int _activeObj = Random.Range(0, activeObjects[i].Objects.Length);
                    if (activeObjects[i].Objects[_activeObj].activeSelf) continue;
                    activeObjects[i].Objects[_activeObj].SetActive(true);
                    break;
                }
            }

            activeObjects[i].ActivatedNum = _activeNum+1;

            //Debug.Log($"������Ʈ�� �����մϴ�. {activeObjects[i].Name} : {activeObjects[i].ActivatedNum}");
        }
    }

    //��� ���ڸ� �����մϴ�.
    public void SetDestroy()
    {
        for(int i =0; i< activeObjects.Length; ++i)
        {
            foreach (var go in activeObjects[i].Objects)
            {
                Destroy(go);
            }
        }
    }
}
