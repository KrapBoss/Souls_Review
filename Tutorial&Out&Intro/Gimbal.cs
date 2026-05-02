using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimbal : InteractionObjects
{
    public Transform parent;

    bool successStep;

    public override bool GrabOff(Vector3 position, Vector3 force)
    {
        StartCoroutine(GrabOffCroutine(position, force));
        return true;
    }
    IEnumerator GrabOffCroutine(Vector3 position, Vector3 force)
    {
        yield return base.GrabOff(position, force);
        this.transform.parent = parent;
    }

    public override bool GrabOn(Transform parent)
    {
        if (!successStep)
        {
            successStep = true;
            GameManager.Instance.SetIntroStep(1);//다음 스텝 시작
        }
        return base.GrabOn(parent);
    }
}
