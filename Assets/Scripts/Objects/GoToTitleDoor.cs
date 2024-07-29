using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ȣ�ۿ��ϸ� Ÿ��Ʋ�� ���ư��ϴ�.
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
