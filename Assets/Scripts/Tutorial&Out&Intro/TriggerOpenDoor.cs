using CustomUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerOpenDoor : MonoBehaviour
{
    public GameObject obj_right;

    bool isActive = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActive)
        {
            isActive = true;
            StartCoroutine(PlayTriggerCroutine());
        }
    }

    IEnumerator PlayTriggerCroutine()
    {
        AudioSource Source = GetComponent<AudioSource>();
        Source.volume = DataSet.Instance.SettingValue.Volume;
        Source.Play();

        float  t = 0.0f;
        while (t < 1.0f)
        {
            obj_right.transform.localRotation = Quaternion.Euler(0,Mathf.Lerp(0,-80,t),0);
            t += Time.deltaTime * (1.2f- (t/1));
            yield return null;
        }

        GameManager.Instance.SetIntroStep(3);
    }
}
