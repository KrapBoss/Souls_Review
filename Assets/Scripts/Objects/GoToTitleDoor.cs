using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 상호작용하면 타이틀로 돌아갑니다.
/// </summary>
public class GoToTitleDoor : ExpandObject
{
    public AudioSource source;
    public override bool Func(string name = null)
    {
        gameObject.layer = 0;

        PlayerEvent.instance.SetDefault();
        PlayerEvent.instance.isDead = true;

        Fade.FadeSetting(true, 5.0f, Color.black);

        source.Play();

        Invoke("GoToTitle", 5.0f);

        return true;
    }

    void GoToTitle()
    {
        SceneLoader.LoadLoaderScene("TitleScene");
    }
}
