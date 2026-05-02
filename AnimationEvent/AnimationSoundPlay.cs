using UnityEngine;

public class AnimationSoundPlay : MonoBehaviour
{
    public void PlayEffectSound(string str)
    {
        AudioManager.instance.PlayEffectiveSound(str, 1.0f);
    }
}
