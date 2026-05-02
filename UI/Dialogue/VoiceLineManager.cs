using CustomUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 음성과 대사라인을 보여줍니다.
// 리소스는 파일내에서 가지고오며, 없을 경우 대사만 나타납니다.
// 지문의 지속 시간을 음성 파일의 시간으로 지정되며, 없을 경우 5초입니다.
// 한번에 저장된 음성들을 순차대로 출력하며, 새로운 데이터가 들어온 경우 기존 제거됩니다.
public class VoiceLineManager : MonoBehaviour
{
    private static VoiceLineManager _instance;
    public static VoiceLineManager Instance
    {
        get
        {
            if(_instance == null)
            {
                GameObject go = new GameObject("VoiceLineManager");

                _instance = go.AddComponent<VoiceLineManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        //PlayerEvent.instance.Action_Init += ClearVoice;
        sources = gameObject.AddComponent<AudioSource>();
        filter = gameObject.AddComponent<AudioReverbFilter>();

        filter.enabled = false;
    }

    private void OnDestroy()
    {
        if (PlayerEvent.instance) PlayerEvent.instance.Action_Init -= ClearVoice;
        _instance = null;
    }

    AudioSource sources = null; //음성을 재생시킬 오디오 소스
    AudioReverbFilter filter = null;
    Queue<VoiceLineData> datas = new();
    List<VoiceLineData> previousltDatas = new();   // 바로 전에 넣은 데이터

    [Serializable]
    public struct VoiceLineData
    {
        public string clip;     // 재생시킬 클립 경로
        public string line;     // 대사 지문
        public float delay;     // 딜레이 시간
        public bool reverb;     // 울림 효과
    }

    /// <summary> 지문 및 보이스 라인 저장 </summary>
    public void EnterLine(string audioName, string line, float delayTime = 0, bool nonTranslate = false)
    {
        string txt = line;
        if (!string.IsNullOrEmpty(line))
        {
            if (nonTranslate) txt = line;
            else txt = LocalLanguageSetting.Instance.GetLocalText("VoiceLine", line);
        }

        datas.Enqueue(new VoiceLineData {  clip = "Voice/" + audioName, line = txt, delay= delayTime });
    }


    /// <summary> 지문 및 보이스 라인 저장 </summary>
    /// <param name="line">대사 정보</param>
    /// <param name="nonTranslate"> 대사를 번역하지 않음 </param>
    public void EnterLine(VoiceLineData line , bool nonTranslate = false)
    {
        //텍스트 변환이 필요없음
        if(!nonTranslate) line.line = LocalLanguageSetting.Instance.GetLocalText("VoiceLine", line.line);

        line.clip = $"Voice/{line.clip}";
        datas.Enqueue(line);

        previousltDatas.Add(line);
    }

    IEnumerator coroutine;
    /// <summary> 지금까지 저장한 모든 지문을 보여줍니다. </summary>
    /// <param name="force"> 강제로 기존 지문 종료 이후 실행할 것인지 </param>
    public void ShowLine(bool force = true)
    {
        if (coroutine != null && force)
        {
            StopCoroutine(coroutine);
            coroutine = null;
            datas.Clear();

            foreach(VoiceLineData line in previousltDatas)
            {
                datas.Enqueue(line);
            }
        }

        if (coroutine == null)
        {
            previousltDatas.Clear();    //데이터를 제거함
            coroutine = ShowLineCoroutine();
            StartCoroutine(coroutine);
        }
    }

    IEnumerator ShowLineCoroutine()
    {
        while(datas.Count > 0)
        {
            var data = datas.Dequeue();
            if (previousltDatas.Contains(data)) previousltDatas.Remove(data);
            AudioClip clip = null;
            float duration = 5f; // 기본 지속 시간

            if (!string.IsNullOrEmpty(data.clip))
            {
                clip = Resources.Load<AudioClip>(data.clip);
                if (clip != null)
                {
                    //클립이 있다면 딜레이 시간을 적용합니다.
                    yield return new WaitForSeconds(data.delay);

                    //울림 효과
                    if (data.reverb) filter.enabled = true;
                    else filter.enabled = false;

                    sources.volume = DataSet.Instance.SettingValue.Volume * 1.2F;
                    sources.clip = clip;
                    sources.Play();
                    duration = clip.length;
                }
            }

            if (!string.IsNullOrEmpty(data.line)) // 대사가 있는 경우
            {
                // 대사 표시 (지문 UI 따로 연결 필요 시 수정)
                UI.staticUI.EnterLine(data.line, duration);
                UI.staticUI.ShowLine();
            }

            yield return new WaitForSeconds(duration); // 실제 대사 시간만큼 대기

            //메모리 해제
            if (clip != null)
            {
                Resources.UnloadAsset(clip);
            }
        }

        datas.Clear();
        previousltDatas.Clear();
        coroutine = null;

        Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// 실행 중인 모든 데이터를 제거한다.
    /// </summary>
    public void ClearVoice()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            sources.Stop();
            sources.clip = null;
            coroutine = null;
        }

        datas.Clear();

        Resources.UnloadUnusedAssets();
    }


    string[] react = new string[]
    {
        "hansung_react1","hansung_react2","hansung_react3"
    };

    public void PlayRandomHansungReact()
    {
        string str = react[UnityEngine.Random.Range(0, react.Length)];
        VoiceLineManager.Instance.EnterLine(str, string.Empty);
        VoiceLineManager.Instance.ShowLine(false);
    }

    string[] moan = new string[]
    {
        "H_Moan1","H_Moan2"//,"H_Moan3"
    };

    /// <summary> 랜덤 신음 소리 </summary>
    public void PlayRandomHansungMoan()
    {
        string str = moan[UnityEngine.Random.Range(0, moan.Length)];
        VoiceLineManager.Instance.EnterLine(str, string.Empty);
        VoiceLineManager.Instance.ShowLine(false);
    }
}
