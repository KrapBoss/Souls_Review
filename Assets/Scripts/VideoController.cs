using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
/// <summary>
/// �������� �����ϰ� ������ �÷��� �մϴ�.
/// </summary>
/// 
public enum VideoType
{
    METER,
    BALL,
    MUSICBOX,
    None
}

[System.Serializable]
public struct VideoInfor
{
    public VideoType type;
    public VideoClip clip;
}

public class VideoController : MonoBehaviour
{
    public VideoInfor[] videos;

    VideoPlayer player;

    private void Start() {player = GetComponent<VideoPlayer>();}

    public void Play(VideoType _type) {
        if(_type.Equals(VideoType.None)) { Stop(); return; }

        foreach (var video in videos)
        {
            if(video.type.Equals(_type)) {
                player.clip = video.clip;
                player.Play();
                return ;
            }
        }

        Debug.LogError("������ ã�� �� �����ϴ�.");
    }

    public void Stop() 
    {
        player.Stop();
        player.clip = null;
    }
}
