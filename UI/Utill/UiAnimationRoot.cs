using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiAnimationRoot : MonoBehaviour
{
    public bool isAwake;
    public bool isStart;
    public bool isOnEnable;

    public UIAnimationParent[] units;

    public float interval = 0.0f;


    private void Awake()
    {
        if (isAwake)
        {
            Show();
        }
    }

    void Start()
    {
        if (isStart)
        {
            Show();
        }
    }
    private void OnEnable()
    {
        if (isOnEnable)
        {
            Show();
        }
    }

    public void OnDisable()
    {
        StopAllCoroutines();
        foreach (var unit in units)
        {
            unit.StopAllCoroutines();
        }
    }

    public void Show()
    {
        StartCoroutine(ShowCor());
    }

    IEnumerator ShowCor()
    {
        WaitForSeconds waitTime = new WaitForSeconds(interval);
        foreach (var unit in units)
        {
            unit.Show();
            yield return waitTime;
        }
    }
}
