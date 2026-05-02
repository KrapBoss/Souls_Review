using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI 애니메이션 동작을 위한 부모
/// </summary>
public abstract class UIAnimationParent : MonoBehaviour
{
    public abstract void Show();

    public abstract void Hide();
}
