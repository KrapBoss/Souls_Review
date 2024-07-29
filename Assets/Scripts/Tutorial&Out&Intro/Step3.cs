using CustomUI;
using UnityEngine;

//번개가 치고, 갑자기 어두워 진다.
//카메라는 사라지고, 카메라를 볼 수 있다.
//step 4 로 넘어간다.
public class Step3 : MonoBehaviour
{
    //public GameObject obj_electronic;
    public GameObject obj_rightDoor;

    public GameObject go_lightning;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
           // obj_electronic.SetActive(true);
            obj_rightDoor.transform.localRotation = Quaternion.Euler(0, 0, 0);
            EventManager.instance.CameraShake(2.0f, 0.4f);

            //번개
            UI.topUI.Dazzle(0.7f,1.0f);
            go_lightning.SetActive(true);
            AudioSource _source = go_lightning.GetComponent<AudioSource>();
            _source.volume = DataSet.Instance.SettingValue.Volume;
            _source.Play();

            //끼익 쿵 소리가 난다.
            //MapManagerScripts.instance.SetDirectional(new Vector3(0, 0, 0), false);
            GameManager.Instance.SetIntroStep(4);
        }
    }
}
