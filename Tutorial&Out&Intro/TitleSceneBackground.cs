using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TitleSceneBackground : MonoBehaviour
{
    public Light pointLight;
    public float lightTime = 20.0f;             // 라이트 깜빡임 주기
    public float lightIntensity = 0.7f; // 라이트 밝기
    public float lightIntensityNoise = 0.01f;
    float light_t;
    bool light_croutine; // 라이트 깜빡임이 작동 중인가?

    public GameObject character;

    public AudioClip clip_light;
    AudioSource source;

    IEnumerator Start()
    {
        light_t = lightTime;

        character.SetActive(false);

        source = GetComponent<AudioSource>();
        source.volume = DataSet.Instance.SettingValue.BGMVoulme;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        Setting.Action_SoundChanger += SoundChange;

        //다른 세팅 먼저 끝마치기 위함
        yield return null;

        //이전 종료 전 게임 데이터와 맞추기 위함
        Setting.LoadInitializeSetting();

        Debug.Log("TitleScene를 시작합니다");
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log($"Anti : { (GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset).msaaSampleCount}, Quality : {QualitySettings.GetQualityLevel()} ");

        if (light_t < 0.001f)
        {
            light_t = Random.Range(lightTime * 0.8f, lightTime * 1.3f);
            StartCoroutine(LightBlinkCroutine());
        }
        else
        {
            light_t -= Time.deltaTime;
            if (!light_croutine)
            {
                pointLight.intensity = Mathf.Clamp(Random.Range(pointLight.intensity - lightIntensityNoise,
                                                    pointLight.intensity + lightIntensityNoise),
                                                    lightIntensity * 0.8f, lightIntensity * 1.12f);
            }
        }
    }

    WaitForSeconds waitTime_light = new WaitForSeconds(0.1f);
    IEnumerator LightBlinkCroutine()
    {
        pointLight.color = Color.red;

        character.SetActive(true);
        light_croutine = true;
        source.PlayOneShot(clip_light,DataSet.Instance.SettingValue.Volume * 1.2f);

        short num = 0;
        while (num < 5)
        {
            pointLight.intensity = 0.0f;
            yield return waitTime_light;
            pointLight.intensity = Random.Range(lightIntensity * 0.8f, lightIntensity*1.2f);
            yield return waitTime_light;
            num++;
        }
        light_croutine = false;
        character.SetActive(false);

        pointLight.color = Color.white;
    }

    public void SoundChange()
    {
        Debug.LogWarning("BGM 사운드 변경 :: title 배경음");
        source.volume = DataSet.Instance.SettingValue.BGMVoulme;
    }

    private void OnDestroy()
    {
        Setting.Action_SoundChanger -= SoundChange;
    }
}
