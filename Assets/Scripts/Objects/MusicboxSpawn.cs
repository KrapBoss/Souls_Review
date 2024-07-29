using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 오르골을 가져가게 되면 스폰합니다.
/// 일정 시간마다 오르골의 존재 유무를 판별합니다.
/// </summary>
public class MusicboxSpawn : MonoBehaviour
{
    public Transform spawnPoint;
    public short maxCreate = 3;
    public float createTime = 2.0f;
    float applyCretaTime =0.0f;

    public GameObject musicBox;

    //만들어진 오르골 개수를 저장
    public short CreatedMusicBox =0;

    // Update is called once per frame
    void Update()
    {
        applyCretaTime -= Time.unscaledDeltaTime;
        if (applyCretaTime <0.0f)
        {
            if(CreatedMusicBox <maxCreate)
            //자식 오브젝트가 존재하지 않는다면?
            if(spawnPoint.childCount == 0)
            {
                //생성합니다
                Instantiate(musicBox,spawnPoint.position, Quaternion.identity,spawnPoint);
                CreatedMusicBox++;
            }
            applyCretaTime = createTime;
        }
    }

    //오르골이 터지면 개수를 감소시킨다.
    public void MusicBoxDestroy()
    {
        CreatedMusicBox--;
    }
}
