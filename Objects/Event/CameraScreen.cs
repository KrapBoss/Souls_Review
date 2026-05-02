using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카메라의 시각적 효과를 담당합니다. 
/// </summary>
public class CameraScreen : MonoBehaviour
{
    [Header("For Glitch")]
    /// <summary> 카메라 글리치 쉐이더의 머티리얼 </summary>
    [SerializeField] Material mat_Glitch;
    [SerializeField] float _GlitchOn;
    //[SerializeField] float _FlickerPower;
    //[SerializeField] float _GlitchPower;
    //[SerializeField] float _GlitchFrequency;

    private void Start()
    {
        PlayerEvent.instance.Action_Init += () => GlitchlSet(false);
    }

    public void GlitchlSet(bool on)
    {
        _GlitchOn = on ? 1.0f : 0.0f;
        mat_Glitch.SetFloat("_GlitchOn", _GlitchOn);
    }
}
