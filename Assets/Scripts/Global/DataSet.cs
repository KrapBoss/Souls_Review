using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// ���ӿ� ��ҿ� ���࿡ �ʿ��� �����͵��� �����Ѵ�.
/// ���� ���ǿ� ���Ǵ� ���� ��ҵ��� �������� ������ �ִ´�.
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

    public bool IsObject(int layer)//�Ѿ�� ������Ʈ�ΰ�?
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

//���� �ɼǰ����� �� ���� ������ ����Ѵ�. ���� �������� ����Ѵ�.
[System.Serializable]
public class SettingValue
{
    public float MouseSensitivity = 0.5f;           //���콺 ����

    public float BGMVoulme = 0.5f;                  //�����
    public float Volume = 0.5f;                     //�⺻ ���� ũ��

    public int CurrentDiff = 1;//���� ���̵��� ��Ÿ����. 0���� ����

    public float[] ClearTime = new float[3]{ 0.0f ,0.0f ,0.0f };

    public bool IntroClear = false;
}



#if UNITY_ANDROID // Graphic Settingvalue
//���� �׷��� �ɼ�
[System.Serializable]
public class GraphicSetValue
{
    public int VSync = 1;           // �⺻ 1
    public int AntiAliasing = 2;    // �⺻ 2���
    public float Brightness = 0.5f; // ���
    public int Quality = 0;         //�⺻ 0 Row
    public int FPS = 30;            //60������
}
#else
//���� �׷��� �ɼ�
[System.Serializable]
public class GraphicSetValue
{
    public int VSync = 1;           
    public int AntiAliasing = 4;    //2��� 2~8
    public float Brightness = 0.5f; 
    public int Quality = 1;         
    public int FPS = 60;            //60������
}
#endif

//���� ���̵� ����
//�ʱ� ������ ��� ���� ���̵� ���� = 1
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

        //������ �ε�
        SettingValue = PlayerPrefs.Load("Setting", new SettingValue());

        //�׷��� ���� �ε�
        GraphicSetValue = PlayerPrefs.Load("GraphicSetting", new GraphicSetValue());
        
        //�ȵ���̵��� ��� ������ row 2�� ��ȯ
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

    //�⺻ ȯ�� ����
    public Color Color_Default;
    public Color Color_Camera;
    public Color Color_Evening;
}
