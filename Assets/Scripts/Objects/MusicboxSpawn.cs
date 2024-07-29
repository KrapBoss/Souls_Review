using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �������� �������� �Ǹ� �����մϴ�.
/// ���� �ð����� �������� ���� ������ �Ǻ��մϴ�.
/// </summary>
public class MusicboxSpawn : MonoBehaviour
{
    public Transform spawnPoint;
    public short maxCreate = 3;
    public float createTime = 2.0f;
    float applyCretaTime =0.0f;

    public GameObject musicBox;

    //������� ������ ������ ����
    public short CreatedMusicBox =0;

    // Update is called once per frame
    void Update()
    {
        applyCretaTime -= Time.unscaledDeltaTime;
        if (applyCretaTime <0.0f)
        {
            if(CreatedMusicBox <maxCreate)
            //�ڽ� ������Ʈ�� �������� �ʴ´ٸ�?
            if(spawnPoint.childCount == 0)
            {
                //�����մϴ�
                Instantiate(musicBox,spawnPoint.position, Quaternion.identity,spawnPoint);
                CreatedMusicBox++;
            }
            applyCretaTime = createTime;
        }
    }

    //�������� ������ ������ ���ҽ�Ų��.
    public void MusicBoxDestroy()
    {
        CreatedMusicBox--;
    }
}
