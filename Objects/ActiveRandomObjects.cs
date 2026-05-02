using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 지정한 오브젝트 아래에 있는 오브젝트 중 지정한 개수만큼의 오브젝트를 미리 활성화시킨다.
/// </summary>

[System.Serializable]
public struct ActiveObjects
{
    public string Name;             //활성화시킬 오브젝트 부모
    public GameObject[] Objects;    //활성화시킬 오브젝트들
    public int ActivatedNum;        //활성화된 개수
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
            //활성화 시킬 개수
            int _activeNum = Random.Range(0, activeObjects[i].Objects.Length);

            for (int j = 0; j <= _activeNum; j++)
            {
                while (true)
                {
                    //랜덤으로 활성화시킬 오브젝트를 선정 후 중복이 아닐때까지 활성화시킨다.
                    int _activeObj = Random.Range(0, activeObjects[i].Objects.Length);
                    if (activeObjects[i].Objects[_activeObj].activeSelf) continue;
                    activeObjects[i].Objects[_activeObj].SetActive(true);
                    break;
                }
            }

            activeObjects[i].ActivatedNum = _activeNum+1;

            //Debug.Log($"오브젝트를 생성합니다. {activeObjects[i].Name} : {activeObjects[i].ActivatedNum}");
        }
    }

    //모든 액자를 제거합니다.
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
