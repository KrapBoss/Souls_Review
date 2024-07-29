using TMPro;
using UnityEngine;

//���� ���� �̸��� ���� ����� ���� �ƴ� ���� �����Ͽ� ������ �����ش�.
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
                    txt = "�̷�....������ ����? ������ �̷� ������!";
                    break;
            }
            video_text.text = txt;

            //���� �Ͻ�����
            GameManager.Instance.FrozenOnGame(FrozenGame);
        }
    }

    public void FrozenGame()
    {
        videoPlayer.Stop();

        gameObject.SetActive(false);
    }
}
