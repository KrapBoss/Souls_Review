using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    void Start()

    {
        AudioSource source = GetComponent<AudioSource>();
        GetComponent<Button>().onClick.AddListener(() => { Application.Quit(); source.Play(); });       
    }
}
