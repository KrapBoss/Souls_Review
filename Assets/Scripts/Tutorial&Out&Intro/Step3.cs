using CustomUI;
using UnityEngine;

//������ ġ��, ���ڱ� ��ο� ����.
//ī�޶�� �������, ī�޶� �� �� �ִ�.
//step 4 �� �Ѿ��.
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

            //����
            UI.topUI.Dazzle(0.7f,1.0f);
            go_lightning.SetActive(true);
            AudioSource _source = go_lightning.GetComponent<AudioSource>();
            _source.volume = DataSet.Instance.SettingValue.Volume;
            _source.Play();

            //���� �� �Ҹ��� ����.
            //MapManagerScripts.instance.SetDirectional(new Vector3(0, 0, 0), false);
            GameManager.Instance.SetIntroStep(4);
        }
    }
}
