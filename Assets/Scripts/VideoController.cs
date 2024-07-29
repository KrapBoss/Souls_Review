using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
/// <summary>
/// 설명영상을 저장하고 비디오를 플레이 합니다.
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

        Debug.LogError("비디오를 찾을 수 없습니다.");
    }

    public void Stop() 
    {
        player.Stop();
        player.clip = null;
    }
}
