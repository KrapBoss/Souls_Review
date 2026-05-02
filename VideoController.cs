using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
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
    WHITEGHOST,
    None
}

[System.Serializable]
public struct VideoInfor
{
    public VideoType type;
    public string clipName;
}

public class VideoController : MonoBehaviour
{
    public VideoInfor[] videos;

    public VideoPlayer videoPlayer;
    public string videoFileName = "";  // 원하는 파일명

    private Coroutine loadCoroutine;
    private UnityWebRequest request;

    private void Start() { videoPlayer = GetComponent<VideoPlayer>();}

    public void Play(VideoType _type) {
        if(_type.Equals(VideoType.None)) { Stop(); return; }

        foreach (var video in videos)
        {
            if(video.type.Equals(_type)) {
                //player.clip = video.clip;
                //player.Play();
                videoFileName = $"{video.clipName}.mp4";
                StartVideoLoad();
                return;
            }
        }

        Debug.LogError("비디오를 찾을 수 없습니다.");
    }

    public void StartVideoLoad()
    {
        loadCoroutine = StartCoroutine(LoadVideoFromStreamingAssets());
    }

    public void Stop()
    {
        videoPlayer.Stop();
        videoPlayer.clip = null;

        if (request != null && !request.isDone)
        {
            request.Abort();
            Debug.Log("Video load aborted by user.");
        }

        if (loadCoroutine != null)
        {
            StopCoroutine(loadCoroutine);
            loadCoroutine = null;
        }
    }

    IEnumerator LoadVideoFromStreamingAssets()
    {
        string streamingPath = Path.Combine(Application.streamingAssetsPath, videoFileName);
        string persistentPath = Path.Combine(Application.persistentDataPath, videoFileName);

#if UNITY_ANDROID && !UNITY_EDITOR
        request = UnityWebRequest.Get(streamingPath);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Video load failed: " + request.error);
            yield break;
        }

        File.WriteAllBytes(persistentPath, request.downloadHandler.data);
#else
        if (!File.Exists(persistentPath))
        {
            File.Copy(streamingPath, persistentPath, true);
        }
        yield return null;
#endif

        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = persistentPath;
        videoPlayer.isLooping = true;
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += (vp) => vp.Play();

        loadCoroutine = null;
    }
}
