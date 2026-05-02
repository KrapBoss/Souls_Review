using CustomUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    int index = 0;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            //GameManager.Instance.GameStart(true);
            GameManager.Instance.GameEnd();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            //GameManager.Instance.GameStart(true);
            LibraryEvent.Activation = true;
        }
    }
}
