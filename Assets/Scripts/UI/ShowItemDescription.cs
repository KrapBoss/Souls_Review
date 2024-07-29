using TMPro;
using UnityEngine;

//현재 가진 이름에 따라 모바일 기계와 아닌 것을 구분하여 설명을 보여준다.
public class ShowItemDescription : MonoBehaviour
{
    public TMP_Text video_text;

    VideoController videoPlayer;

    public void Show(VideoType type)
    {
        if(videoPlayer == null) { videoPlayer = FindObjectOfType<VideoController>(); }

        if(!type.Equals(VideoType.None))
        {
            videoPlayer.Play(type);

            string txt;
            switch (type)
            {
                case VideoType.METER: // Meter
                    txt = LocalLanguageSetting.Instance.GetLocalText("Tip", "Meter");
                    break;
                case VideoType.BALL: // Ball
                    txt = LocalLanguageSetting.Instance.GetLocalText("Tip", "Chrystal");
                    break;
                case VideoType.MUSICBOX: //Box
                    txt = LocalLanguageSetting.Instance.GetLocalText("Tip", "MusicBox");
                    break;
                default:
                    txt = "이런....오류가 났네? ㅋㅋㅋ 이런 제엔장!";
                    break;
            }
            video_text.text = txt;

            //게임 일시정지
            GameManager.Instance.FrozenOnGame(FrozenGame);
        }
    }

    public void FrozenGame()
    {
        videoPlayer.Stop();

        gameObject.SetActive(false);
    }
}
