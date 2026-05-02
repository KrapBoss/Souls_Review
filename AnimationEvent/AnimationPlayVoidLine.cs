using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static VoiceLineManager;

public class AnimationPlayVoidLine : MonoBehaviour
{
    public VoiceLineData[] voiceLines;

    /// <summary> 실제 음성 대사 시작 </summary>
    public void ShowVoice()
    {
        for (int i = 0; i < voiceLines.Length; i++)
        {
            VoiceLineManager.Instance.EnterLine(voiceLines[i]);
        }
        VoiceLineManager.Instance.ShowLine();
    }
}
