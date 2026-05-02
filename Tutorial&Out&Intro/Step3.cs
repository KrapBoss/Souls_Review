using CustomUI;
using UnityEngine;

//번개가 치고, 갑자기 어두워 진다.
//카메라는 사라지고, 카메라를 볼 수 있다.
//step 4 로 넘어간다.
public class Step3 : MonoBehaviour
{
    public AudioSource source; 
    //public GameObject obj_electronic;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            source.volume = DataSet.Instance.SettingValue.Volume;
            source.Play();

            //문 잠궈
            GameManager.Instance.cs_MapObject.MainDoorLeft.isLocked = true;
            GameManager.Instance.cs_MapObject.MainDoorRight.isLocked = true;

            //문을 닫아
            GameManager.Instance.cs_MapObject.MainDoorLeft.Close();
            GameManager.Instance.cs_MapObject.MainDoorRight.Close();

            GameManager.Instance.SetIntroStep(4);
        }
    }
}
