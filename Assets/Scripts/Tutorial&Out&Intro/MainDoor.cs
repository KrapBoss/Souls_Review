using System.Collections;
using UnityEngine;

public class MainDoor : ExpandObject
{
    public GameObject left_Door; public GameObject right_Door;

    int tryNum = 0;// 문을 흔들횟수
    bool canTry = true; // 문을 흔들 수 있는 상태일 경우

    public AudioSource Source;
    public override bool Func(string name = null)
    {
        if(!canTry)return false;

        tryNum++;

        StartCoroutine(ShakeDoor());

        //시도 횟수를 모두 했다면, 다음 스텝 시행
        if (tryNum == 2)
        {
            this.gameObject.layer = 0;
            GameManager.Instance.SetIntroStep(2);
            GetComponent<BoxCollider>().enabled = false;
        }
        return true;
    }

    WaitForSeconds waitTime = new WaitForSeconds(0.1f);
    IEnumerator ShakeDoor()
    {
        Source.volume = DataSet.Instance.SettingValue.Volume;
        Source.Play();

        canTry = false;

        int num = 0;
        float randA = 3.0f;
        float randB = 0.5f;

        while(num < 6) {
            float rotateY = Random.Range(randA, randB);
            randA *= -1; randB *= -1;

            left_Door.transform.localRotation = Quaternion.Euler(0, -rotateY, 0);
            right_Door.transform.localRotation = Quaternion.Euler(0, rotateY, 0);

            num++;
            yield return waitTime;
        }

        left_Door.transform.localRotation = Quaternion.Euler(0, 0, 0);
        right_Door.transform.localRotation = Quaternion.Euler(0, 0, 0);

        canTry = true;

        
    }
}