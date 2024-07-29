using UnityEngine;

public class TestingGUI : MonoBehaviour
{
    [Range(10, 150)]
    public int fontSize = 30;
    public Color color = new Color(.0f, .0f, .0f, 0.5f);
    public float width, height;


    void OnGUI()
    {
        /*
        // ù ��° ��ư
        if (GUI.Button(new Rect(10, 10, 100, 30), "Quality-0"))
        {
            Debug.Log("��ư 1 Ŭ����!");


            Application.targetFrameRate = 30;
        }
        if (GUI.Button(new Rect(10, 50, 100, 30), "Quality-1"))
        {
            Debug.Log("��ư 2 Ŭ����!");

            //var urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset;

            Application.targetFrameRate = 60;
        }
        if (GUI.Button(new Rect(10, 90, 100, 30), "Quality-2"))
        {
            Debug.Log("��ư 3 Ŭ����!");
            //var urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset;

            Application.targetFrameRate = 120;
        }
        */


        Rect position = new Rect(width, height + fontSize, Screen.width, Screen.height);

        float fps = 1.0f / Time.deltaTime;
        float ms = Time.deltaTime * 1000.0f;

        GUIStyle style = new GUIStyle();
        style.fontSize = fontSize;
        style.normal.textColor = color;

#if UNITY_EDITOR
        string text = $"{Screen.width} X {Screen.height} : {Screen.fullScreenMode} [{Application.platform}] [{GameConfig.Platform}]" +
            $"\n[{Screen.currentResolution.refreshRateRatio.numerator*0.001f}]  " +
            $"[{Application.targetFrameRate}] [{Mathf.CeilToInt((float)(Screen.currentResolution.refreshRateRatio.value))}]" +
            $"\n {fps}";
#else
        string text = string.Format("{0:N1} FPS ({1:N1}ms)", fps, ms);

#endif
        GUI.Label(position, text, style);
    }
}
