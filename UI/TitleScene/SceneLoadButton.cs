using UnityEngine.UI;
using UnityEngine;

public class SceneLoadButton : MonoBehaviour
{
    public string LoadSceneName;

    public bool Tutorial = false;

    public bool isAd = true;

    private void Start()
    {
        AudioSource source = GetComponent<AudioSource>();

        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (GameConfig.IsPc())
            {
                SceneLoad();
            }
#if UNITY_ANDROID
            else
            {
                if (isAd)
                {
                    GoogleManager.instance.ShowAd(SceneLoad);
                }
                else
                {
                    SceneLoad();
                }
            }
#endif
        });
    }

    public void SceneLoad()
    {
        Fade.FadeSetting(true, 1.0f, Color.black);
        SceneLoader.LoadLoaderScene(LoadSceneName, EnterTheMain);
    }

    public void EnterTheMain()
    {
        if (LoadSceneName.Equals("Main"))
        {
            if (Tutorial)
            {
                GameManager.Instance.GameStart(true);
            }
            else
            {
                GameManager.Instance.GameStart(false);
            }
        }
    }
}
