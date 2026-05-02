using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 부딪힐 경우 아이템을 보여줍니다.
/// </summary>
public class ShowRecording : MonoBehaviour
{
    public VideoType type;

    public void OnTriggerEnter(Collider other)
    {
        switch (type)
        {
            case VideoType.METER:
                PlayerEvent.instance.ShowVideo(VideoType.METER);
                GameManager.Instance.DontUseGhostMeter = false;
                break;
            case VideoType.BALL:
                PlayerEvent.instance.ShowVideo(VideoType.BALL);
                GameManager.Instance.DontUseGhostBall = false;
                break;
            case VideoType.MUSICBOX:
                PlayerEvent.instance.ShowVideo(VideoType.MUSICBOX);
                break;
            case VideoType.WHITEGHOST:
                PlayerEvent.instance.ShowVideo(VideoType.WHITEGHOST);
                break;
        }

        GetComponent<BoxCollider>().enabled = false;
    }
}
