using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 게임에 요소에 진행에 필요한 데이터들을 저장한다.
/// 각종 조건에 사용되는 변수 요소들을 공공으로 가지고 있는다.
/// </summary>
/// 

public enum LayerType
{
    Object, OverlayObject, ObjectNonCollsion, NonCastLight, NonDraw, Ghost, Default, Soul
}

public class LayerSet
{
    public int Object, OverlayObject, ObjectNonCollsion, NonCastLight, NonDraw, Ghost, Default, Soul;
    public LayerSet()
    {
        Object = LayerMask.NameToLayer("Object");
        OverlayObject = LayerMask.NameToLayer("OverlayObject");
        ObjectNonCollsion = LayerMask.NameToLayer("ObjectNonCollsion");
        NonDraw = LayerMask.NameToLayer("NonDraw");
        Ghost = LayerMask.NameToLayer("Ghost");
        Default = LayerMask.NameToLayer("Default");
        Soul = LayerMask.NameToLayer("Soul");
    }

    public bool IsObject(int layer)//넘어온 오브젝트인가?
    {
        return (layer == Object) || (layer == ObjectNonCollsion);
    }

    public int GetLayerTypeToLayer(LayerType _type)
    {
        switch (_type)
        {
            case LayerType.Object:
                return Object;
            case LayerType.OverlayObject:
                return OverlayObject;
            case LayerType.ObjectNonCollsion:
                return ObjectNonCollsion;
            case LayerType.NonCastLight:
                return NonCastLight;
            case LayerType.NonDraw:
                return NonDraw;
            case LayerType.Ghost:
                return Ghost;
            case LayerType.Default:
                return Default;
        }
        return Default;
    }
}

//게임 옵션값으로 이 값을 가지고 사용한다. 게임 설정들을 사용한다.
[System.Serializable]
public class SettingValue
{
    public float MouseSensitivity = 0.5f;           //마우스 감도

    public float BGMVoulme = 0.5f;                  //배경음
    public float Volume = 0.5f;                     //기본 사운드 크기

    public int CurrentDiff = 1;//현재 난이도를 나타낸다. 0부터 쉬움

    public float[] ClearTime = new float[3]{ 0.0f ,0.0f ,0.0f };

    public bool IntroClear = false;
}



#if UNITY_ANDROID // Graphic Settingvalue
//게임 그래픽 옵션
[System.Serializable]
public class GraphicSetValue
{
    public int VSync = 1;           // 기본 1
    public int AntiAliasing = 2;    // 기본 2배수
    public float Brightness = 0.5f; // 밝기
    public int Quality = 0;         //기본 0 Row
    public int FPS = 30;            //60프레임
}
#else
//게임 그래픽 옵션
[System.Serializable]
public class GraphicSetValue
{
    public int VSync = 1;           
    public int AntiAliasing = 4;    //2배수 2~8
    public float Brightness = 0.5f; 
    public int Quality = 1;         
    public int FPS = 60;            //60프레임
}
#endif

//게임 난이도 설정
//초기 설정은 모두 보통 난이도 기준 = 1
[System.Serializable]
public class GameDifficulty
{
    public float HoldOnBreath = 8.0f;
    public float GhostBlackCycleTime = 120;
    public float BatterryUsePerSec=2.0f;
    public float BatteryPermanentDamageRate = 1.0f;
    public bool SoulRun = false;
    public float GhostInitSpeedRate = 0.7f;
    public float SoundSensitivity = 1.0f;
    public float CrystalBallRange = 10.0f;
    public float CameraRecovery = 5.0f;
    public int CameraRecoveryBatteryCount = 5;
    public float WireBoxDotSpeed = 180;

    public int SoulCount = 7;
}


public class DataSet
{
    static DataSet instance;

    public static DataSet Instance
    {
        get
        {
            if (instance == null) instance = new DataSet();
            return instance;
        }
    }

    DataSet()
    {
        // PlayerPrefs.DeleteAll();
        Color_Camera = new Color(60.0f/255.0f,180.0f/255.0f,60f/255.0f, 1.0f);
        Color_Default = new Color(50.0f/255.0f, 50.0f / 255.0f, 50.0f / 255.0f);
        Color_Evening = new Color(145.0f / 255.0f, 129.0f / 255.0f, 60.0f / 255.0f);

        //데이터 로드
        SettingValue = PlayerPrefs.Load("Setting", new SettingValue());

        //그래픽 정보 로드
        GraphicSetValue = PlayerPrefs.Load("GraphicSetting", new GraphicSetValue());
        
        //안드로이드의 경우 강제로 row 2로 전환
        if(Application.platform == RuntimePlatform.Android)GraphicSetValue.Quality = 0;

        Layers = new LayerSet();

        GameDifficulty = new GameDifficulty();
    }

    public void Save()
    {
        PlayerPrefs.Save("Setting", SettingValue);
        PlayerPrefs.Save("GraphicSetting", GraphicSetValue);
    }

    public Color GetDefaultColor() { return  Color_Default * GraphicSetValue.Brightness; }


    public LayerSet Layers;

    public SettingValue SettingValue;

    public GameDifficulty GameDifficulty;

    public GraphicSetValue GraphicSetValue;

    //기본 환경 색상
    public Color Color_Default;
    public Color Color_Camera;
    public Color Color_Evening;
}
